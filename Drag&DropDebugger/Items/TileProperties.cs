using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class TileProperties //{86D40B4D-9069-443C-819A-2A54090DCCEC}
    {
        enum PropertyTypes
        {
            SmallLogoPath = 2, //VT_LPWSTR
            Background = 4, //VT_UI4
            Foreground = 5, //VT_UI4
            LongDisplayName = 11, //VT_LPWSTR
            Square150x150LogoPath = 12, //VT_LPWSTR
            Wide310x150LogoPath = 13, //VT_LPWSTR
            Flags = 14, //VT_UI4
            BadgeLogoPath = 15, //VT_LPWSTR
            SuiteDisplayName = 16, //VT_LPWSTR
            SuiteSortName = 17, //VT_LPWSTR
            DisplayNameLanguage = 18, //VT_LPWSTR
            Square310x310LogoPath = 19, //VT_LPWSTR
            Square70x70LogoPath = 20, //VT_LPWSTR
            FencePost = 21, //VT_UI4
            InstallProgress = 22, //VT_UI4
            EncodedTargetPath = 23, //VT_LPWSTR
            HoloContent = 24, //VT_LPWSTR
            HoloBoundingBox = 25, //VT_LPWSTR
        }

        const ushort VT_UI4 = 0x013;
        const ushort VT_LPWSTR = 0x001F;

        class TileProperty
        {
            uint mSize;
            uint mPropertyType;
            byte _buffer;
            uint mVariableType;
            uint mDataSize = 0xFFFF;
            object mData;

            public TileProperty(TabControl parentTab, ByteReader byteReader)
            {
                ByteReader propertyBytes = new ByteReader(byteReader.read_bytes(byteReader.read_uint(false)));
                
                mSize = propertyBytes.read_uint();
                mPropertyType = propertyBytes.read_uint();
                _buffer = propertyBytes.read_byte();
                mVariableType = propertyBytes.read_uint();

                string DataString = "";
                switch (mVariableType)
                {
                    case VT_UI4:
                        mData = propertyBytes.read_uint();
                        DataString = $"Data: {mData.ToString()} (0x{((uint)mData).ToString("X").PadLeft(8,'0')})";
                        break;

                    case VT_LPWSTR:
                        mDataSize = propertyBytes.read_uint();
                        mData = propertyBytes.read_UnicodeString();
                        DataString = $"Data: {mData.ToString()}";
                        break;
                }

                string _typeName = Enum.GetName(((PropertyTypes)mPropertyType).GetType(), (PropertyTypes)mPropertyType);

                TabHelper.AddStringListTab(parentTab, _typeName, new string[]
                {

                $"Size: {mSize}",
                $"Type: {_typeName}",
                $"Buffer: {Convert.ToHexString(new byte[]{_buffer})}",
                "",
                $"{(mDataSize == 0xFFFF ? "" : $"DataSize: {mDataSize}")}",
                DataString });
            }
        }

        List<TileProperty> mProperties;

        public TileProperties(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "TileProperties");
            mProperties = new List<TileProperty>();

            while (byteReader.scan_uint() != 0x0)
            {
                mProperties.Add(new TileProperty(childTab, byteReader));
            }
        }
       
    }
}