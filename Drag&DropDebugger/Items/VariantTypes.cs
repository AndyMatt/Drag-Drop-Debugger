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
                {0x0048, "VT_CLSID"}, 
                {0x1000, "VT_VECTOR" },
                {0x2000, "VT_ARRAY" },
                {0x4000, "VT_BYREF" }
        };

        public static string GetVariantType(uint id)
        {
            string result = "";
            uint _id = (id & 0xFFF);
            uint _array = (id & 0x7000);

            if (TypeVar.ContainsKey(_id))
            {
                result = GetVariantType(TypeVar[_id]);
            }

            if (_array != 0)
            {
                result += "[]";
            }
            return result;
        }

        private static string GetVariantType(string type)
        {          
            switch (type)
            {
                case "VT_NULL": return "";
                case "VT_EMPTY": return "";
                case "VT_I2": return "short / int16";
                case "VT_I4": return "long / int32";
                case "VT_R4": return "float";
                case "VT_R8": return "double";
                case "VT_CY": return "currency";
                case "VT_DATE": return "date";
                case "VT_BSTR": return "string";
                case "VT_DISPATCH": return "IDispatch";
                case "VT_ERROR": return "SCODE";
                case "VT_BOOL": return "bool";
                case "VT_VARIANT": return "pointer";
                case "VT_UNKNOWN": return "IUnknown";
                case "VT_DECIMAL": return "decimal";
                case "VT_I1": return "char";
                case "VT_UI1": return "unsigned char";
                case "VT_UI2": return "unsigned short / unsigned int16";
                case "VT_UI4": return "unsigned long / unsigned int32";
                case "VT_I8": return "longlong / int64";
                case "VT_UI8": return "unsigned longlong / unsigned int64";
                case "VT_INT": return "int";
                case "VT_UINT": return "unsigned int";
                case "VT_VOID": return "void";
                case "VT_HRESULT": return "HRESULT";
                case "VT_PTR": return "pointer";
                case "VT_SAFEARRAY": return "Array";
                case "VT_CARRAY": return "Array";
                case "VT_USERDEFINED": return "user defined type";
                case "VT_LPSTR": return "string";
                case "VT_LPWSTR": return "string";
                case "VT_RECORD": return "user defined type";
                case "VT_INT_PTR": return "int*";
                case "VT_UINT_PTR": return "unsigned int*";
                case "VT_FILETIME": return "FILETIME";
                case "VT_BLOB": return "byte[]";
                case "VT_STREAM": return "stream name";
                case "VT_STORAGE": return "storage name";
                case "VT_BLOB_OBJECT": return "blob object";
                case "VT_STREAMED_OBJECT": return "stream object";
                case "VT_STORED_OBJECT": return "storage object";
                case "VT_CF": return "clipboard format";
                case "VT_CLSID": return "CLSID/GUID";
                case "VT_VERSIONED_STREAM": return "stream + GUID";
                case "VT_BSTR_BLOB": return "Reserved";
                case "VT_VECTOR": return "Array";
                case "VT_ARRAY": return "SafeArray*";
                case "VT_BYREF": return "Void*";
            }

            return type;
        }

        public static string GetTypeName(uint id)
        {
            string result = "";

            uint _id = id;
            if (id >= 0x1000)
            {
                uint ParentType = id & 0x7000;
                if (TypeVar.ContainsKey(ParentType))
                    result = $"{TypeVar[ParentType]}|";

                _id = _id & 0xFFF;
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
                result += $"{TypeVar[_id]} ({GetVariantType(_id)})";
            }
            else
            {
                result += $"UNKNOWN({_id})";
            }

            return result;
        }

        
    }
}
