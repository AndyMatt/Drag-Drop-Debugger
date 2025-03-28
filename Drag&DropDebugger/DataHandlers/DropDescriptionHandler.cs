using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.DataHandlers
{
    internal class DropDescriptionHandler
    {
        public enum DROPIMAGETYPE : short
        {
            DROPIMAGE_INVALID = -1,
            DROPIMAGE_NONE = 0,
            DROPIMAGE_COPY,
            DROPIMAGE_MOVE,
            DROPIMAGE_LINK,
            DROPIMAGE_LABEL = 6,
            DROPIMAGE_WARNING = 7,
            DROPIMAGE_NOIMAGE = 8
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public sealed class DROPDESCRIPTION
        {
            public DROPIMAGETYPE type;
            [MarshalAs(UnmanagedType.LPWStr, SizeConst = 260)]
            public string szMessage;
            [MarshalAs(UnmanagedType.LPWStr, SizeConst = 260)]
            public string szInsert;
        }

        public object? mTabReference;

        public DropDescriptionHandler(TabControl ParentTab, object dropData, string dropType)
        {
            mTabReference = null;
            if (dropData is MemoryStream)
            {
                byte[] bytes = ((MemoryStream)dropData).ToArray();
                IntPtr dropDescriptionPointer = IntPtr.Zero;
                try
                {
                    dropDescriptionPointer = Marshal.AllocHGlobal(bytes.Length);
                    Marshal.Copy(bytes, 0, dropDescriptionPointer, bytes.Length);

                    object dropDescriptorObject = Marshal.PtrToStructure(dropDescriptionPointer, typeof(DROPDESCRIPTION));
                    DROPDESCRIPTION dropDescriptor = (DROPDESCRIPTION)dropDescriptorObject;

                    mTabReference = TabHelper.AddDataGridTab(ParentTab, "DragSourceHelperFlags", new Dictionary<string, object>
                    {
                        {"Type", dropDescriptor.type },
                        {"Message", dropDescriptor.szMessage },
                        {"Insert", dropDescriptor.szInsert },
                    });
                }
                catch { }
            }

        }
    }
}
