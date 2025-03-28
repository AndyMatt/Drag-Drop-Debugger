using Drag_DropDebugger.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.DataHandlers
{
    public class DROPEFFECTS
    {
        public enum Flags : uint //DROPEFFECT Constants
        {
            DROPEFFECT_NONE = 0x0, // Drop target cannot accept the data.
            DROPEFFECT_COPY = 0x1, // Drop results in a copy. The original data is untouched by the drag source.
            DROPEFFECT_MOVE = 0x2, // Drag source should remove the data.
            DROPEFFECT_LINK = 0x4, // Drag source should create a link to the original data.
            DROPEFFECT_SCROLL = 0x80000000 //Scrolling is about to start or is currently occurring in the target. This value is used in addition to the other values.
        }
    }

    internal class DropEffectHandler
    {
        private static string FlagstoString(uint _flags)
        {
            string flagStr = "( 0x" + _flags.ToString("X").PadLeft(8, '0') + ")";
            if (_flags == 0x0)
            {
                flagStr += "| DROPEFFECT_NONE";
            }
            else
            {
                foreach (DROPEFFECTS.Flags _flag in Enum.GetValues(typeof(DROPEFFECTS.Flags)))
                {
                    flagStr += ((_flags & (uint)_flag) != 0) ? " | " + Enum.GetName(typeof(DROPEFFECTS.Flags), _flag) : "";
                }
            }
            return flagStr;
        }

        public object? mTabReference;

        public DropEffectHandler(TabControl ParentTab, object dropData, string dropType)
        {
            if (dropData is MemoryStream)
            {
                ByteReader byteReader = new ByteReader(((MemoryStream)dropData).ToArray());
                uint flags = byteReader.read_uint();
                mTabReference = FlagstoString(flags);
            }
        }
    }
}
