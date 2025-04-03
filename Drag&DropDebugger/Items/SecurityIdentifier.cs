using Drag_DropDebugger.Helpers;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class SecurityIdentifier : TabbedClass //{46588ae2-4cbc-4338-bbfc-139326986dce}
    {
        uint mSize;
        uint mID; // 0x4
        byte reserved;
        ushort mPropertyType;
        ushort padding; //0x0
        uint mPropertySize;
        string mProperty;

        public SecurityIdentifier(TabControl parentTab, ByteReader byteReader)
        {
            mSize = byteReader.read_uint();
            mID = byteReader.read_uint();
            reserved = byteReader.read_byte();
            mPropertyType = byteReader.read_ushort();
            padding = byteReader.read_ushort();
            mPropertySize = byteReader.read_uint();
            mProperty = byteReader.read_UnicodeString();

            mTabReference = TabHelper.AddDataGridTab(parentTab, "SecurityIdentifier", new Dictionary<string, object>()
            {
                {"Size", $"{mSize} (0x{mSize.ToString("X")})" },
                {"ID", mID },
                {"PropertyType", mPropertyType },
                {"PropertySize", mPropertySize },
                {"SID", mProperty },

            }, 0);
        }

    }
}
