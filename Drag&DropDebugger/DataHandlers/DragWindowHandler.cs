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
    internal class DragWindowHandler
    {
        public object? mTabReference;
        public DragWindowHandler(TabControl ParentTab, object dropData, string dropType)
        {
            if (dropData is MemoryStream)
            {
                ByteReader byteReader = new ByteReader(((MemoryStream)dropData).ToArray());
                uint hwnd = byteReader.read_uint();
                mTabReference = $"DWORD Handle (0x{hwnd.ToString("X").PadLeft(8, '0')})";
            }
        }
    }
}
