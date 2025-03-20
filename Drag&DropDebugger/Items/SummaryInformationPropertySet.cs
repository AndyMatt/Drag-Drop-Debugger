using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class SummaryInformationPropertySet //{F29F85E0-4FF9-1068-AB91-08002B27B3D9}
    {        
        enum PropertyTypes
        {
            Title = 0x02, //VT_LPSTR
            Subject = 0x03, //VT_LPSTR
            Author = 0x04, //VT_LPSTR
            Keywords = 0x05, //VT_LPSTR
            Comments = 0x06, //VT_LPSTR
            Template = 0x07, //VT_LPSTR
            LastSavedBy = 0x08, //VT_LPSTR
            RevisionNumber = 0x09, //VT_LPSTR
            TotalEditingTime = 0x0A, //VT_FILETIME (UTC)
            LastPrinted = 0x0B, //VT_FILETIME (UTC)
            CreateTime = 0x0C, //VT_FILETIME (UTC)
            LastSavedTime = 0x0D, //VT_FILETIME (UTC)
            NumberofPages = 0x0E, //VT_I4
            NumberofWords = 0x0F, //VT_I4
            NumberofCharacters = 0x10, //VT_I4
            Thumbnail = 0x11, //VT_CF
            NameofCreatingApplication = 0x12, //VT_LPSTR
            Security = 0x13 //VT_I4
        }

        const ushort VT_I4 = 0x004;
        const ushort VT_LPSTR = 0x001E;
        const ushort VT_LPWSTR = 0x001F;
        const ushort VT_FILETIME = 0x0040;
        const ushort VT_CF = 0x047;

        uint mSize;
        uint mPropertyType;
        byte _buffer;
        uint mVariableType;
        uint mDataSize = 0xFFFF;
        object mData;

        public SummaryInformationPropertySet(TabControl parentTab, ByteReader byteReader)
        {
            mSize = byteReader.read_uint();
            mPropertyType = byteReader.read_uint();
            _buffer = byteReader.read_byte();
            mVariableType = byteReader.read_uint();

            switch (mVariableType)
            {
                case VT_I4:
                    mData = byteReader.read_uint();
                    break;

                case VT_LPSTR:
                    mDataSize = byteReader.read_uint();
                    mData = byteReader.read_AsciiString();
                    break;

                case VT_LPWSTR:
                    mDataSize = byteReader.read_uint();
                    mData = byteReader.read_UnicodeString();
                    break;

                case VT_FILETIME:
                    mData = byteReader.read_uint64();
                    break;

                case VT_CF:
                    mDataSize = byteReader.read_uint();
                    mData = byteReader.read_bytes(mDataSize);
                    break;
            }

            string _typeName = Enum.GetName(((PropertyTypes)mPropertyType).GetType(), (PropertyTypes)mPropertyType);

            TabHelper.AddStringListTab(parentTab, "SummaryInformationPropertySet", new string[]
            {
                
                $"Size: {mSize}",
                $"Type: {_typeName}",
                $"Buffer: {Convert.ToHexString(new byte[]{_buffer})}",
                "",
                $"{(mDataSize == 0xFFFF ? "" : $"DataSize: {mDataSize}")}",
                $"Data: {mData.ToString()}" });
        }
    }
}
