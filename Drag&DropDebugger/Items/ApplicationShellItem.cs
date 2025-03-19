using static Drag_DropDebugger.DataHandlers.ShellIDListArray;
using System.Windows.Controls;
using Drag_DropDebugger.Helpers;

namespace Drag_DropDebugger.Items
{
    public class ApplicationShellItem
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

            TabControl setsTab = TabHelper.AddSubTab(childTab, "PropertySets");

            while (byteReader.read_uint(false) != 0)
            {
                mProperties.Add(new WindowsPropertySet(setsTab, byteReader, mProperties.Count));
            }

            AddTab(childTab);

            byteReader.SetOffset(mPropertyStoreOffset);
            if (hasExtensionBlock(byteReader))
            {
                mExtensionBlock = new ApplicationShellExtensionBlock(parentTab, byteReader);
            }
        }

        public void AddTab(TabControl parentTab)
        {
            TabHelper.AddStringListTab(parentTab, "Header", new string[]{
                $"Size: {mSize} (0x{mSize.ToString("X")}) (0x{mSize.ToString("X")})",
                $"InnerSize: {mInnerSize}",
                $"Unknown: {Unknown_1}",
                $"mSigniture: {mSigniture}",
                $"mInnerInnerSize: {mInnerInnerSize}",
                $"Unknown: {unknown_2}",
                $"Unknown: {unknown_3}",
                $"Unknown: {unknown_4}",
                $"Number of PropertySet: {mProperties.Count}"}, 0);
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
