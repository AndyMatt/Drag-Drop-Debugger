using Drag_DropDebugger.Helpers;
using Drag_DropDebugger.Items;
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
    public class ShellIDListArray
    {
        class FolderContext
        {
            public uint mSize;
            public byte[] mData;
            public FolderContext(TabControl tabCtrl, ByteReader byteReader)
            {
                mSize = byteReader.read_ushort(false);
                uint dataLength = (mSize + CIDA_NULL_TERMINATOR) * sizeof(ushort);
                mData = byteReader.read_bytes(dataLength);

                TabHelper.AddStringListTab(tabCtrl, "FolderContext", new string[] {
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

            public CIDA_Structure(TabControl ParentTab, ByteReader byteReader)
            {
                TabControl childTab = TabHelper.AddSubTab(ParentTab, "CIDA");
                TabHelper.AddRawDataTab(childTab, byteReader.copy_allbytes());

                cidl = byteReader.read_uint();
                aoffset = new uint[cidl + 1];
                mChildren = new List<Object>();

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
                        mChildren.Add(new FolderContext(childTab, byteReader));
                    }
                    else
                    {
                        Object? obj = ShellItemHandler.Handle(childTab, byteReader);
                        if(obj != null)
                            mChildren.Add(obj);
                    }

                }

                AddTab(childTab);
            }

            void AddTab(TabControl ParentTab)
            {
                List<string> dataStrings = new List<string>();
                dataStrings.Add($"Size: {cidl}");
                dataStrings.Add($"OffsetCount: {aoffset.Length}");
                dataStrings.Add("");
                for (int i = 0; i < mChildren.Count; i++)
                {
                    dataStrings.Add($"Offset {i + 1}:");
                    dataStrings.Add($"    Position: {aoffset[i]}");
                    dataStrings.Add($"    Type: {mChildren[i].GetType().Name}");
                    dataStrings.Add("");
                }

                TabHelper.AddStringListTab(ParentTab, "Header", dataStrings.ToArray(), 0);
            }

        }

        public static TabControl Handle(TabControl ParentTab, MemoryStream dropData)
        {
            TabControl childTabCtrl = TabHelper.AddSubTab(ParentTab, "ShellIDListArray");
            ByteReader byteReader = new ByteReader(dropData.ToArray());
            CIDA_Structure _cida = new CIDA_Structure(childTabCtrl,byteReader);

            return childTabCtrl;
        }
    }
}
