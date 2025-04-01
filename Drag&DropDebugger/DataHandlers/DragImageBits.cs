using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing;
using WpfHexaEditor.Core;
using Drag_DropDebugger.UI;

namespace Drag_DropDebugger.DataHandlers
{
    public class DragImageBits : TabbedClass
    {
        public DragImageBits(TabControl parentTab, MemoryStream dropData, string dataType)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "DragImageBits");

            ByteReader byteReader = new ByteReader(dropData.ToArray());
            byte[] rawData = byteReader.read_remainingbytes();

            int mWidth = byteReader.read_int();
            int mHeight = byteReader.read_int();
            int mX = byteReader.read_int();
            int mY = byteReader.read_int();
            int mBitmapHandle = byteReader.read_int();
            int mColorRef = byteReader.read_int();

            StackedDataTab stackedDataTab = new StackedDataTab("Header");

            stackedDataTab.AddDataGrid("Properties", new Dictionary<string, object>()
            {
                { "Width", mWidth },
                { "Height", mHeight },
                { "OffsetX", mX },
                { "OffsetY", mY },
                { "BitmapHandle", $"0x{mBitmapHandle.ToString("X").PadLeft(8,'0')}"},
            });

            stackedDataTab.AddDataGrid("ColorRef", new Dictionary<string, object>()
            {
                { "Alpha", (mColorRef & 0xFF000000)>>24 },
                { "Red", (mColorRef & 0x00FF0000)>>16 },
                { "Green", (mColorRef & 0x0000FF00)>>8 },
                { "Blue", (mColorRef & 0x000000FF) },
            });

            BitmapSource bmpSource = FromArray(byteReader.read_remainingbytes(), mWidth, mHeight, 4);
            stackedDataTab.AddPreviewPanel("Preview", bmpSource);

            TabHelper.AddStackTab(childTab, stackedDataTab);
            TabHelper.AddRawDataTab(childTab, rawData, "Raw");

            mTabReference = childTab;
        }

        public static BitmapSource FromArray(byte[] data, int w, int h, int ch)
        {
            System.Windows.Media.PixelFormat format = PixelFormats.Default;

            if (ch == 1) format = PixelFormats.Gray8; //grey scale image 0-255
            if (ch == 3) format = PixelFormats.Bgr24; //RGB
            if (ch == 4) format = PixelFormats.Bgra32; //RGB + alpha


            WriteableBitmap wbm = new WriteableBitmap(w, h, 96, 96, format, null);
            for(int i = 0; i < h; i++)
            {
                wbm.WritePixels(new Int32Rect(0, (h-1)-i, w, 1), data, ch * w, 0, i);
            }

            return wbm;
        }
    }
}
