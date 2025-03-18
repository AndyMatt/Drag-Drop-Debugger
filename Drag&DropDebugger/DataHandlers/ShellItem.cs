using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Drag_DropDebugger.DataHandlers.ShellIDListArray;

namespace Drag_DropDebugger.DataHandlers
{
    internal class RootFolderShellItem
    {
        /* mflagIdentifier
        The volume shell item can be identified by a value of 0x20 after applying a bitmask of 0x70. 
        The remaining bits in the class type indicator are presumed to be a sub-type or flags.

        Value	Description
        0x01    Has name
        0x02    Unknown (0x23 C:, 0x2f C: or D:, 0x2a J:)
        0x04    Unknown (0x23 C:, 0x25 D:)
        0x08    Is removable media (0x23 C:, 0x29 A:, 0x2a J:)
         */
        ushort mSize;
        byte mflagIdentifier;
        string mLabel;
        FileEntryShellItem? mFileEntry;
        public RootFolderShellItem(TabControl parentTab, ByteReader byteReader)
        {
            mSize = byteReader.read_ushort();
            mflagIdentifier = byteReader.read_byte();
            int StringSize = mSize - sizeof(ushort) - sizeof(byte);
            mLabel = byteReader.read_AsciiString((uint)StringSize);
            TabHelper.AddStringListTab(parentTab, "RootFolderShellItem", new string[]
            {
                $"Size: {mSize} | 0x{mSize.ToString("X")}",
                $"Flag Identifier: {mflagIdentifier}",
                $"Label: {mLabel}"
            });

            while(byteReader.scan_ushort() != 0x0)
            {
                ushort identifier = byteReader.scan_ushort(2);

                if((identifier & 0x70) == 0x30)
                {
                    mFileEntry = new FileEntryShellItem(TabHelper.AddSubTab(parentTab, "FileEntryShellItem"), byteReader);
                }
            }
        }
    }

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

    internal class FileEntryExtensionBlock //0xBEEF0004
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

            mLongStringSize = (mVersion >= 3) ? byteReader.read_ushort() : (ushort)0;

            mUnknown3 = (mVersion >= 9) ? byteReader.read_uint() : 0;

            mUnknown4 = (mVersion >= 8) ? byteReader.read_uint() : 0;

            mLongName = (mVersion >= 3) ? byteReader.read_UnicodeString() : "";

            mLocalizedName = "";

            if (mLongStringSize > 0)
            {
                if (mVersion >= 7)
                    mLocalizedName = byteReader.read_UnicodeString(); 
                else if (mVersion >= 3)
                    mLocalizedName = byteReader.read_AsciiString();
            }

            if(mVersion >= 3)
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

    internal class ApplicationShellExtensionBlock //0xBEEF0027
    {
        ushort mSize;
        ushort mVersion; // Seen 0x0000
        uint mExtensionSigniture; //0xBEEF0027
        WindowsPropertySet? mPropertySet;
        uint mTerminator;

        public string[] toStringList()
        {
            List<string> list = new List<string>();
            list.Add($"Size: {mSize} (0x{mSize.ToString("X")})");
            list.Add($"Version: {mVersion}");
            list.Add($"ExtensionSigniture: 0x{mExtensionSigniture.ToString("X2")}");
            list.Add($"Terminator: {mTerminator}");
            return list.ToArray();
        }

        public ApplicationShellExtensionBlock(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "ApplicationShellExtentionBlock");
            mSize = byteReader.read_ushort();
            mVersion = byteReader.read_ushort();
            mExtensionSigniture = byteReader.read_uint();

            mPropertySet = new WindowsPropertySet(TabHelper.AddSubTab(childTab, "PropertySets"), byteReader, 0);

            mTerminator = byteReader.read_uint();

