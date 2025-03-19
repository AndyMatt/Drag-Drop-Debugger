using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class ApplicationShellExtensionBlock //0xBEEF0027
    {
        ushort mSize;
        ushort mVersion; // Seen 0x0000
        uint mExtensionSigniture; //0xBEEF0027
        WindowsPropertySet? mPropertySet;
        uint mTerminator;

        public string[] toStringList()
        {
            List<string> list = new List<string>();
            list.Add($"Size: {mSize} (0x{mSize.ToString("X")})");
            list.Add($"Version: {mVersion}");
            list.Add($"ExtensionSigniture: 0x{mExtensionSigniture.ToString("X2")}");
            list.Add($"Terminator: {mTerminator}");
            return list.ToArray();
        }

        public ApplicationShellExtensionBlock(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "ApplicationShellExtentionBlock");
            mSize = byteReader.read_ushort();
            mVersion = byteReader.read_ushort();
            mExtensionSigniture = byteReader.read_uint();

            mPropertySet = new WindowsPropertySet(TabHelper.AddSubTab(childTab, "PropertySets"), byteReader, 0);

            mTerminator = byteReader.read_uint();

            TabHelper.AddStringListTab(childTab, "Header", toStringList(), 0);
        }
    }
}
