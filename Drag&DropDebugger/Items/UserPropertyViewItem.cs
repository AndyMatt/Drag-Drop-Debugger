using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class UserPropertyViewItem : TabbedClass //0x23FEBBEE
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

            mTabReference = TabHelper.AddDataGridTab(parentTab, "Properties", new Dictionary<string, object>()
                {
                    {"Size", $"{mSize} (0x{mSize.ToString("X")})" },
                    {"ClassTypeID", mClassTypeID },
                    {"UnknownField", mUnknown },
                    {"UserPropertyView.Size", $"{mData.mSize} (0x{mData.mSize.ToString("X")})" },
                    {"UserPropertyView.DataSigniture", $"0x{mData.mDataSigniture.ToString("X")}" },
                    {"UserPropertyView.PropertyStoreDataSize", mData.mPropertyStoreDataSize },
                    {"UserPropertyView.IdentifierSize", mData.mIdentifierSize},
                    {"UserPropertyView.KnownFolder.GUID", mData.mKnownFolder.ToString()},
                    {"UserPropertyView.KnownFolder.Path", NativeMethods.GetFolderFromKnownFolderGUID(mData.mKnownFolder) }
                }, 0);
        }
    }
}
