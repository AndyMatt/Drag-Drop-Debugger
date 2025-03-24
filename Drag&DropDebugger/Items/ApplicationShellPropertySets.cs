using Drag_DropDebugger.Helpers;
using Microsoft.VisualBasic;
using System.Windows.Controls;
using WpfHexaEditor.Core;

namespace Drag_DropDebugger.Items
{
    sealed class PropertyNames
    {
        public static string GetPropertySetName(uint id)
        {
            if (TypeNames.ContainsKey(id))
                return TypeNames[id];

            return $"UnknownPropertySet({id})";
        }

        static Dictionary<uint, string> TypeNames = new Dictionary<uint, string>
        {
            {20, "ActivationContext"},
            {10, "BestShortcut"},
            {29, "DestListLogoUri"},
            {28, "DestListProvidedDescription"},
            {30, "DestListProvidedGroupName"},
            {27, "DestListProvidedTitle"},
            {23, "ExcludedFromLauncher"},
            { 8, "ExcludeFromShowInNewInstall"},
            {14, "HostEnvironment"},
            { 5, "ID"},
            {18, "InstalledBy"},
            { 7, "IsDestListLink"},
            { 6, "IsDestListSeparator"},
            {11, "IsDualMode"},
            {17, "PackageFamilyName"},
            {21, "PackageFullName"},
            {15, "PackageInstallPath"},
            {22, "PackageRelativeApplicationID"},
            {19, "ParentID"},
            {16, "RecordState"},
            { 2, "RelaunchCommand"},
            { 4, "RelaunchDisplayNameResource"},
            { 3, "RelaunchIconResource"},
            {13, "Relevance"},
            {25, "RunFlags"},
            {12, "StartPinOption"},
            {32, "TileUniqueId"},
            {26, "ToastActivatorCLSID"},
            {31, "VisualElementsManifestHintPath"},
            { 9, "PreventPinning"}
        };
    }

    class VT_LPWSTR
    {
        uint mStrLength;
        string mValue;
        public VT_LPWSTR(TabControl parentTab, ByteReader byteReader)
        {
            mStrLength = byteReader.read_uint();
            mValue = byteReader.read_UnicodeString();
        }
        public override string ToString()
        {
            return mValue;
        }
    }

    class VT_GUID
    {
        Guid mValue;
        public VT_GUID(TabControl parentTab, ByteReader byteReader)
        {
            mValue = byteReader.read_guid();
        }
        public override string ToString()
        {
            return mValue.ToString();
        }
    }

    class BESTSHORTCUT
    {
        uint mSize;
        byte[] mValue;
        Object? mObj;
        public BESTSHORTCUT(TabControl parentTab, ByteReader byteReader)
        {
            mSize = byteReader.read_uint();
            mValue = byteReader.read_bytes(mSize);
            mObj = ShellItemHandler.Handle(parentTab, new ByteReader(mValue));
        }

        public override string ToString()
        {
            return $"{mObj.GetType().Name}";
        }
    }

    class VT_DYNAMIC
    {
        object mValue;
        public VT_DYNAMIC(TabControl parentTab, ByteReader byteReader, uint type)
        {
            switch (VariantTypes.GetTypeName(type))
            {
                case "VT_BSTR":
                    mValue = byteReader.read_AsciiString();
                    break;

                case "VT_BOOL":
                    mValue = byteReader.read_bool();
                    break;

                case "VT_UI4":
                case "VT_UINT":
                    mValue = byteReader.read_uint();
                    break;

                case "VT_GUID":
                    mValue = byteReader.read_guid();
                    break;

                case "VT_LPWSTR":
                    uint mStrLength = byteReader.read_uint();
                    mValue = byteReader.read_UnicodeString();
                    break;

                default:
                    mValue = byteReader.read_uint();
                    break;
            }

        }

        public override string ToString()
        {
            if (mValue != null)
                return mValue.ToString();

            return "ERROR";
        }
    }

    class VT_GENERIC
    {
        byte[] mValue;

        public VT_GENERIC(TabControl parentTab, ByteReader byteReader, uint dataSize)
        {
            mValue = byteReader.read_bytes(dataSize);
        }

        public override string ToString()
        {
            return Convert.ToHexString((byte[])mValue);
        }

    }

    public class AppUserModel_Generic
    {
        static Dictionary<uint, Type> PropertyIDLookup = new Dictionary<uint, Type>
        {
            {0x0A, typeof(BESTSHORTCUT)},
        };

        uint mSize;
        uint mPropertyIdentifier;
        byte reserved; //0x0
        uint mPropertyType; //0x1F (VT_LPWSTR)
        Object? mProperty;

        public AppUserModel_Generic(TabControl parentTab, ByteReader byteReader)
        {
            ByteReader propertyReader = new ByteReader(byteReader.read_bytes(byteReader.scan_uint()));
            mSize = propertyReader.read_uint();
            mPropertyIdentifier = propertyReader.read_uint();
            reserved = propertyReader.read_byte();
            mPropertyType = propertyReader.read_uint();

            string _PropertyName = PropertyNames.GetPropertySetName(mPropertyIdentifier);

            TabControl childTab = TabHelper.AddSubTab(parentTab, _PropertyName);
            
            if(PropertyIDLookup.ContainsKey(mPropertyIdentifier))
            {
                Type ClassType = PropertyIDLookup[mPropertyIdentifier];
                mProperty = Activator.CreateInstance(ClassType, childTab, propertyReader);
            }
            else
            {
                mProperty = new VT_DYNAMIC(childTab, propertyReader, mPropertyType);
                //mProperty = new VT_GENERIC(childTab, propertyReader, mSize - 15);      
            }

            TabHelper.AddStringListTab(childTab, "Properties", new string[]
            {
                $"Size: {mSize}",
                $"PropertyIdentifier: {mPropertyIdentifier}",
                $"PropertyType: {VariantTypes.GetTypeString(mPropertyType)}",
                $"Value: {mProperty.ToString()}",
            },0);
        }
    }

    public class ApplicationShellPropertySets
    {
        List<Object>? mPropertySets;
        public ApplicationShellPropertySets(TabControl parentTab, ByteReader byteReader)
        {
            mPropertySets = new List<Object>();

            while (byteReader.scan_uint() != 0)
            {
                mPropertySets.Add(new AppUserModel_Generic(parentTab, byteReader));
            }
        }
    }
}
