using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class RootFolderExtensionBlock //0xBEEF0026
    {
        ushort mSize;
        ushort mVersion; // Seen 0x0001
        uint mExtensionSigniture; //0xBEEF0027
        uint mUnknownFlag; //0x00000011
        ulong mFileTime1;
        ulong mFileTime2;
        ulong mFileTime3;

        ushort mListSize;
        ushort mFirstExtensionBlockOffset;

        public string[] toStringList()
        {
            List<string> list = new List<string>();
            list.Add($"Size: {mSize} (0x{mSize.ToString("X")})");
            list.Add($"Version: {mVersion}");
            list.Add($"ExtensionSigniture: 0x{mExtensionSigniture.ToString("X2")}");
            list.Add($"UnknownFlag: {mUnknownFlag}");
            list.Add($"FileTime1: {mFileTime1}");
            list.Add($"FileTime2: {mFileTime2}");
            list.Add($"FileTime3: {mFileTime3}");
            list.Add($"FirstExtensionBlockOffset: {mFirstExtensionBlockOffset}");
            return list.ToArray();
        }

        public RootFolderExtensionBlock(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "RootFolderShellExtensionBlock");
            mSize = byteReader.read_ushort();
            mVersion = byteReader.read_ushort();
            mExtensionSigniture = byteReader.read_uint();
            mUnknownFlag = byteReader.read_uint();
            if ((mUnknownFlag & 0x10) == 0x10)
            {
                mFileTime1 = byteReader.read_uint64();
                mFileTime2 = byteReader.read_uint64();
                mFileTime3 = byteReader.read_uint64();

                mFirstExtensionBlockOffset = byteReader.read_ushort();

            }

            TabHelper.AddStringListTab(childTab, "Header", toStringList(), 0);
        }
    }
}
