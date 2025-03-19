using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class DelegateFolderShellItem
    {
        ushort mSize;
        byte mClassType;
        byte mUnknown; //0x00
        ushort mInnerDataSize;
        string mSigniture; // 4 Bytes ("CFSF")
        FileEntryShellItem mFileShellEntry;
        Guid mDelegateClassId; //{5e591a74-df96-48d3-8d67-1733bcee28ba}
        Guid mDelegateFolderId; //{dffacdc5-679f-4156-8947-c5c76bc0b67f}

        FileEntryExtensionBlock? mExtensionBlock;


        public DelegateFolderShellItem(TabControl parentTab, ByteReader byteReader)
        {
            byte[] rawData = byteReader.read_bytes(byteReader.read_ushort(false), false);
            TabHelper.AddRawDataTab(parentTab, rawData);

            mSize = byteReader.read_ushort();
            mClassType = byteReader.read_byte();
            mUnknown = byteReader.read_byte();
            mInnerDataSize = byteReader.read_ushort();
            mSigniture = byteReader.read_AsciiString(4); ;
            mFileShellEntry = new FileEntryShellItem(TabHelper.AddSubTab(parentTab, "FileEntryShellItem"), byteReader);
            mDelegateClassId = byteReader.read_guid();
            mDelegateFolderId = byteReader.read_guid();

            if (hasExtensionBlock(byteReader))
            {
                mExtensionBlock = new FileEntryExtensionBlock(parentTab, byteReader);
            }

            AddTab(parentTab);
        }

        void AddTab(TabControl parentTab)
        {
            List<string> data = new List<string>();
            data.Add($"Size: {mSize} (0x{mSize.ToString("X")})");
            data.Add($"Unknown: {mUnknown}");
            data.Add($"InnerDataSize: {mInnerDataSize}");
            data.Add($"Signiture: {mSigniture}");
            data.Add($"DelegateClassId: {mDelegateClassId.ToString()}");
            data.Add($"DelegateFolderId: {mDelegateFolderId.ToString()}");
            data.Add("");
            data.Add("");

            TabHelper.AddStringListTab(parentTab, "Header", data.ToArray(), 0);
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
