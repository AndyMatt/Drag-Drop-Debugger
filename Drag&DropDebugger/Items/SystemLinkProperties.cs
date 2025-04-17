using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{

    internal class SystemLinkProperties : TabbedClass //436F2667-14E2-4FEB-B30A-146C53B5B674
    {
        enum PropertyTypes
        {
            Arguments = 100, //VT_LPWSTR
        }

        const ushort VT_LPWSTR = 0x001F;

        class SystemLinkProperty : TabbedClass
        {
            uint mSize;
            uint mPropertyType;
            byte _buffer;
            uint mVariableType;
            uint mDataSize = 0xFFFF;
            object mData;

            public SystemLinkProperty(TabControl parentTab, ByteReader byteReader)
            {
                ByteReader propertyBytes = new ByteReader(byteReader.read_bytes(byteReader.read_uint(false)));

                mSize = propertyBytes.read_uint();
                mPropertyType = propertyBytes.read_uint();
                _buffer = propertyBytes.read_byte();
                mVariableType = propertyBytes.read_uint();

                object _Data = "";
                switch (mVariableType)
                {
                    case VT_LPWSTR:
                        mDataSize = propertyBytes.read_uint();
                        mData = propertyBytes.read_UnicodeString();
                        _Data = mData;
                        break;
                }

                string _typeName = Enum.GetName(((PropertyTypes)mPropertyType).GetType(), (PropertyTypes)mPropertyType);


                mTabReference = TabHelper.AddDataGridTab(parentTab, _typeName, new Dictionary<string, object>()
                {
                    {"Size", $"{mSize} (0x{mSize.ToString("X")})" },
                    {"Type", _typeName },
                    {"Buffer", Convert.ToHexString(new byte[]{_buffer}) },
                    {"mDataSize", mDataSize == 0xFFFF ? "NA" : mDataSize },
                    {"Data", _Data },
                }, 0);
            }
        }

        List<SystemLinkProperty> mProperties;

        public SystemLinkProperties(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "SystemLinkProperties");
            mProperties = new List<SystemLinkProperty>();

            while (byteReader.scan_uint() != 0x0)
            {
                mProperties.Add(new SystemLinkProperty(childTab, byteReader));
            }

            mTabReference = childTab;
        }

    }
}
