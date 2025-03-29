using Drag_DropDebugger.Helpers;
using Drag_DropDebugger.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.DataHandlers
{
    internal class HTMLHandler
    {
        private static object? HandleUnicodeString(MemoryStream dropData)
        {
            if (dropData is MemoryStream)
                return Encoding.Unicode.GetString(((MemoryStream)dropData).ToArray());

            return null;
        }

        private static object? HandleAsciiString(MemoryStream dropData)
        {
            if (dropData is MemoryStream)
                return Encoding.ASCII.GetString(((MemoryStream)dropData).ToArray());

            return null;
        }

        private static object? HandleHTML(TabControl ParentTab, MemoryStream dropData)
        {
            if (dropData is MemoryStream)
            {
                TabControl childTab = TabHelper.AddSubTab(ParentTab, "text/html");
                TabHelper.AddHTMLTab(childTab, "Preview", Encoding.ASCII.GetString(((MemoryStream)dropData).ToArray()));
                TabHelper.AddHTMLTextTab(childTab, (MemoryStream)dropData);
                return childTab;
            }

            return null;
        }

        private static object? HandleByteFlag(MemoryStream dropData)
        {
            if (dropData is MemoryStream)
                return dropData.ReadByte().ToString();

            return null;
        }

        public object? mTabReference;

        public HTMLHandler(TabControl ParentTab, MemoryStream dropData, string dropType)
        {
            mTabReference = parse(ParentTab, dropData, dropType);
        }

        struct URLElement
        {
            public bool IsURL;
            public uint URLLength;
            public string URL;
            public uint TitleLength;
            public string Title;
            public UInt64 ID;
            public bool hasMetaData;
            public uint MetaDataLength;
            public string MetaDataName;
            public string MetaDataValue;
        }

        static void ProcessElements(StackedDataTab stackTab, ByteReader byteReader, uint count, string path)
        {
            List<URLElement> elements = new List<URLElement>();

            for (int i = 0; i < count; i++)
            {
                if(byteReader.scan_uint() == 0) //ISFOLDER
                {
                    ProcessFolder(stackTab, byteReader, path);
                    continue;
                }

                URLElement element = new URLElement();
                element.IsURL = !(byteReader.read_uint() == 0);
                element.URLLength = byteReader.scan_uint();
                element.URL = byteReader.read_alignedAsciiString();
                element.TitleLength = byteReader.scan_uint();
                element.Title = byteReader.read_alignedUnicodeString();
                element.ID = byteReader.read_uint64();
                element.hasMetaData = (byteReader.read_uint() != 0);
                if(element.hasMetaData)
                {
                    element.MetaDataLength = byteReader.scan_uint();
                    element.MetaDataName = byteReader.read_alignedAsciiString();
                    element.MetaDataValue = byteReader.read_alignedAsciiString();
                }
                elements.Add(element);

                Dictionary<string, object> properties = new Dictionary<string, object>()
                {
                    { "IsURL", element.IsURL },
                    { "Location", path },
                    { "URL", element.URL },
                    { "Title", element.Title },
                    { "ID", element.ID },
                    { "MetaData", element.hasMetaData ? "Present" : "N/A" },
                };

                if (element.hasMetaData)
                    properties.Add(element.MetaDataName, element.MetaDataValue);

                stackTab.AddDataGrid(element.Title != "" ? element.Title : element.URL, properties, (path.Split('/').Length - 1));               
            }
        }

        static void ProcessFolder(StackedDataTab stackTab, ByteReader byteReader, string path)
        {
            List<URLElement> elements = new List<URLElement>();

            URLElement element = new URLElement();
            element.IsURL = !(byteReader.read_uint() == 0);
            element.URLLength = byteReader.scan_uint();
            element.URL = byteReader.read_alignedAsciiString();
            element.TitleLength = byteReader.scan_uint();
            element.Title = byteReader.read_alignedUnicodeString();
            element.ID = byteReader.read_uint64();
            element.hasMetaData = (byteReader.read_uint() != 0);
            if (element.hasMetaData)
            {
                element.MetaDataLength = byteReader.scan_uint();
                element.MetaDataName = byteReader.read_alignedAsciiString();
                element.MetaDataValue = byteReader.read_alignedAsciiString();
            }
            elements.Add(element);

            uint elementCount = byteReader.read_uint();

            Dictionary<string, object> properties = new Dictionary<string, object>()
            {
                { "IsFolder", !element.IsURL },
                { "Location", path },
                { "URL", element.URL },
                { "Title", element.Title },
                { "ID", element.ID },
                { "MetaData", element.hasMetaData ? "Present" : "N/A" },
            };

            if (element.hasMetaData)
                properties.Add(element.MetaDataName, element.MetaDataValue);

            properties.Add("Element Count", elementCount);

            stackTab.AddDataGrid("Folder", properties, (path.Split('/').Length - 1));

            if (elementCount > 0)
            {
                ProcessURLsElements(stackTab, byteReader, elementCount, $"{path}/{element.Title}");
            }
            
        }

        static void ProcessURLsElements(StackedDataTab stackTab, ByteReader byteReader, uint count, string path)
        {
            bool isFolder = byteReader.scan_uint() == 0;
           
            if (count == 1 && isFolder)
            {
                ProcessFolder(stackTab, byteReader, path);
            }
            else
            {
                ProcessElements(stackTab, byteReader, count, path);
            }
           
        }

        private static object? HandleBookmarkEntries(TabControl ParentTab, MemoryStream dropData)
        {
            if (dropData is MemoryStream)
            {
                ByteReader byteReader = new ByteReader(dropData.ToArray());
                uint mSize = byteReader.read_uint();
                uint mProfilePathLength = byteReader.read_uint();
                string mProfilePath = byteReader.read_UnicodeString();
                uint elementCount = byteReader.read_uint();

                StackedDataTab stackTab = new StackedDataTab("chromium/x-bookmark-entries");

                stackTab.AddDataGrid("Properties", new Dictionary<string, object>
                {
                    {"Size", $"{mSize} (0x{mSize.ToString("X")})"},
                    {"Profile Path", mProfilePath },
                    {"Element Count", elementCount },

                });

                TabItem tab = TabHelper.AddStackTab(ParentTab, stackTab);

                ProcessURLsElements(stackTab, byteReader, elementCount, ".");

                return tab;
            }

            return null;
        }

        public static object? parse(TabControl ParentTab, MemoryStream dropData, string dropFormat)
        {
            switch(dropFormat)
            {
                case "UniformResourceLocatorW":
                case "text/x-moz-url":
                    return HandleUnicodeString(dropData);

                case "UniformResourceLocator":
                case "chromium/x-renderer-taint":
                    return HandleAsciiString(dropData);

                case "text/html":
                    return HandleHTML(ParentTab, dropData);

                case "chromium/x-ignore-file-contents":
                    return HandleByteFlag(dropData);

                case "chromium/x-bookmark-entries":
                    return HandleBookmarkEntries(ParentTab, dropData);
            }

            return null;
        }
    }
}
