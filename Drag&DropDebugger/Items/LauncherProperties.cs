using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    //{0DED77B3-C614-456C-AE5B-285B38D7B01B}
    internal class LauncherProperties
    {
        enum PropertyTypes
        {
            Order = 2, //VT_UI8
            GroupID = 3, //VT_UI8
            AppState = 7, //VT_UI4
            ViewID = 6, //VT_UI4
            TileSize = 8, //VT_UI4
            SplashScreenImagePath = 10, //VT_LPWSTR
            WinStoreCategoryId = 21, //VT_UI4
        }

        const ushort VT_UI8 = 0x015;
        const ushort VT_UI4 = 0x013;
        const ushort VT_LPWSTR = 0x001F;

        class LauncherProperty
        {
            uint mSize;
            PropertyTypes mPropertyType;
            byte _buffer;
            uint mVariableType;
            uint mDataSize;
            object mData;

            public LauncherProperty(TabControl parentTab, ByteReader byteReader)
            {
                mSize = byteReader.read_uint();
                mPropertyType = (PropertyTypes)byteReader.read_uint();
                _buffer = byteReader.read_byte();
                mVariableType = byteReader.read_uint();

                string DataString = "";
                switch (mVariableType)
                {
                    case VT_UI4:
                        mData = byteReader.read_uint();
                        DataString = $"Value: {mData.ToString()} (0x{((uint)mData).ToString("X").PadLeft(8, '0')})";
                        break;

                    case VT_UI8:
                        mData = byteReader.read_uint64();
                        DataString = $"Value: {mData.ToString()} (0x{((UInt64)mData).ToString("X").PadLeft(16, '0')})";
                        break;

                    case VT_LPWSTR:
                        mDataSize = byteReader.read_uint();
                        mData = byteReader.read_UnicodeString();
                        DataString = $"Value {mData}";
                        break;
                }

                string _typeName = Enum.GetName(mPropertyType.GetType(), mPropertyType);

                TabHelper.AddStringListTab(parentTab, _typeName, new string[]
                {

            $"Size: {mSize}",
            $"Type: {_typeName}",
            $"Buffer: {Convert.ToHexString(new byte[]{_buffer})}",
            $"VariableType: {mVariableType}",
            "",
            DataString });
            }
        }

        List<LauncherProperty> mProperties;

        public LauncherProperties(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "LauncherProperties");
            mProperties = new List<LauncherProperty>();

            while (byteReader.scan_uint() != 0x0)
            {
                mProperties.Add(new LauncherProperty(childTab, byteReader));
            }
        }
    }
}
