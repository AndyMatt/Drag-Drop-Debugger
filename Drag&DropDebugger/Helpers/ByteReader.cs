using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Drag_DropDebugger.Helpers
{
    public class ByteReader
    {
        byte[] bytes;
        uint _iterator;

        const int GUID_Size = 16;

        public int GetGUIDSize() { return GUID_Size; }

        public ByteReader(byte[] bytes)
        {
            this.bytes = bytes;
            _iterator = 0;
        }

        public bool End()
        {
            return !(_iterator < bytes.Length);
        }

        public void RollBack(uint offset)
        {
            if (_iterator > offset)
                _iterator -= offset;
        }

        public void SetOffset(uint offset)
        {
            _iterator = offset;
        }

        public uint GetOffset()
        {
            return _iterator;
        }

        public void SkipTerminators()
        {
            while (bytes[_iterator] == 0)
            {
                _iterator++;
            }
        }

        public ulong read_uint64(bool advance = true)
        {
            uint hi = read_uint();
            uint low = read_uint();

            return (ulong)low << 32 | hi;
        }

        public long read_int64(bool advance = true)
        {
            int hi = read_int();
            int low = read_int();

            return (long)low << 32 | hi;
        }

        public uint read_uint(bool advance = true)
        {
            uint result = (uint)bytes[_iterator + 3] << 24 | (uint)bytes[_iterator + 2] << 16 | (uint)bytes[_iterator + 1] << 8 | bytes[_iterator];
            if (advance)
                _iterator += sizeof(uint);
            return result;
        }

        public int read_int(bool advance = true)
        {
            int result = bytes[_iterator + 3] << 24 | bytes[_iterator + 2] << 16 | bytes[_iterator + 1] << 8 | bytes[_iterator];
            if (advance)
                _iterator += sizeof(uint);
            return result;
        }

        public bool read_bool(bool advance = true)
        {
            uint result = (uint)bytes[_iterator + 3] << 24 | (uint)bytes[_iterator + 2] << 16 | (uint)bytes[_iterator + 1] << 8 | bytes[_iterator];
            if (advance)
                _iterator += sizeof(uint);
            return result == 0 ? false : true;
        }


        public uint scan_uint(uint offset = 0)
        {
            uint _of = offset + _iterator;
            uint result = (uint)bytes[_of + 3] << 24 | (uint)bytes[_of + 2] << 16 | (uint)bytes[_of + 1] << 8 | bytes[_of];
            return result;
        }

        public ushort read_ushort(bool advance = true)
        {
            ushort result = (ushort)(bytes[_iterator + 1] << 8 | bytes[_iterator]);
            if (advance)
                _iterator += sizeof(ushort);
            return result;
        }
        public ushort scan_ushort(uint offset = 0)
        {
            uint _of = offset + _iterator;
            ushort result = (ushort)(bytes[_of + 1] << 8 | bytes[_of]);
            return result;
        }

        public byte read_byte(bool advance = true)
        {
            if (advance)
                return bytes[_iterator++];

            return bytes[_iterator];
        }

        public byte scan_byte(uint offset)
        {
           return bytes[_iterator+offset];
        }

        public byte[] copy_allbytes()
        {
            byte[] result = new byte[bytes.Length];
            Array.Copy(bytes, 0, result, 0, bytes.Length);
            return result;
        }

        public byte[] read_bytes(uint size, bool advance = true)
        {
            byte[] result = new byte[size];
            Array.Copy(bytes, _iterator, result, 0, size);
            if (advance)
                _iterator += size;
            return result;
        }

        public byte[] read_remainingbytes()
        {
            byte[] result = new byte[bytes.Length - _iterator];
            Array.Copy(bytes, _iterator, result, 0, bytes.Length - _iterator);
            return result;
        }

        public string read_AsciiString(uint strLength, bool advance = true)
        {
            string result = Encoding.ASCII.GetString(bytes, (int)_iterator, (int)strLength);
            if (advance)
                _iterator += strLength;
            return result;
        }

        public string read_AsciiString(bool advance = true)
        {
            uint strLength = 0;

            while (bytes[_iterator + strLength] != 0)
            {
                strLength++;
            }

            string result = Encoding.ASCII.GetString(bytes, (int)_iterator, (int)strLength);
            if (advance)
                _iterator += strLength + 1;
            return result;
        }

        public string read_alignedAsciiString()
        {
            uint strLength = read_uint();
            uint alignmentOffset = strLength % sizeof(uint) == 0 ? 0 : sizeof(uint) - (strLength % sizeof(uint));

            string result = Encoding.ASCII.GetString(bytes, (int)_iterator, (int)strLength);
            _iterator += strLength + alignmentOffset;
            return result;
        }

        public string read_alignedUnicodeString()
        {
            uint strLength = read_uint() * 2;
            uint alignmentOffset = strLength % sizeof(uint) == 0 ? 0 : sizeof(uint) - (strLength % sizeof(uint));

            string result = Encoding.Unicode.GetString(bytes, (int)_iterator, (int)strLength);
            _iterator += strLength + alignmentOffset;
            return result;
        }

        public string read_UnicodeString(int strLength, bool advance = true)
        {
            string result = Encoding.Unicode.GetString(bytes, (int)_iterator, (int)strLength*2);
            if (advance)
                _iterator += (uint)strLength*2;
            return result;
        }

        public string read_UnicodeString(bool advance = true)
        {
            uint pos = _iterator;

            do
            {
                pos += 2;
            }
            while (bytes[pos] != 0 << 8 | bytes[pos + 1] != 0);

            uint length = pos - _iterator;

            string result = Encoding.Unicode.GetString(bytes, (int)_iterator, (int)length);
            if (advance)
                _iterator += length + 2;
            return result;
        }

        public Guid read_guid()
        {
            ReadOnlySpan<byte> guidBytes = new ReadOnlySpan<byte>(bytes, (int)_iterator, GUID_Size);
            _iterator += GUID_Size;
            return new Guid(guidBytes);
        }

        public bool advance_TerminalIdentifier()
        {
            uint nullTerminator = read_uint();
            return nullTerminator == 0;
        }

        public object? marshal_toObject(Type objType, uint objSize)
        {
            if (GetRemainingLength() < objSize)
            {
                return null;
            }

            byte[] bytes = read_bytes(objSize, false);
            IntPtr marshalPointer = IntPtr.Zero;
            marshalPointer = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, marshalPointer, bytes.Length);
            return Marshal.PtrToStructure(marshalPointer, objType);
        }

        private int GetRemainingLength()
        {
            return bytes.Length - (int)_iterator;
        }
    }
}