            TabHelper.AddStringListTab(childTab, "Header", toStringList(), 0);
        }
    }

    internal class RootFolderShellExtensionBlock //0xBEEF0026
    {
        ushort mSize;
        ushort mVersion; // Seen 0x0001
        uint mExtensionSigniture; //0xBEEF0027
        uint mUnknown; //0x00000011
        UInt64 mFileTime1;
        UInt64 mFileTime2;
        UInt64 mFileTime3;
        uint mFirstExtensionBlockOffset;

        public string[] toStringList()
        {
            List<string> list = new List<string>();
            list.Add($"Size: {mSize} (0x{mSize.ToString("X")})");
            list.Add($"Version: {mVersion}");
            list.Add($"ExtensionSigniture: 0x{mExtensionSigniture.ToString("X2")}");
            list.Add($"Unknown: {mUnknown}");
            list.Add($"FileTime1: {mFileTime1}");
            list.Add($"FileTime2: {mFileTime2}");
            list.Add($"FileTime3: {mFileTime3}");
            list.Add($"FirstExtensionBlockOffset: {mFirstExtensionBlockOffset}");
            return list.ToArray();
        }

        public RootFolderShellExtensionBlock(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "RootFolderShellExtensionBlock");
            mSize = byteReader.read_ushort();
            mVersion = byteReader.read_ushort();
            mExtensionSigniture = byteReader.read_uint();
            mUnknown = byteReader.read_uint();
            mFileTime1 = byteReader.read_uint64();
            mFileTime2 = byteReader.read_uint64();
            mFileTime3 = byteReader.read_uint64();
            mFirstExtensionBlockOffset = byteReader.read_uint();

            TabHelper.AddStringListTab(childTab, "Header", toStringList(), 0);
        }
    }

    internal class ApplicationShellItem
    {
        ushort mSize;
        ushort Unknown_1;
        ushort mInnerSize;
        string mSigniture; //4 Bytes
        ushort mInnerInnerSize;
        uint unknown_2; //0x00030008
        uint unknown_3; //0x0
        ushort unknown_4; //0x0
        uint mPropertyStoreOffset;
        ApplicationShellExtensionBlock? mExtensionBlock;

        List<WindowsPropertySet> mProperties;

        public ApplicationShellItem(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "ApplicationShellItem");
            mSize = byteReader.read_ushort();
            Unknown_1 = byteReader.read_ushort();
            mInnerSize = byteReader.read_ushort();
            mPropertyStoreOffset = byteReader.GetOffset() + mInnerSize;
            mSigniture = byteReader.read_AsciiString(4);
            mInnerInnerSize = byteReader.read_ushort();
            unknown_2 = byteReader.read_uint();
            unknown_2 = byteReader.read_uint();
            unknown_4 = byteReader.read_ushort();

            mProperties = new List<WindowsPropertySet>();

            TabControl setsTab = TabHelper.AddSubTab(childTab, "PropertySets");

            while (byteReader.read_uint(false) != 0)
            {
                mProperties.Add(new WindowsPropertySet(setsTab, byteReader, mProperties.Count));
            }

            AddTab(childTab);

            byteReader.SetOffset(mPropertyStoreOffset);
            if (hasExtensionBlock(byteReader))
            {
                mExtensionBlock = new ApplicationShellExtensionBlock(parentTab, byteReader);
            }
        }

        public void AddTab(TabControl parentTab)
        {
            TabHelper.AddStringListTab(parentTab, "Header", new string[]{
                $"Size: {mSize} (0x{mSize.ToString("X")}) (0x{mSize.ToString("X")})",
                $"InnerSize: {mInnerSize}",
                $"Unknown: {Unknown_1}",
                $"mSigniture: {mSigniture}",
                $"mInnerInnerSize: {mInnerInnerSize}",
                $"Unknown: {unknown_2}",
                $"Unknown: {unknown_3}",
                $"Unknown: {unknown_4}",
                $"Number of PropertySet: {mProperties.Count}"}, 0);
        }

        const uint ExtensionSignitureOffset = 4;
        bool hasExtensionBlock(ByteReader byteReader)
        {
            uint signiture = byteReader.scan_uint(ExtensionSignitureOffset);

            if (signiture == 0xBEEF0027)
            {
                return true;
            }

            return false;
        }
    }

    internal class FileEntryShellItem
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

    internal class DelegateFolderShellItem
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
            mSigniture = byteReader.read_AsciiString(4);;
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
            List<String> data = new List<String>();
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

    //{59031A47-3F72-44A7-89C5-5595FE6B30EE}
    class UserFolderShellItem
    {
        RootFolderShellExtensionBlock? mExtensionBlock;

        const uint ExtensionSignitureOffset = 4;
        public UserFolderShellItem(TabControl parentTab, Guid guid, ByteReader byteReader)
        {
            string SpecialPath = NativeMethods.GetPathFromGUID(guid);

            TabHelper.AddStringListTab(parentTab, "GUID", new string[]{
                $"GUID: {{{guid.ToString()}}}",
                $"Path: {SpecialPath}"},0);

            if (byteReader.scan_uint(ExtensionSignitureOffset) == 0xBEEF0026)
            {
                mExtensionBlock = new RootFolderShellExtensionBlock(parentTab, byteReader);
            }
        }
    }

    internal static class ShellItemHandler
    {
        enum SortIndex
        {
            InternetExplorer = 0x00,
            Libraries = 0x42,
            Users = 0x44,
            MyDocuments = 0x48,
            MyComputer = 0x50,
            Network = 0x58,
            RecycleBin = 0x60,
            IExplorer = 0x68,
            Unknown = 0x70,
            MyGames = 0x80,
        }

        static Dictionary<Guid, Type> ClassIDs = new Dictionary<Guid, Type>()
        {
            {new Guid("20D04FE0-3AEA-1069-A2D8-08002B30309D"), typeof(RootFolderShellItem)},
            {new Guid("4234D49B-0245-4DF3-B780-3893943456E1"), typeof(ApplicationShellItem)},
            {new Guid("59031A47-3F72-44A7-89C5-5595FE6B30EE"), typeof(UserFolderShellItem)},
            {new Guid("24AD3AD4-A569-4530-98E1-AB02F9417AA8"), typeof(UserFolderShellItem)},
            {new Guid("088e3905-0323-4b02-9826-5d99428e115f"), typeof(UserFolderShellItem)},
            {new Guid("1cf1260c-4dd0-4ebb-811f-33c572699fde"), typeof(UserFolderShellItem)},
            {new Guid("a8cdff1c-4878-43be-b5fd-f8091c1c60d0"), typeof(UserFolderShellItem)},
            {new Guid("b4bfcc3a-db2c-424c-b029-7fe99a87c641"), typeof(UserFolderShellItem)},
            {new Guid("374de290-123f-4565-9164-39c4925e467b"), typeof(UserFolderShellItem)},
            {new Guid("3add1653-eb32-4cb0-bbd7-dfa0abb5acca"), typeof(UserFolderShellItem)},
            {new Guid("a0953c92-50dc-43bf-be83-3742fed03c9c"), typeof(UserFolderShellItem)},
        };

        public static object? Handle(TabControl parentTab, ByteReader byteReader)
        {
            ushort size = byteReader.read_ushort();
            byte indicator = byteReader.read_byte();

            if(indicator == 0x1f)
            {
                byte sortIndex = byteReader.read_byte();
                Guid classID = byteReader.read_guid();

                if (ClassIDs.ContainsKey(classID))
                {
                    Type ClassType = ClassIDs[classID];

                    TabControl childTab = TabHelper.AddSubTab(parentTab, ClassType.Name);
                    TabHelper.AddStringListTab(childTab, "Header", new string[] {
                        $"Size: {size}",
                        $"Indicator: {indicator}",
                        $"SortIndex: {sortIndex}",
                        $"Guid: {classID.ToString()}" },0);

                    if (ClassType == typeof(UserFolderShellItem))
                    {
                        
                        return new UserFolderShellItem(childTab, classID, byteReader);
                    }
           
                    return Activator.CreateInstance(ClassType, childTab, byteReader);
                }
                else
                {
                    TabHelper.AddStringTab(parentTab, "MissingGuid", $"GIUD {{{classID.ToString()}}} is Missing");
                }
            }
            else if(indicator == 0x32)
            {
                byteReader.RollBack(3);
                TabControl childTab = TabHelper.AddSubTab(parentTab, "FileEntryShellItem");
                return new FileEntryShellItem(childTab, byteReader);
            }
            else if(indicator == 0x74)
            {
                byteReader.RollBack(3);
                TabControl childTab = TabHelper.AddSubTab(parentTab, "DelegateFolderShellItem");
                return new DelegateFolderShellItem(childTab, byteReader);
            }

            return null;

        }
    }
}
