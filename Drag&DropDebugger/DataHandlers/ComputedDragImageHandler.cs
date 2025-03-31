using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace Drag_DropDebugger.DataHandlers
{
    internal class ComputedDragImageHandler
    {
        public Object? mTabReference;

        public ComputedDragImageHandler(TabControl ParentTab, object dropData, string dropType)
        {
            if (dropData is MemoryStream)
            {
                ByteReader byteReader = new ByteReader(((MemoryStream)dropData).ToArray());
                uint dwordValue = byteReader.read_uint();
                mTabReference = StringHelper.uint2HexString(dwordValue);
            }
        }
    }
}