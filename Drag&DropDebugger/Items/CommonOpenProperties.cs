using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class CommonOpenProperties : TabbedClass //{B725F130-47EF-101A-A5F1-02608C9EEBAC}
    {
        enum PropertyTypes
        {
            ItemFolderNameDisplay = 0x02, //VT_LPWSTR
            ClassId = 0x03, //VT_CLSID
            FileIndex = 0x08, //VT_UI8
            USN = 0x09, //VT_I8
            ItemNameDisplay = 0x0A, //VT_LPWSTR
            Path = 0x0B, //VT_LPWSTR
            Size = 0x0C, //VT_I8
            FileAttributes = 0x0D, //VT_UI4
            DateModified = 0x0E, //VT_FILETIME
            DateCreated = 0x0F, //VT_FILETIME
            DateAccessed = 0x10, //VT_FILETIME
            AllocSize = 0x12, //VT_I8
            ShortFilename = 0x14 //VT_LPWSTR
        }

        const ushort VT_I4 = 0x004;
        const ushort VT_UI4 = 0x013;
        const ushort VT_I8 = 0x014;
        const ushort VT_UI8 = 0x015;
        const ushort VT_LPWSTR = 0x001F;
        const ushort VT_FILETIME = 0x0040;

        uint mSize;
        uint mPropertyType;
        byte _buffer;
        uint mVariableType;
        uint mDataSize = 0xFFFF;
        object mData;
        public CommonOpenProperties(TabControl parentTab, ByteReader byteReader)
        {
            mSize = byteReader.read_uint();
            mPropertyType = byteReader.read_uint();
            _buffer = byteReader.read_byte();
            mVariableType = byteReader.read_uint();

            switch (mVariableType)
            {
                case VT_I4:
                    mData = byteReader.read_int();
                    break;

                case VT_UI4:
                    mData = byteReader.read_uint();
                    break;

                case VT_I8:
                    mData = byteReader.read_int64();
                    break;

                case VT_UI8:
                    mData = byteReader.read_uint64();
                    break;

                case VT_LPWSTR:
                    mDataSize = byteReader.read_uint();
                    mData = byteReader.read_UnicodeString();
                    break;

                case VT_FILETIME:
                    mData = byteReader.read_uint64();
                    break;

            }

            string _typeName = Enum.GetName(((PropertyTypes)mPropertyType).GetType(), (PropertyTypes)mPropertyType);

            mTabReference = TabHelper.AddDataGridTab(parentTab, "CommonOpenProperties", new Dictionary<string, object>()
            {
                {"Size", $"{mSize} (0x{mSize.ToString("X")})"},
                {"Type", _typeName},
                {"Buffer", Convert.ToHexString(new byte[]{_buffer}) },
                {"DataSize", (mDataSize == 0xFFFF ? "NA" : mDataSize) },
                {"Data", mData.ToString() },
            }, 0);
        }
    }
}
