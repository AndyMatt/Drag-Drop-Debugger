using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class FileEntryShellItem : TabbedClass
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

            mSize = byteReader.read_ushort();
            mClassType = byteReader.read_byte();
            mUnknown = byteReader.read_byte();
            mFileSize = byteReader.read_uint();
            mLastModificationTime = byteReader.read_uint();
            mFileAttributes = byteReader.read_ushort();
            mPrimaryName = byteReader.read_AsciiString();
            byteReader.SkipTerminators();

            TabControl childTab = TabHelper.AddSubTab(parentTab, GetClassTypeString(mClassType));
            TabHelper.AddRawDataTab(childTab, rawData);

            if (hasExtensionBlock(byteReader))
            {
                mExtensionBlock = new FileEntryExtensionBlock(childTab, byteReader, mClassType);
            }

            TabHelper.AddDataGridTab(childTab, "Header", new Dictionary<string, object>()
            {
                {"Size", $"{mSize} (0x{mSize.ToString("X")})"},
                {"Class Type", mClassType},
                {"Unknown", mUnknown },
                {"FileSize", mFileSize },
                {"Last Modification Date", mLastModificationTime },
                {"FileAttributes",mFileAttributes },
                {"PrimaryName", mPrimaryName },
                {"ExtensionBlock", (mExtensionBlock != null ? mExtensionBlock.mTabReference : "NULL") },
            }, 0);

            mTabReference = childTab;
        }

        string GetClassTypeString(byte _classTypeID)
        {
            if ((_classTypeID & 0x1) == 0x1)
                return "DirectoryShellItem";

            return "FileEntryShellItem";
        }

        public string GetPropertyString()
        {
            string fileName = mExtensionBlock != null ? mExtensionBlock.GetFileName() : mPrimaryName;

            if ((mClassType & 0x1) == 0x1)
                return $"DirectoryShellItem({fileName})";

            return $"FileEntryShellItem({fileName})";
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
