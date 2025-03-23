using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public uint read_uint(bool advance = true)
        {
            uint result = (uint)bytes[_iterator + 3] << 24 | (uint)bytes[_iterator + 2] << 16 | (uint)bytes[_iterator + 1] << 8 | bytes[_iterator];
            if (advance)
                _iterator += sizeof(uint);
            return result;
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
    }
}
