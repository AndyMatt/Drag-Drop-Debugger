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
    internal class SystemProperties 
    {
        enum PropertyTypes
        {
            ThumbnailCacheId = 100, //VT_UI8
            VolumeId = 104, //VT_GUID/VT_CLSID
        }

        const ushort VT_UI8 = 0x015;
        const ushort VT_GUID = 0x0048;

        class SystemProperty
        {
            uint mSize;
            uint mPropertyType;
            byte _buffer;
            uint mVariableType;
            object mData;

            public SystemProperty(TabControl parentTab, ByteReader byteReader)
            {
                mSize = byteReader.read_uint();
                mPropertyType = byteReader.read_uint();
                _buffer = byteReader.read_byte();
                mVariableType = byteReader.read_uint();

                string DataString = "";
                switch (mVariableType)
                {
                    case VT_UI8:
                        mData = byteReader.read_uint64();
                        DataString = $"Value: {mData.ToString()} (0x{((UInt64)mData).ToString("X").PadLeft(16, '0')})";
                        break;

                    case VT_GUID:
                        mData = byteReader.read_guid();
                        DataString = $"GUID: {mData.ToString()}";
                        break;
                }

                string _typeName = Enum.GetName(((PropertyTypes)mPropertyType).GetType(), (PropertyTypes)mPropertyType);

                TabHelper.AddStringListTab(parentTab, _typeName, new string[]
                {

                $"Size: {mSize}",
                $"Type: {_typeName}",
                $"Buffer: {Convert.ToHexString(new byte[]{_buffer})}",
                "",
                DataString });
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
        }
    }

    //{FFAE9DB7-1C8D-43FF-818C-84403AA3732D} PropID = 100
    public class SourcePackageFamilyName
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

            TabHelper.AddStringListTab(parentTab, "SourcePackageFamilyName", new string[]
                {
                $"Size: {mSize}",
                $"TypeID: {mPropertyType}",
                $"Buffer: {Convert.ToHexString(new byte[]{_buffer})}",
                $"VariableType: {mVariableType}",
                $"StringLength: {mStringSize}",
                $"FamilyName:  {mString}" });
        }
    }
}
