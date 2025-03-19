using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class TargetParsingPath //{B9B4B3FC-2B51-4A42-B5D8-324146AFCF25}
    {
        uint mSize;
        uint mID; // 0x2
        byte reserved;
        ushort mPropertyType;
        ushort padding; //0x0
        uint mPropertySize;
        string mProperty;

        public TargetParsingPath(TabControl parentTab, ByteReader byteReader)
        {
            mSize = byteReader.read_uint();
            mID = byteReader.read_uint();
            reserved = byteReader.read_byte();
            mPropertyType = byteReader.read_ushort();
            padding = byteReader.read_ushort();
            mPropertySize = byteReader.read_uint();
            mProperty = byteReader.read_UnicodeString();

            TabHelper.AddStringListTab(TabHelper.AddSubTab(parentTab, "TargetParsingPath"),
                "Properties", new string[]
                {
                    $"Size: {mSize}",
                    $"ID: {mID}",
                    $"PropertyType: {mPropertyType}",
                    $"PropertySize: {mPropertySize}",
                    $"Path: {mProperty}",
                }, 0);
        }

    }
}
