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
    public class BooleanHandler
    {
        public object? mTabReference;
        public BooleanHandler(TabControl ParentTab, object dropData, string dropType)
        {
            if (dropData is MemoryStream)
            {
                ByteReader byteReader = new ByteReader(((MemoryStream)dropData).ToArray());
                uint flags = byteReader.read_uint();
                mTabReference = (flags == 0 ? $"False" : "True") + $" (0x{ flags.ToString("X").PadLeft(8, '0')})";
            }
        }
    }
}
