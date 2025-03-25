using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    //{59031A47-3F72-44A7-89C5-5595FE6B30EE}
    public class UserFolderShellItem
    {
        RootFolderExtensionBlock? mExtensionBlock;

        const uint ExtensionSignitureOffset = 4;
        const uint KnownFolderIDOffset = 6;
        public UserFolderShellItem(TabControl parentTab, Guid guid, ByteReader byteReader)
        {
            string SpecialPath = NativeMethods.GetPathFromGUID(guid);

            TabHelper.AddStringListTab(parentTab, "GUID", new string[]{
                $"GUID: {{{guid.ToString()}}}",
                $"Path: {SpecialPath}"}, 0);

            if (byteReader.scan_uint(ExtensionSignitureOffset) == 0xBEEF0026)
            {
                mExtensionBlock = new RootFolderExtensionBlock(parentTab, byteReader);

                if (byteReader.scan_uint(KnownFolderIDOffset) == 0x23FEBBEE)
                {
                    UserPropertyViewItem propertyView = new UserPropertyViewItem(parentTab, byteReader);
                }
            }

            while(byteReader.scan_ushort() != 0x0)
            {
                new FileEntryShellItem(parentTab, byteReader);
            }
        }
    }
}
