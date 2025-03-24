using Drag_DropDebugger.Helpers;
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
                return TabHelper.AddHTMLTab(ParentTab, "text/html", Encoding.ASCII.GetString(((MemoryStream)dropData).ToArray()));

            return null;
        }

        public object? mTabReference;

        public HTMLHandler(TabControl ParentTab, MemoryStream dropData, string dropType)
        {
            mTabReference = parse(ParentTab, dropData, dropType);
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
            }

            return null;
        }
    }
}
