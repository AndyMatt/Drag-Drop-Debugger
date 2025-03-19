using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class FileEntryShellItem
    {
        ushort mSize;
        byte mClassType;
        byte mUnknown; //0x00
        uint mFileSize;
        uint mLastModificationTime;
        ushort mFileAttributes;
        string mPrimaryName;

        FileEntryExtensionBlock? mExtensionBlock;

        public FileEntryShellItem(TabControl parentTab, ByteReader byteReader)
        {
            byte[] rawData = byteReader.read_bytes(byteReader.read_ushort(false), false);
            TabHelper.AddRawDataTab(parentTab, rawData);

            mSize = byteReader.read_ushort();
            mClassType = byteReader.read_byte();
            mUnknown = byteReader.read_byte();
            mFileSize = byteReader.read_uint();
            mLastModificationTime = byteReader.read_uint();
            mFileAttributes = byteReader.read_ushort();
            mPrimaryName = byteReader.read_AsciiString();
            byteReader.SkipTerminators();


            if (hasExtensionBlock(byteReader))
            {
                mExtensionBlock = new FileEntryExtensionBlock(parentTab, byteReader);
            }

            TabHelper.AddStringListTab(parentTab, "Header", new string[]{

                $"Size: {mSize} (0x{mSize.ToString("X")})",
                $"ClassType: {mClassType}",
                $"Unknown: {mUnknown}",
                $"FileSize: {mFileSize}",
                $"LastModificationDate: {mLastModificationTime}",
                $"FileAttributes: {mFileAttributes}",
                $"PrimaryName: {mPrimaryName}" }, 0);
        }

        const uint ExtensionSignitureOffset = 4;
        bool hasExtensionBlock(ByteReader byteReader)
        {
            uint signiture = byteReader.scan_uint(ExtensionSignitureOffset);

            if (signiture == 0xBEEF0004)
            {
                return true;
            }

            return false;
        }
    }
}
