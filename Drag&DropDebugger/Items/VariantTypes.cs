using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Drag_DropDebugger.Items
{
    internal class VariantTypes
    {
        static Dictionary<uint, string> TypeVar = new Dictionary<uint, string>
        {
                {0x0000, "VT_EMPTY" },
                {0x0001, "VT_NULL" },
                {0x0002, "VT_I2" },
                {0x0003, "VT_I4" },
                {0x0004, "VT_R4" },
                {0x0005, "VT_R8" },
                {0x0006, "VT_CY" },
                {0x0007, "VT_DATE" },
                {0x0008, "VT_BSTR" },
                {0x0009, "VT_DISPATCH" },
                {0x000A, "VT_ERROR" },
                {0x000B, "VT_BOOL" },
                {0x000C, "VT_VARIANT" },
                {0x000D, "VT_UNKNOWN" },
                {0x000E, "VT_DECIMAL" },
                {0x0010, "VT_I1" },
                {0x0011, "VT_UI1" },
                {0x0012, "VT_UI2" },
                {0x0013, "VT_UI4" },
                {0x0014, "VT_I8" },
                {0x0015, "VT_UI8" },
                {0x0016, "VT_INT" },
                {0x0017, "VT_UINT" },
                {0x0018, "VT_VOID" },
                {0x0019, "VT_HRESULT" },
                {0x001A, "VT_PTR" },
                {0x001B, "VT_SAFEARRAY" },
                {0x001C, "VT_CARRAY" },
                {0x001D, "VT_USERDEFINED" },
                {0x001E, "VT_LPSTR" },
                {0x001F, "VT_LPWSTR" },
                {0x0024, "VT_RECORD" },
                {0x0025, "VT_INT_PTR" },
                {0x0026, "VT_UINT_PTR" },
                {0x0040, "VT_FILETIME" },
                {0x0042, "VT_STREAM" },
                {0x0047, "VT_CF"},
                {0x0048, "VT_GUID"}, //UNVERIFIED 
                {0x1000, "VT_ARRAY" },
                {0x2000, "VT_ARRAY" },
                {0x4000, "VT_BYREF" }
        };


        public static string GetTypeName(uint id)
        {
            string result = "";

            uint _id = id;
            if (id >= 0x1000)
            {
                uint ParentType = id & 0x1100;
                if (TypeVar.ContainsKey(ParentType))
                    result = $"{TypeVar[ParentType]}|";

                _id = _id & 0x11;
            }

            if (TypeVar.ContainsKey(_id))
            {
                result += $"{TypeVar[_id]}";
            }
            else
            {
                result += $"UNKNOWN({_id})";
            }

            return result;
        }
        public static string GetTypeString(uint id)
        {
            string result = "";

            uint _id = id;
            if (id >= 0x1000)
            {
                uint ParentType = id & 0x1100;
                if (TypeVar.ContainsKey(ParentType))
                    result = $"{TypeVar[ParentType]} | ";

                _id = _id & 0x11;
            }

            if (TypeVar.ContainsKey(_id))
            {
                result += $"{TypeVar[_id]} (!VARIABLETYPE)";
            }
            else
            {
                result += $"UNKNOWN({_id})";
            }

            return result;
        }

        
    }
}
