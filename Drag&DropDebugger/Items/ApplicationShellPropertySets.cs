using Drag_DropDebugger.Helpers;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    sealed class VARENUM
    {
        private static VARENUM? instance = null;
        private static readonly object padlock = new object();

        public Dictionary<uint, string> TypeVar;

        public static string GetTypeName(uint id)
        {
            string result = "";

            uint _id = id;
            if(id >= 0x1000)
            {
                uint ParentType = id & 0x1100;
                if (Instance.TypeVar.ContainsKey(ParentType))
                    result = $"{Instance.TypeVar[ParentType]} | ";

                _id = _id & 0x11;
            }

            if (Instance.TypeVar.ContainsKey(_id))
            {
                result += Instance.TypeVar[_id];
            }
            else
            {
                result += $"UNKNOWN({_id})";
            }

            return result;
        }

        VARENUM()
        {
            TypeVar = new Dictionary<uint, string>
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
        }

        public static VARENUM Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new VARENUM();
                    }
                    return instance;
                }
            }
        }
    }

    public class AppUserModel_ID //5 (0x05)
    {
        uint mSize;
        uint mPropertyIdentifier;
        byte reserved; //0x0
        uint mPropertyType; //0x1F (VT_LPWSTR)
        uint mPropertySize;
        string mProperty;
        public AppUserModel_ID(TabControl parentTab, ByteReader byteReader)
        {
            ByteReader propertyReader = new ByteReader(byteReader.read_bytes(byteReader.scan_uint()));
            mSize = propertyReader.read_uint();
            mPropertyIdentifier = propertyReader.read_uint();
            reserved = propertyReader.read_byte();
            mPropertyType = propertyReader.read_uint();

            if (mPropertyType != 0x1F)
                throw new Exception($"Invalid Property Set Passed to AppUserModel_ID. Type Code was {mPropertyType}");

            mPropertySize = propertyReader.read_uint();
            mProperty = propertyReader.read_UnicodeString();

            TabControl childTab = TabHelper.AddSubTab(parentTab, "ID");

            TabHelper.AddStringListTab(childTab, "Properties", new string[]
            {
                $"Size: {mSize}",
                $"PropertyIdentifier: {mPropertyIdentifier}",
                $"PropertyType: {VARENUM.GetTypeName(mPropertyType)}",
                $"PropertySize: {mPropertySize}",
                $"Value: {mProperty}",
            });
        }
    }
 
    public class AppUserModel_PackageFullName //21 (0x15)
    {
        uint mSize;
        uint mPropertyIdentifier;
        byte reserved; //0x0
        uint mPropertyType; //0x1F (VT_LPWSTR)
        uint mPropertySize;
        string mProperty;
        public AppUserModel_PackageFullName(TabControl parentTab, ByteReader byteReader)
        {
            ByteReader propertyReader = new ByteReader(byteReader.read_bytes(byteReader.scan_uint()));
            mSize = propertyReader.read_uint();
            mPropertyIdentifier = propertyReader.read_uint();
            reserved = propertyReader.read_byte();
            mPropertyType = propertyReader.read_uint();

            if (mPropertyType != 0x1F)
                throw new Exception($"Invalid Property Set Passed to AppUserModel_PackageFullName. Type Code was {mPropertyType}");

            mPropertySize = propertyReader.read_uint();
            mProperty = propertyReader.read_UnicodeString();

            TabControl childTab = TabHelper.AddSubTab(parentTab, "PackageFullName");

            TabHelper.AddStringListTab(childTab, "Properties", new string[]
            {
                $"Size: {mSize}",
                $"PropertyIdentifier: {mPropertyIdentifier}",
                $"PropertyType: {VARENUM.GetTypeName(mPropertyType)}",
                $"PropertySize: {mPropertySize}",
                $"FamilyName: {mProperty}",
            });
        }
    }

    public class AppUserModel_PackageFamilyName //17 (0x11)
    {

        uint mSize;
        uint mPropertyIdentifier;
        byte reserved; //0x0
        uint mPropertyType; //0x1F (VT_LPWSTR)
        uint mPropertySize;
        string mProperty;
        public AppUserModel_PackageFamilyName(TabControl parentTab, ByteReader byteReader)
        {
            ByteReader propertyReader = new ByteReader(byteReader.read_bytes(byteReader.scan_uint()));
            mSize = propertyReader.read_uint();
            mPropertyIdentifier = propertyReader.read_uint();
            reserved = propertyReader.read_byte();
            mPropertyType = propertyReader.read_uint();

            if (mPropertyType != 0x1F)
                throw new Exception($"Invalid Property Set Passed to AppUserModel_PackageFamilyName. Type Code was {mPropertyType}");

            mPropertySize = propertyReader.read_uint();
            mProperty = propertyReader.read_UnicodeString();

            TabControl childTab = TabHelper.AddSubTab(parentTab, "PackageFamilyName");


            TabHelper.AddStringListTab(childTab, "Properties", new string[]
            {
                $"Size: {mSize}",
                $"PropertyIdentifier: {mPropertyIdentifier}",
                $"PropertyType: {mPropertyType}",
                $"PropertySize: {mPropertySize}",
                $"FamilyName: {mProperty}",
            });
        }
    }

    public class AppUserModel_HostEnvironment // 14 (0x0E)
    {
        uint mSize;
        uint mPropertyIdentifier;
        byte reserved; //0x0
        uint mPropertyType; //0x13 (VT_UI4)
        uint mProperty;
        public AppUserModel_HostEnvironment(TabControl parentTab, ByteReader byteReader)
        {
            ByteReader propertyReader = new ByteReader(byteReader.read_bytes(byteReader.scan_uint()));
            mSize = propertyReader.read_uint();
            mPropertyIdentifier = propertyReader.read_uint();
            reserved = propertyReader.read_byte();
            mPropertyType = propertyReader.read_uint();

            if (mPropertyType != 0x13)
                throw new Exception($"Invalid Property Set Passed to AppUserModel_HostEnvironment. Type Code was {mPropertyType}");

            mProperty = propertyReader.read_uint();

            TabControl childTab = TabHelper.AddSubTab(parentTab, "HostEnvironment");

            TabHelper.AddStringListTab(childTab, "Properties", new string[]
            {
                $"Size: {mSize}",
                $"PropertyIdentifier: {mPropertyIdentifier}",
                $"PropertyType: {VARENUM.GetTypeName(mPropertyType)}",
                $"Property: {mProperty}",
            });
        }
    }

    public class AppUserModel_UnhandledSet
    {
        uint mSize;
        uint mPropertyIdentifier;
        byte reserved; //0x0
        uint mPropertyType;
        uint mPropertySize;
        object mProperty;

        public AppUserModel_UnhandledSet(TabControl parentTab, ByteReader byteReader)
        {
            ByteReader propertyReader = new ByteReader(byteReader.read_bytes(byteReader.scan_uint()));
            mSize = propertyReader.read_uint();
            mPropertyIdentifier = propertyReader.read_uint();
            reserved = propertyReader.read_byte();
            mPropertyType = propertyReader.read_uint();
            if (mPropertyType == 0x1F)
            {
                mPropertySize = propertyReader.read_uint();
                mProperty = propertyReader.read_UnicodeString();

                TabHelper.AddStringListTab(TabHelper.AddSubTab(parentTab, $"UnhandledPropertySet({mPropertyIdentifier})"), $"Properties", new string[]
               {
                $"Size: {mSize}",
                $"PropertyIdentifier: {mPropertyIdentifier}",
                $"PropertyType: {VARENUM.GetTypeName(mPropertyType)}",
                $"PropertySize: {mPropertySize}",
                $"FamilyName: {mProperty}",
               });
            }
            else
            {
                mProperty = propertyReader.read_bytes(mSize - 13);

                TabHelper.AddStringListTab(TabHelper.AddSubTab(parentTab, $"UnhandledPropertySet({mPropertyIdentifier})"), $"Properties", new string[]
                {
                $"Size: {mSize}",
                $"PropertyIdentifier: {mPropertyIdentifier}",
                $"PropertyType: {VARENUM.GetTypeName(mPropertyType)}",
                $"Property: {Convert.ToHexString((byte[])mProperty)}",
                });
            }
        }
    }

    public class ApplicationShellPropertySets
    {
        
        public static void Handle(TabControl parentTab, ByteReader byteReader)
        {
            List<Object> mPropertySets = new List<Object>();

            while (byteReader.scan_uint() != 0)
            {
                uint PropertyTypeIdOffset = 4;
                if (byteReader.scan_uint(PropertyTypeIdOffset) == 17)
                {
                    mPropertySets.Add(new AppUserModel_PackageFamilyName(parentTab, byteReader));
                }
                else if(byteReader.scan_uint(PropertyTypeIdOffset) == 14)
                {
                    mPropertySets.Add(new AppUserModel_HostEnvironment(parentTab, byteReader));
                }
                else if (byteReader.scan_uint(PropertyTypeIdOffset) == 21)
                {
                    mPropertySets.Add(new AppUserModel_PackageFullName(parentTab, byteReader));
                }
                else if (byteReader.scan_uint(PropertyTypeIdOffset) == 5)
                {
                    mPropertySets.Add(new AppUserModel_ID(parentTab, byteReader));
                }
                else
                {
                    mPropertySets.Add(new AppUserModel_UnhandledSet(parentTab, byteReader));
                }
            }
        }
    }
}
