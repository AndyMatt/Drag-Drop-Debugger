using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    //{20D04FE0-3AEA-1069-A2D8-08002B30309D}
    public class MyComputerShellItem
    {
        RootFolderShellItem? mRootFolderShellItem;
        public MyComputerShellItem(TabControl parentTab, ByteReader byteReader)
        {
           TabHelper.AddStringListTab(parentTab, "Summary", new string[]
           {
                "CLSID: {20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                "Label: This PC"
           });

            if(byteReader.scan_ushort() != 0x0)
            {
                mRootFolderShellItem = new RootFolderShellItem(parentTab, byteReader);
            }
        }
    }
}
