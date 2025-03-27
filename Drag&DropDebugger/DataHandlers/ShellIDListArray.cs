using Drag_DropDebugger.Helpers;
using Drag_DropDebugger.Items;
using Drag_DropDebugger.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfHexaEditor.Core;

namespace Drag_DropDebugger.DataHandlers
{
    public class ShellIDListArray : TabbedClass
    {
        class FolderContext
        {
            public uint mSize;
            public byte[] mData;
            public FolderContext()
            {
                mSize = 0;
                mData = new byte[0];
            }

            public TabItem CreateTab(TabControl tabCtrl, ByteReader byteReader)
            {
                mSize = byteReader.read_ushort(false);
                uint dataLength = (mSize + CIDA_NULL_TERMINATOR) * sizeof(ushort);
                mData = byteReader.read_bytes(dataLength);

                return TabHelper.AddStringListTab(tabCtrl, "FolderContext", new string[] {
                    $"Size: {mSize} (0x{mSize.ToString("X")})",
                    $"Data: {Convert.ToHexString(mData)}"});
            }
        }

        static uint CIDA_NULL_TERMINATOR = 1;
        class CIDA_Structure
        {
            public uint cidl; //Size
            public uint[] aoffset; //Elements (cidl+1)
            public List<Object> mChildren;
            public List<UIElement?> mTabs;

            public CIDA_Structure(TabControl ParentTab, ByteReader byteReader)
            {
                TabControl childTab = TabHelper.AddSubTab(ParentTab, "CIDA");
                TabHelper.AddRawDataTab(childTab, byteReader.copy_allbytes());

                cidl = byteReader.read_uint();
                aoffset = new uint[cidl + 1];
                mChildren = new List<Object>();
                mTabs = new List<UIElement?>();

                for (int i = 0; i < aoffset.Length; i++)
                {
                    aoffset[i] = byteReader.read_uint();
                }

                for (int i = 0; i < aoffset.Length; i++)
                {
                    byteReader.SetOffset(aoffset[i]);
                    ushort _Size = byteReader.read_ushort(false);

                    if (_Size < 2)
                    {
                        FolderContext folderContext = new FolderContext();
                        mTabs.Add(folderContext.CreateTab(childTab, byteReader));
                        mChildren.Add(folderContext);
                    }
                    else
                    {
                        TabControl? tabControl;
                        Object? obj = ShellItemHandler.Handle(childTab, byteReader, out tabControl);
                        if (obj != null)
                        {
                            mChildren.Add(obj);
                            mTabs.Add(tabControl);
                        }
                    }

                }

                AddTab(childTab);
            }

            void AddTab(TabControl ParentTab)
            {
                StackedDataTab childTab = new StackedDataTab("Header");
                childTab.AddDataGrid("Properties", new Dictionary<string, object>
                {
                    {"Size",  cidl},
                    {"OffsetCount", aoffset},
                });

                for (int i = 0; i < mChildren.Count; i++)
                {
                    childTab.AddDataGrid($"Offset #{i}", new Dictionary<string, object>
                    {
                        {"Position",  aoffset[i]},
                        {"Type",  mChildren[i].GetType().Name},
                        {mChildren[i].GetType().Name,  mTabs[i]}
                    });
                }
                TabHelper.AddStackTab(ParentTab, "CIDA", childTab, 0);
            }

        }

        public ShellIDListArray(TabControl ParentTab, MemoryStream dropData, string dropType)
        {
            TabControl childTabCtrl = TabHelper.AddSubTab(ParentTab, "ShellIDListArray");
            ByteReader byteReader = new ByteReader(dropData.ToArray());
            CIDA_Structure _cida = new CIDA_Structure(childTabCtrl, byteReader);
            mTabReference = childTabCtrl;
        }
    }
}
