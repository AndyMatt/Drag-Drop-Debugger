﻿using Drag_DropDebugger.Helpers;
using Drag_DropDebugger.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class ApplicationShellExtensionBlock: TabbedClass //0xBEEF0027
    {
        ushort mSize;
        ushort mVersion; // Seen 0x0000
        uint mExtensionSigniture; //0xBEEF0027
        WindowsPropertySet? mPropertySet;
        uint mTerminator;
        ushort mFirstExtensionBlockOffset;

        public ApplicationShellExtensionBlock(TabControl parentTab, ByteReader byteReader)
        {
            TabControl childTab = TabHelper.AddSubTab(parentTab, "ApplicationShellExtentionBlock");
            mSize = byteReader.read_ushort();
            mVersion = byteReader.read_ushort();
            mExtensionSigniture = byteReader.read_uint();

            byte[] propertySetBytes = byteReader.read_bytes(byteReader.scan_uint());
            ByteReader propertySetReader = new ByteReader(propertySetBytes);

            PropertySetTab propertySetTab = new PropertySetTab();
            mPropertySet = new WindowsPropertySet(propertySetReader, 0);
            propertySetTab.AddPropertySet(mPropertySet);
            childTab.Items.Add(propertySetTab);

            mTerminator = byteReader.read_uint();
            mFirstExtensionBlockOffset = byteReader.read_ushort();

            TabHelper.AddDataGridTab(childTab, "Header", new Dictionary<string, object>()
            {
                {"Size", mSize},
                {"Version", mVersion},
                {"ExtensionSigniture", $"0x{mExtensionSigniture.ToString("X2")}"},
                {"Terminator", mTerminator},
                {"FirstExtensionBlockOffset", mFirstExtensionBlockOffset},
                {"PropertySet", propertySetTab }
            }, 0);

            mTabReference = childTab;
        }
    }
}
