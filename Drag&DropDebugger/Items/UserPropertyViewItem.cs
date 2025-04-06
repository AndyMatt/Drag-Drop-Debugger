using Drag_DropDebugger.Helpers;
using Drag_DropDebugger.UI;
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
            StackedDataTab stackedDataTab = new StackedDataTab("UserPropertyViewItem");
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

            mTabReference = TabHelper.AddStackTab(parentTab, stackedDataTab);


            List<KeyValuePair<string, object>> Entries = new List<KeyValuePair<string, object>>();
            while (!byteReader.End() && byteReader.scan_ushort() != 0x0)
            {
                ushort identifier = byteReader.scan_ushort(2);

                if ((identifier & 0x70) == 0x30)
                {
                    FileEntryShellItem fileEntry = new FileEntryShellItem(parentTab, byteReader);
                    Entries.Add(new KeyValuePair<string, object>(fileEntry.GetPropertyString(), fileEntry.mTabReference));
                }
            }

            stackedDataTab.AddDataGrid("Properties", new Dictionary<string, object>()
            {
                {"Size", $"{mSize} (0x{mSize.ToString("X")})" },
                {"ClassTypeID", mClassTypeID },
                {"UnknownField", mUnknown },
            });        

            stackedDataTab.AddDataGrid("UserPropertyView", new Dictionary<string, object>()
            {
                {"Size", $"{mData.mSize} (0x{mData.mSize.ToString("X")})" },
                {"DataSigniture", $"0x{mData.mDataSigniture.ToString("X")}" },
                {"PropertyStoreDataSize", mData.mPropertyStoreDataSize },
                {"IdentifierSize", mData.mIdentifierSize},
            }, 0);

            stackedDataTab.AddDataGrid("KnownFolder", new Dictionary<string, object>()
            {
                {"GUID", mData.mKnownFolder.ToString()},
                {"Path", NativeMethods.GetFolderFromKnownFolderGUID(mData.mKnownFolder) }
            }, 0);

            stackedDataTab.AddDataGrid("Entries", Entries);
        }
    }
}
