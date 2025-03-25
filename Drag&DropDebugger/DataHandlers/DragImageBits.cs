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

namespace Drag_DropDebugger.DataHandlers
{
    public class DragImageBits
    {
        public object mTabReference;

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

            TabHelper.AddStringListTab(childTab, "Properties", new string[]
            {
                $"Width: {mWidth}",
                $"Height: {mHeight}",
                $"OffsetX: {mX}",
                $"OffsetY: {mY}",
                $"BitmapHandle: 0x{mBitmapHandle.ToString("X")}",
                $"ColorRef",
                $"    Alpha: {(mColorRef & 0xFF000000)>>24}",
                $"    Red: {(mColorRef & 0x00FF0000)>>16}",
                $"    Green: {(mColorRef & 0x0000FF00)>>8}",
                $"    Blue: {(mColorRef & 0x000000FF)}",
            });

            TabHelper.AddRawDataTab(childTab, rawData, "Raw");

            BitmapSource bmpSource = FromArray(byteReader.read_remainingbytes(), mWidth, mHeight, 4);
            TabHelper.AddBitmapTab(childTab, "Preview", bmpSource);
            mTabReference = childTab;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private static BitmapImage? Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapImage? retval = null;

            try
            {
                retval = (BitmapImage)Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

        public static BitmapSource FromNativePointer(IntPtr pData, int w, int h, int ch)
        {
            System.Windows.Media.PixelFormat format = PixelFormats.Default;

            if (ch == 1) format = PixelFormats.Gray8; //grey scale image 0-255
            if (ch == 3) format = PixelFormats.Bgr24; //RGB
            if (ch == 4) format = PixelFormats.Bgra32; //RGB + alpha


            WriteableBitmap wbm = new WriteableBitmap(w, h, 96, 96, format, null);
            CopyMemory(wbm.BackBuffer, pData, (uint)(w * h * ch));

            wbm.Lock();
            wbm.AddDirtyRect(new Int32Rect(0, 0, wbm.PixelWidth, wbm.PixelHeight));
            wbm.Unlock();

            return wbm;
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
