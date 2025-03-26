using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class UserPropertyViewItem //0x23FEBBEE
    {
        class UserPropertyViewData
        {
            public ushort mSize;
            public uint mDataSigniture;
            public ushort mPropertyStoreDataSize;
            public ushort mIdentifierSize;
            public Guid mKnownFolder;
        }

        ushort mSize;
        byte mClassTypeID; // 0x0 Seen
        byte mUnknown; // 0x0
        UserPropertyViewData? mData;

        public UserPropertyViewItem(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "UserPropertyViewItem");
            ByteReader viewReader = new ByteReader(byteReader.read_bytes(byteReader.scan_ushort()));
            mSize = viewReader.read_ushort();
            mClassTypeID = viewReader.read_byte();
            mUnknown = viewReader.read_byte();

            mData = new UserPropertyViewData();
            mData.mSize = viewReader.read_ushort();
            mData.mDataSigniture = viewReader.read_uint();
            mData.mPropertyStoreDataSize = viewReader.read_ushort();
            mData.mIdentifierSize = viewReader.read_ushort();
            mData.mKnownFolder = viewReader.read_guid();

            AddTab(childTab);
        }

        void AddTab(TabControl parentTab)
        {
            if (mData != null)
            {
                string KnownFolderName = NativeMethods.GetFolderFromKnownFolderGUID(mData.mKnownFolder);
                TabHelper.AddStringListTab(parentTab, "Properties", new string[]
                {
                    $"Size: {mSize}",
                    $"ClassTypeID: {mClassTypeID}",
                    $"UnknownField: {mUnknown}",
                    "",
                    "UserPropertyViewData",
                    $"  Size: {mData.mSize}",
                    $"  DataSigniture: 0x{mData.mDataSigniture.ToString("X")}",
                    $"  PropertyStoreDataSize: {mData.mPropertyStoreDataSize}",
                    $"  IdentifierSize: {mData.mIdentifierSize}",
                    $"  KnownFolder",
                    $"    GUID: {mData.mKnownFolder.ToString()}",
                    $"    Path: {KnownFolderName}",

                });
            }
        }
    }
}
