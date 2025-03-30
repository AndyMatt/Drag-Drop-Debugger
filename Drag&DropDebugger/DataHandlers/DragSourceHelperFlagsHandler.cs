using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.DataHandlers
{
    internal class DragSourceHelperFlagsHandler
    {
        public object? mTabReference;

        public DragSourceHelperFlagsHandler(TabControl ParentTab, object dropData, string dropType)
        {
            if (dropData is MemoryStream)
            {
                ByteReader byteReader = new ByteReader(((MemoryStream)dropData).ToArray());
                uint flag = byteReader.read_uint();

                if (flag == 1)
                {
                    mTabReference = "DSH_ALLOWDROPDESCRIPTIONTEXT";
                }
                else if (flag == 0)
                {
                    mTabReference = "DSH_EMPTY";
                }
                else
                {
                    mTabReference = $"DSH_UNKNOWN (0x{flag.ToString("X").PadLeft(8,'0')})";
                }
            }
            else
            { 
            mTabReference = "ERROR";
                }
        }
    }
}
