using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class WindowsPropertySet
    {
        uint mSize;
        string mVersion;
        public Guid mClassID;
        //SimplePropertyRecord mRecord;
        dynamic mPropertySet;

        const uint mVersionSize = 4;

        public WindowsPropertySet(TabControl tabCtrl, ByteReader byteReader, int index = -1)
        {
            TabControl childTab = TabHelper.AddSubTab(tabCtrl, index == -1 ? "PropertySet" : $"PropertySet#{index + 1}");
            byte[] rawData = byteReader.read_bytes(byteReader.read_uint(false));
            TabHelper.AddRawDataTab(childTab, rawData);

            ByteReader propertyReader = new ByteReader(rawData);

            mSize = propertyReader.read_uint();
            mVersion = propertyReader.read_AsciiString(mVersionSize);
            mClassID = propertyReader.read_guid();

            if (mClassID.ToString().ToUpper() == "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3")
            {
                ApplicationShellPropertySets.Handle(childTab, propertyReader);
            }
            else if (mClassID.ToString().ToUpper() == "B9B4B3FC-2B51-4A42-B5D8-324146AFCF25")
            {
                mPropertySet = new TargetParsingPath(childTab, propertyReader);
            }
            else
            {
                mPropertySet = propertyReader.read_remainingbytes();
            }

            string HeaderName = "Set" + (index == -1 ? "" : $"#{index + 1}") + $" ";

            TabHelper.AddStringListTab(childTab, "Header", new string[]{
                    $"mSize: {mSize} (0x{mSize.ToString("X")})",
                    $"mVersion: {mVersion}",
                    $"mClassID: {mClassID.ToString()}"}, 0);
        }
    }
}
