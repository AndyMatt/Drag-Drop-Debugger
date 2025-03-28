using static Drag_DropDebugger.DataHandlers.ShellIDListArray;
using System.Windows.Controls;
using Drag_DropDebugger.Helpers;

namespace Drag_DropDebugger.Items
{
    public class ApplicationShellItem : TabbedClass
    {
        ushort mSize;
        ushort Unknown_1;
        ushort mInnerSize;
        string mSigniture; //4 Bytes
        ushort mInnerInnerSize;
        uint unknown_2; //0x00030008
        uint unknown_3; //0x0
        ushort unknown_4; //0x0
        uint mPropertyStoreOffset;
        ApplicationShellExtensionBlock? mExtensionBlock;

        List<WindowsPropertySet> mProperties;

        public ApplicationShellItem(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "ApplicationShellItem");
            mSize = byteReader.read_ushort();
            Unknown_1 = byteReader.read_ushort();
            mInnerSize = byteReader.read_ushort();
            mPropertyStoreOffset = byteReader.GetOffset() + mInnerSize;
            mSigniture = byteReader.read_AsciiString(4);
            mInnerInnerSize = byteReader.read_ushort();
            unknown_2 = byteReader.read_uint();
            unknown_2 = byteReader.read_uint();
            unknown_4 = byteReader.read_ushort();

            mProperties = new List<WindowsPropertySet>();

            Dictionary<string, object> properties = new Dictionary<string, object>()
            {
                {"Size", $"{mSize} (0x{mSize.ToString("X")})"},
                {"InnerSize", mInnerSize},
                {"Unknown_1", Unknown_1},
                {"mSigniture", mSigniture},
                {"Size of Property Sets", mInnerInnerSize},
                {"Unknown_2", unknown_2},
                {"Unknown_3", unknown_3},
                {"Unknown_4", unknown_4},
            };

            while (byteReader.read_uint(false) != 0)
            {
                WindowsPropertySet set = new WindowsPropertySet(childTab, byteReader, mProperties.Count);
                properties.Add($"Property Set #{mProperties.Count+1}", set.mTabReference);
                mProperties.Add(set);
            }

            byteReader.SetOffset(mPropertyStoreOffset);
            if (hasExtensionBlock(byteReader))
            {
                mExtensionBlock = new ApplicationShellExtensionBlock(parentTab, byteReader);
                properties.Add($"ApplicationShellExtentionBlock", mExtensionBlock.mTabReference);
            }

            TabHelper.AddDataGridTab(childTab, "Properties", properties, 0);

            mTabReference = childTab;
        }   

        const uint ExtensionSignitureOffset = 4;
        bool hasExtensionBlock(ByteReader byteReader)
        {
            uint signiture = byteReader.scan_uint(ExtensionSignitureOffset);

            if (signiture == 0xBEEF0027)
            {
                return true;
            }

            return false;
        }
    }
}
