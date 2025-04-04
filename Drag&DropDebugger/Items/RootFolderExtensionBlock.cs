using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class RootFolderExtensionBlock : TabbedClass//0xBEEF0026
    {
        ushort mSize;
        ushort mVersion; // Seen 0x0001
        uint mExtensionSigniture; //0xBEEF0026
        uint mUnknownFlag; //0x00000011
        public WinDateTime mCreationDateTime;
        public WinDateTime mLastModifiedDateTime;
        public WinDateTime mLastAccessDateTime;


        ushort mListSize;
        ushort mFirstExtensionBlockOffset;

        public RootFolderExtensionBlock(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "RootFolderShellExtensionBlock");
            mSize = byteReader.read_ushort();
            mVersion = byteReader.read_ushort();
            mExtensionSigniture = byteReader.read_uint();
            mUnknownFlag = byteReader.read_uint();
            if ((mUnknownFlag & 0x10) == 0x10)
            {
                mCreationDateTime = byteReader.read_uint64();
                mLastModifiedDateTime = byteReader.read_uint64();
                mLastAccessDateTime = byteReader.read_uint64();

                mFirstExtensionBlockOffset = byteReader.read_ushort();
            }

            TabHelper.AddDataGridTab(childTab, "Header", new Dictionary<string, object>()
            {
                {"Size", $"{mSize} (0x{mSize.ToString("X")}"},
                {"Version", mVersion},
                {"ExtensionSigniture", $"0x{mExtensionSigniture.ToString("X2")}"},
                {"UnknownFlag", mUnknownFlag},
                {"DateCreated", mCreationDateTime},
                {"DateLastModified", mLastModifiedDateTime},
                {"DateLastAccess", mLastAccessDateTime},
                {"FirstExtensionBlockOffset", $"{mFirstExtensionBlockOffset} (0x{mFirstExtensionBlockOffset.ToString("X")}"},
            }, 0);

            mTabReference = childTab;
        }
    }
}
