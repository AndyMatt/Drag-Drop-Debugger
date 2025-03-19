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
        byte[]? mRecord;
        ApplicationShellPropertySets? mPropertySets;

        const uint mVersionSize = 4;

        public WindowsPropertySet(TabControl tabCtrl, ByteReader byteReader, int index = -1)
        {
            TabControl childTab = TabHelper.AddSubTab(tabCtrl, index == -1 ? "PropertySet" : $"PropertySet#{index + 1}");
            byte[] rawData = byteReader.read_bytes(byteReader.read_uint(false), false);
            TabHelper.AddRawDataTab(childTab, rawData);

            mSize = byteReader.read_uint();
            mVersion = byteReader.read_AsciiString(mVersionSize);
            mClassID = byteReader.read_guid();

            if (mClassID.ToString().ToUpper() == "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3")
            {
                ApplicationShellPropertySets.Handle(tabCtrl, byteReader);
            }
            else
            {
                uint Length = mSize - sizeof(uint) - (uint)Encoding.ASCII.GetByteCount(mVersion) - (uint)byteReader.GetGUIDSize();
                mRecord = byteReader.read_bytes(Length);
            }

            string HeaderName = "Set" + (index == -1 ? "" : $"#{index + 1}") + $" ";

            TabHelper.AddStringListTab(childTab, "Header", new string[]{
                    $"mSize: {mSize} (0x{mSize.ToString("X")})",
                    $"mVersion: {mVersion}",
                    $"mClassID: {mClassID.ToString()}"}, 0);
        }
    }
}
