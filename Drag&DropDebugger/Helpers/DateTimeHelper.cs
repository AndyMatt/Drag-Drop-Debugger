using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Drag_DropDebugger
{
    public class WinDateTime
    {
        private readonly System.Runtime.InteropServices.ComTypes.FILETIME mDateTime;

        public WinDateTime(UInt64 fileTime)
        {
            this.mDateTime = new System.Runtime.InteropServices.ComTypes.FILETIME();
            this.mDateTime.dwHighDateTime = (int)(fileTime << 32 >> 32);
            this.mDateTime.dwHighDateTime = (int)(fileTime >> 32);
        }

        public static implicit operator WinDateTime(UInt64 fileTime)
        {
            return new WinDateTime(fileTime);
        }

        public override string ToString()
        {
            ulong high = (ulong)this.mDateTime.dwHighDateTime;
            uint low = (uint)this.mDateTime.dwLowDateTime;
            long fileTime64 = (long)((high << 32) + low);
            if (fileTime64 == 0)
                return "NA";

            try
            {
                DateTime dateTime = DateTime.FromFileTimeUtc(fileTime64);
                DateTime timezone = TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.Local);
                return timezone.ToShortDateString() + " " + timezone.ToLongTimeString();
            }
            catch
            {
                return "NA";
            }
        }
    }
    public class DOSDateTime
    {
        private readonly uint mDOSDateTime;
        private readonly ushort mDate;
        private readonly ushort mTime;

        public DOSDateTime(uint dosDateTime)
        {
            this.mDate = (ushort)(dosDateTime << 16 >> 16);  
            this.mTime = (ushort)(dosDateTime >> 16);
            this.mDOSDateTime = dosDateTime;
        }

        public static implicit operator DOSDateTime(uint dateTime)
        {
            return new DOSDateTime(dateTime);
        }

        public override string ToString() => $"{FromDosDateTime(mDate,mTime)}";


        [DllImport("kernel32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int DosDateTimeToFileTime(ushort dateValue, ushort timeValue, out UInt64 fileTime);

        public static DateTime FromDosDateTime(ushort date, ushort time)
        {
            UInt64 fileTime;
            if (DosDateTimeToFileTime(date, time, out fileTime) == 0)
            {
                throw new Exception($"Date conversion failed: {Marshal.GetLastWin32Error()}");
            }

            return DateTime.FromFileTime(Convert.ToInt64(fileTime));
        }
    }    
}
