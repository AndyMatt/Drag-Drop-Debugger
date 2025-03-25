using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    //{446D16B1-8DAD-4870-A748-402EA43D788C}
    internal class SystemProperties : TabbedClass
    {
        enum PropertyTypes
        {
            ThumbnailCacheId = 100, //VT_UI8
            VolumeId = 104, //VT_GUID/VT_CLSID
        }

        const ushort VT_UI8 = 0x015;
        const ushort VT_CLSID = 0x0048;

        class SystemProperty : TabbedClass
        {
            uint mSize;
            uint mPropertyType;
            byte _buffer;
            uint mVariableType;
            object mData;
            string mVariableName;


            public SystemProperty(TabControl parentTab, ByteReader byteReader)
            {
                mSize = byteReader.read_uint();
                mPropertyType = byteReader.read_uint();
                _buffer = byteReader.read_byte();
                mVariableType = byteReader.read_uint();

                object _Data = "";
                switch (mVariableType)
                {
                    case VT_UI8:
                        mData = byteReader.read_uint64();
                        mVariableName = "Value";
                        _Data = $"{mData.ToString()} (0x{((UInt64)mData).ToString("X").PadLeft(16, '0')})";
                        break;

                    case VT_CLSID:
                        mData = byteReader.read_guid();
                        mVariableName = "GUID";
                        _Data = mData;
                        break;
                }

                string _typeName = Enum.GetName(((PropertyTypes)mPropertyType).GetType(), (PropertyTypes)mPropertyType);

                mTabReference = TabHelper.AddDataGridTab(parentTab, _typeName, new Dictionary<string, object>()
                {
                    {"Size", $"{mSize} (0x{mSize.ToString("X")})"},
                    {"Type",_typeName},
                    {"Buffer",Convert.ToHexString(new byte[]{_buffer})},
                    {mVariableName, _Data},
                }, 0);
            }
        }

        List<SystemProperty> mProperties;

        public SystemProperties(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "SystemProperties");
            mProperties = new List<SystemProperty>();

            while (byteReader.scan_uint() != 0x0)
            {
                mProperties.Add(new SystemProperty(childTab, byteReader));
            }
            mTabReference = childTab;
        }
    }

    //{FFAE9DB7-1C8D-43FF-818C-84403AA3732D} PropID = 100
    public class SourcePackageFamilyName : TabbedClass
    {
        uint mSize;
        uint mPropertyType;
        byte _buffer;
        uint mVariableType;
        uint mStringSize;
        string mString;

        public SourcePackageFamilyName(TabControl parentTab, ByteReader byteReader)
        {
            if(byteReader.scan_uint(4) != 100)
            {
                byteReader.read_bytes(byteReader.scan_uint());
                return;
            }

            mSize = byteReader.read_uint();
            mPropertyType = byteReader.read_uint();            
            _buffer = byteReader.read_byte();
            mVariableType = byteReader.read_uint();
            mStringSize = byteReader.read_uint();
            mString = byteReader.read_UnicodeString();

            mTabReference = TabHelper.AddDataGridTab(parentTab, "SourcePackageFamilyName", new Dictionary<string, object>()
            {
                {"Size", $"{mSize} (0x{mSize.ToString("X")})"},
                {"TypeID", mPropertyType},
                {"Buffer", Convert.ToHexString(new byte[]{_buffer})},
                {"VariableType", mVariableType },
                {"StringLength", mStringSize },
                {"FamilyName", mString },
            });
        }
    }
}
