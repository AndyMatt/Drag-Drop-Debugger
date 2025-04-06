using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class RootFolderShellItem : TabbedClass
    {
        /* mflagIdentifier
        The volume shell item can be identified by a value of 0x20 after applying a bitmask of 0x70. 
        The remaining bits in the class type indicator are presumed to be a sub-type or flags.

        Value	Description
        0x01    Has name
        0x02    Unknown (0x23 C:, 0x2f C: or D:, 0x2a J:)
        0x04    Unknown (0x23 C:, 0x25 D:)
        0x08    Is removable media (0x23 C:, 0x29 A:, 0x2a J:)
         */
        ushort mSize;
        byte mflagIdentifier;
        string mLabel;
        List<FileEntryShellItem> mFileEntry;
        public RootFolderShellItem(TabControl parentTab, ByteReader byteReader)
        {
            mSize = byteReader.read_ushort();
            mflagIdentifier = byteReader.read_byte();
            int StringSize = mSize - sizeof(ushort) - sizeof(byte);
            mLabel = byteReader.read_AsciiString((uint)StringSize);

            Dictionary<string, object> properties = new Dictionary<string, object>()
            {
                {"Size", $"{mSize} (0x{mSize.ToString("X")}"},
                {"Flag Identifier", mflagIdentifier},
                {"Label", mLabel}
            };

            mFileEntry = new List<FileEntryShellItem>();
            while (!byteReader.End() && byteReader.scan_ushort() != 0x0)
            {
                ushort identifier = byteReader.scan_ushort(2);

                if ((identifier & 0x70) == 0x30)
                {
                    FileEntryShellItem fileEntry = new FileEntryShellItem(parentTab, byteReader);
                    properties.Add(fileEntry.GetPropertyString(), fileEntry.mTabReference);
                    mFileEntry.Add(fileEntry);
                }
            }

            mTabReference = TabHelper.AddDataGridTab(parentTab, "RootFolderShellItem", properties, 0);
        }
    }
}
