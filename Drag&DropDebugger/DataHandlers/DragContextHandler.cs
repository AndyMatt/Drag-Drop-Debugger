using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.DataHandlers
{
    internal class DragContextHandler
    {
        //Jim Barry, MVP for Windows SDK (6th September 2002)
        //"Apparently the CFSTR_DRAGCONTEXT format is used internally by the shell
        //drag/drop helpers and is not intended for use by anybody else. It should
        //never have appeared in the headers and is scheduled for removal."
        //As this is the case, class will return 4byte hex segments as a string
        public object? mTabReference;

        public DragContextHandler(TabControl ParentTab, object dropData, string dropType)
        {
            if (dropData is MemoryStream)
            {
                byte[] bytes = ((MemoryStream)dropData).ToArray();
                string hexStr = "";

                for (int i = 0; i < bytes.Length; i++)
                {
                    if(i % 4 == 0)
                    {
                        hexStr += i == 0 ? "0x" : " 0x";
                    }
                    hexStr += bytes[i].ToString("X").PadLeft(2,'0').ToUpper();
                }

                mTabReference = hexStr;
            }
        }
    }
}
