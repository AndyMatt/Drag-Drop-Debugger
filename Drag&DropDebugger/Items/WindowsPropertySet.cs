﻿using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.Items
{
    public class WindowsPropertySet : TabbedClass
    {
        uint mSize;
        string mVersion;
        public Guid mClassID;
        //SimplePropertyRecord mRecord;
        dynamic mPropertySet;

        const uint mVersionSize = 4;
        public string PropertySetName = "";

        static Dictionary<Guid, Type> ClassIDs = new Dictionary<Guid, Type>()
        {
            {new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), typeof(ApplicationShellPropertySets)},
            {new Guid("B9B4B3FC-2B51-4A42-B5D8-324146AFCF25"), typeof(TargetParsingPath)},
            {new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9"), typeof(SummaryInformationPropertySet)},
            {new Guid("B725F130-47EF-101A-A5F1-02608C9EEBAC"), typeof(CommonOpenProperties)},
            {new Guid("86D40B4D-9069-443C-819A-2A54090DCCEC"), typeof(TileProperties)},
            {new Guid("446D16B1-8DAD-4870-A748-402EA43D788C"), typeof(SystemProperties)},
            {new Guid("FFAE9DB7-1C8D-43FF-818C-84403AA3732D"), typeof(SourcePackageFamilyName)},
            {new Guid("0DED77B3-C614-456C-AE5B-285B38D7B01B"), typeof(LauncherProperties)},
            {new Guid("46588ae2-4cbc-4338-bbfc-139326986dce"), typeof(SecurityIdentifier)},
        };

        public WindowsPropertySet(ByteReader byteReader, int index = -1)
        {
            byte[] rawData = byteReader.read_bytes(byteReader.read_uint(false));
            ByteReader propertyReader = new ByteReader(rawData);

            mSize = propertyReader.read_uint();
            mVersion = propertyReader.read_AsciiString(mVersionSize);
            mClassID = propertyReader.read_guid();

            TabControl childTab = TabHelper.CreateTab();
            TabHelper.AddRawDataTab(childTab, rawData);

            if (ClassIDs.ContainsKey(mClassID))
            {
                Type ClassType = ClassIDs[mClassID];
                mPropertySet = Activator.CreateInstance(ClassType, childTab, propertyReader);
            }
            else
            {
                mPropertySet = propertyReader.read_remainingbytes();
            }

            PropertySetName = mPropertySet.GetType().Name;

            //TabHelper.SetTabLabel(childTab, $"{index + 1} - {PropertySetName}");

            TabHelper.AddDataGridTab(childTab, "Header", new Dictionary<string, object>()
                {
                    {"Size", $"{mSize} (0x{mSize.ToString("X")})" },
                    {"mVersion", mVersion },
                    {"mClassID", mClassID },
                    { mPropertySet.GetType() == typeof(byte[]) ? "UnknownPropertySet" : mPropertySet.GetType().Name,  mPropertySet.GetType() == typeof(byte[]) ? mPropertySet : mPropertySet.mTabReference }
                }, 0);

            mTabReference = childTab;
        }
    }
}
