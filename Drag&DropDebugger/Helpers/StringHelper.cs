using Drag_DropDebugger.DataHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Drag_DropDebugger.DataHandlers.FileGroupDescriptor;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters;
using System.Windows.Controls;

namespace Drag_DropDebugger.Helpers
{
    internal class StringHelper
    {
        public static string GetTimeString(System.Runtime.InteropServices.ComTypes.FILETIME fileTime)
        {
            ulong high = (ulong)fileTime.dwHighDateTime;
            uint low = (uint)fileTime.dwLowDateTime;
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

        public static string GetTimeString(uint time)
        {
            try
            {
                DateTime dateTime = DateTime.FromFileTime(time);
                return dateTime.ToShortDateString() + " " + dateTime.ToLongTimeString();
            }
            catch
            {
                return "NA";
            }
        }

        public static string GetFileSizeString(uint fileSize)
        {
            string[] bytePeriods = { " B", " KB", " MB", " GB", " TB" };
            string byteSizeStr = fileSize.ToString("N0") + " bytes";

            if (fileSize < 1024)
                return byteSizeStr;

            int period = 0;
            double fileSizeConv = fileSize;
            while (fileSizeConv > 1024)
            {
                fileSizeConv /= 1024;
                period++;
            }
            fileSizeConv = Math.Round(fileSizeConv, period > 2 ? 2 : 0);

            return $"{fileSizeConv}{bytePeriods[period]} ({byteSizeStr})";
        }

        public static string GetFileSizeString(uint fileSizeHigh, uint fileSizeLow)
        {
            string[] bytePeriods = { " B", " KB", " MB", " GB", " TB" };

            long fileSizeBytes = (fileSizeHigh << 32) + fileSizeLow;
            string byteSizeStr = fileSizeBytes.ToString("N0") + " bytes";

            if (fileSizeBytes < 1024)
                return byteSizeStr;

            int period = 0;
            double fileSizeConv = fileSizeBytes;
            while (fileSizeConv > 1024)
            {
                fileSizeConv /= 1024;
                period++;
            }
            fileSizeConv = Math.Round(fileSizeConv, period > 2 ? 2 : 0);

            return $"{fileSizeConv}{bytePeriods[period]} ({byteSizeStr})";
        }

        public static string[] ClassToString(object classObj)
        {
            List<string> list = new List<string>();
            if (classObj != null)
            {
                FieldInfo[] fieldInfo = classObj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (FieldInfo field in fieldInfo)
                {
                    list.Add($"{field.Name}: {ParsepropertyValue(field, classObj)}");
                }
            }
            return list.ToArray();
        }

        public static Dictionary<string, object> ClassToDictionary(object classObj)
        {
            Dictionary<string, object> list = new Dictionary<string, object>();
            if (classObj != null)
            {
                FieldInfo[] fieldInfo = classObj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (FieldInfo field in fieldInfo)
                {
                    list.Add(field.Name, ParsepropertyValue(field, classObj));
                }
            }
            return list;
        }

        private static object? ParsepropertyValue(FieldInfo field, object classObj)
        {

            object? value = field.GetValue(classObj);
            if (field.Name.ToLower().Contains("flag") || field.Name.ToLower().Contains("attribute"))
            {
                string flagStr = $"{value.GetType().Name} ";

                Array? values = Enum.GetValues(value.GetType());
                foreach (object enumVal in values)
                {
                    flagStr += (((uint)value & (uint)enumVal) != 0) ? Enum.GetName(value.GetType(), enumVal) + " | " : "";
                }
                flagStr += "(0b" + Convert.ToString((uint)value, 2).PadLeft(32, '0') + ")";
                return flagStr;
            }
            if (value == null)
            {
                return value;
            }

            if (value.GetType() == typeof(FILETIME))
            {
                return StringHelper.GetTimeString((FILETIME)value);
            }
            if (value.GetType() == typeof(uint))
            {
                return $"{value.ToString()} (0x{((uint)value).ToString("X")})";
            }
            return value;
        }

        public static string uint2HexString(uint value)
        {
            return $"0x{value.ToString("X").PadLeft(8, '0')}";
        }
    }
}
