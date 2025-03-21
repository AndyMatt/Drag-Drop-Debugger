﻿using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    internal class NTFSFileReference
    {
        public ulong mEntryIndex;
        public ushort mSequenceNumber;

        public NTFSFileReference(ulong reference)
        {
            mEntryIndex = reference >> 16;
            mSequenceNumber = (ushort)(reference << 48 >> 48);
        }
    }

    public class FileEntryExtensionBlock //0xBEEF0004
    {
        ushort mSize;
        ushort mVersion; // 3=>Windows XP or 2003   7=> Windows Vista (SP0) 8=> Windows 2008, 7, 8.0    9=> Windows 8.1, 10, 11
        uint mExtensionSigniture; //0xBEEF0004
        uint mCreationModificationTime;
        uint mLastAccessTime;
        ushort mUnknownVersion; //0x14 ⇒ Windows XP or 2003   0x26 ⇒ Windows Vista(SP0)   0x2a ⇒ Windows 2008, 7, 8.0 0x2e ⇒ Windows 8.1, 10
        ushort mUnknown1; //0x0
        NTFSFileReference? mNtfsReference;
        ulong mUnknown2; //0x0
        ushort mLongStringSize;
        uint mUnknown3; //0x0
        uint mUnknown4; //0x0
        string mLongName;
        string mLocalizedName;
        ushort mExtensionBlockOffset;

        public FileEntryExtensionBlock(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "FileEntryExtensionBlock");
            byte[] rawData = byteReader.read_bytes(byteReader.read_ushort(false), false);
            TabHelper.AddRawDataTab(childTab, rawData);

            mSize = byteReader.read_ushort();
            mVersion = byteReader.read_ushort();
            mExtensionSigniture = byteReader.read_uint();
            mCreationModificationTime = byteReader.read_uint();
            mLastAccessTime = byteReader.read_uint();
            mUnknownVersion = byteReader.read_ushort();

            if (mVersion >= 7)
            {
                mUnknown1 = byteReader.read_ushort();
                mNtfsReference = new NTFSFileReference(byteReader.read_uint64());
                mUnknown2 = byteReader.read_uint64();
            }

            mLongStringSize = mVersion >= 3 ? byteReader.read_ushort() : (ushort)0;

            mUnknown3 = mVersion >= 9 ? byteReader.read_uint() : 0;

            mUnknown4 = mVersion >= 8 ? byteReader.read_uint() : 0;

            mLongName = mVersion >= 3 ? byteReader.read_UnicodeString() : "";

            mLocalizedName = "";

            if (mLongStringSize > 0)
            {
                if (mVersion >= 7)
                    mLocalizedName = byteReader.read_UnicodeString();
                else if (mVersion >= 3)
                    mLocalizedName = byteReader.read_AsciiString();
            }

            if (mVersion >= 3)
            {
                mExtensionBlockOffset = byteReader.read_ushort();
            }

            AddTab(childTab);
        }

        void AddTab(TabControl parentTab)
        {
            List<string> list = new List<string>();
            list.Add($"Size: {mSize} (0x{mSize.ToString("X")})");
            list.Add($"Version: {mVersion}");
            list.Add($"ExtensionSigniture: 0x{mExtensionSigniture.ToString("X2")}");
            list.Add($"CreationModificationTime: {mCreationModificationTime}");
            list.Add($"LastAccessTime: {mLastAccessTime}");
            list.Add($"Unknown|Version: {mUnknownVersion}");
            if (mNtfsReference != null)
            {
                list.Add($"mNtfsReference:");
                list.Add($"    EntryIndex: {mNtfsReference.mEntryIndex}");
                list.Add($"    SequenceNumber: {mNtfsReference.mSequenceNumber}");
            }
            list.Add($"LongStringSize: {mLongStringSize}");
            list.Add($"LongName: {mLongName}");
            list.Add($"LocalizedName: {mLocalizedName}");
            list.Add($"mExtensionBlockOffset: 0x{mExtensionBlockOffset.ToString("X2")}");
            TabHelper.AddStringListTab(parentTab, "Header", list.ToArray(), 0);
        }
    }
}
