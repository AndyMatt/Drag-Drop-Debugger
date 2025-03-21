﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using WpfHexaEditor.Core;
using WpfHexaEditor;
using Drag_DropDebugger.Helpers;

namespace Drag_DropDebugger.DataHandlers
{
    internal class FileGroupDescriptor
    {
        [Flags]
        enum Group_dwFlag : uint
        {
            FD_CLSID = 0x1,
            FD_SIZEPOINT = 0x2,
            FD_ATTRIBUTES = 0x4,
            FD_CREATETIME = 0x8,
            FD_ACCESSTIME = 0x10,
            FD_WRITESTIME = 0x20,
            FD_FILESIZE = 0x40,
            FD_PROGRESSUI = 0x4000,
            FD_LINKUI = 0x8000,
            FD_UNICODE = 0x80000000,
        }

        private static string FlagstoString(uint dwFlags)
        {
            string flagStr = "( 0b" + Convert.ToString(dwFlags, 2).PadLeft(8,'0') + ")" ;
            foreach (Group_dwFlag _dwFlag in Enum.GetValues(typeof(Group_dwFlag)))
            {
                flagStr += ((dwFlags & (uint)_dwFlag) != 0) ? " | " + Enum.GetName(typeof(Group_dwFlag), _dwFlag) : "";
            }
            return flagStr;
        }

        private static string FileAttributestoString(uint dwFileAttributes)
        {
            string attribStr = "( 0b" + Convert.ToString(dwFileAttributes, 2).PadLeft(8, '0') + ")";
            foreach (FileAttributes _fileAttri in Enum.GetValues(typeof(FileAttributes)))
            {
                attribStr += ((dwFileAttributes & (uint)_fileAttri) != 0) ? " | " + Enum.GetName(typeof(FileAttributes), _fileAttri) : "";
            }
            return attribStr;
        }

        private static string GetTimeString(System.Runtime.InteropServices.ComTypes.FILETIME fileTime)
        {
            ulong high = (ulong)fileTime.dwHighDateTime;
            uint low = (uint)fileTime.dwLowDateTime;
            long fileTime64 = (long)((high << 32) + low);
            if (fileTime64 == 0)
                return "NA";

            try
            {
                DateTime dateTime = DateTime.FromFileTimeUtc(fileTime64);
                return dateTime.ToShortDateString() + " " + dateTime.ToLongTimeString();
            }
            catch
            {
                return "NA";
            }
        }

        private static string GetFileSizeString(uint fileSizeHigh, uint fileSizeLow)
        {
            string[] bytePeriods = { " B", " KB", " MB", " GB", " TB" };

            long fileSizeBytes = (fileSizeHigh << 32) + fileSizeLow;
            string byteSizeStr = fileSizeBytes.ToString("N0") + " bytes";

            if(fileSizeBytes < 1024)
                return byteSizeStr;

            int period = 0;
            double fileSizeConv = fileSizeBytes;
            while (fileSizeConv > 1024)
            {
                fileSizeConv /= 1024;
                period++;
            }
            fileSizeConv = Math.Round(fileSizeConv, period > 2 ? 2 : 0);

            return $"{fileSizeConv}{bytePeriods[period]} ({byteSizeStr})";
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class POINTL
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class SIZEL
        {
            public int cx;
            public int cy;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public sealed class FILEGROUPDESCRIPTORW
        {
            public uint cItems;
            public FILEDESCRIPTORW[] fgd;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public sealed class FILEDESCRIPTORW
        {
            public uint dwFlags;
            public Guid clsid;
            public SIZEL sizel;
            public POINTL pointl;
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
        }

        //https://stackoverflow.com/questions/58120399/c-sharp-drag-and-drop-from-outlook-using-net-4-5-or-later
        static void ProcessFileDescriptor(TabControl TabCtrl, MemoryStream fileGroupStream)
        {
            IntPtr fileGroupDescriptorWPointer = IntPtr.Zero;
            try
            {
                //use the underlying IDataObject to get the FileGroupDescriptorW as a MemoryStream
                fileGroupStream.Position = 0;
                byte[] fileGroupDescriptorBytes = new byte[fileGroupStream.Length];
                fileGroupStream.Read(fileGroupDescriptorBytes, 0, fileGroupDescriptorBytes.Length);

                //copy the file group descriptor into unmanaged memory
                fileGroupDescriptorWPointer = Marshal.AllocHGlobal(fileGroupDescriptorBytes.Length);
                Marshal.Copy(fileGroupDescriptorBytes, 0, fileGroupDescriptorWPointer, fileGroupDescriptorBytes.Length);

                //marshal the unmanaged memory to to FILEGROUPDESCRIPTORW struct
                object fileGroupDescriptorObject = Marshal.PtrToStructure(fileGroupDescriptorWPointer, typeof(FILEGROUPDESCRIPTORW));
                FILEGROUPDESCRIPTORW fileGroupDescriptor = (FILEGROUPDESCRIPTORW)fileGroupDescriptorObject;

                //create a new array to store file names in of the number of items in the file group descriptor
                string[] fileNames = new string[fileGroupDescriptor.cItems];

                //get the pointer to the first file descriptor
                //get the pointer to the first file descriptor
                IntPtr fileDescriptorPointer = IntPtr.Add(fileGroupDescriptorWPointer, Marshal.SizeOf(fileGroupDescriptor.cItems));

                //loop for the number of files acording to the file group descriptor
                for (int fileDescriptorIndex = 0; fileDescriptorIndex < fileGroupDescriptor.cItems; fileDescriptorIndex++)
                {
                    //marshal the pointer top the file descriptor as a FILEDESCRIPTORW struct and get the file name
                    FILEDESCRIPTORW fileDescriptor = (FILEDESCRIPTORW)Marshal.PtrToStructure(fileDescriptorPointer, typeof(FILEDESCRIPTORW));
                    //fileNames[fileDescriptorIndex] = fileDescriptor.cFileName;

                    AddFileDescriptorTab(TabCtrl, fileDescriptor);

                    //move the file descriptor pointer to the next file descriptor
                    fileDescriptorPointer = new IntPtr((long)fileDescriptorPointer + Marshal.SizeOf(fileDescriptor));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        static void AddFileDescriptorTab(TabControl tabCtrl, FILEDESCRIPTORW? desc)
        {
            if (desc == null)
                return;

            TabHelper.AddStringListTab(tabCtrl, desc.cFileName, new string[]
            {
                $"FileName: {desc.cFileName}",
                $"FileSize: {GetFileSizeString(desc.nFileSizeHigh, desc.nFileSizeLow)}",
                "",
                $"dwFlags: {FlagstoString(desc.dwFileAttributes)}",
                $"clsid: {desc.clsid}",
                $"sizel: ({desc.sizel.cx},{desc.sizel.cy})",
                $"pointl: ({desc.pointl.x},{desc.pointl.y})",
                $"dwFileAttributes: {FileAttributestoString(desc.dwFileAttributes)}",
                $"ftCreationTime: {GetTimeString(desc.ftCreationTime)}",
                $"ftLastAccessTime: {GetTimeString(desc.ftLastAccessTime)}",
                $"ftLastWriteTime: {GetTimeString(desc.ftLastWriteTime)}",
                $"nFileSizeHigh: {desc.nFileSizeHigh.ToString()}",
                $"nFileSizeLow: {desc.nFileSizeLow.ToString()}",
                $"cFileName: {desc.cFileName}"});
        }

        public static void Handle(TabControl ParentTab, IDataObject dropData)
        {
            if (dropData.GetDataPresent("FileGroupDescriptorW"))
            {
                //TabControl childTabCtrl = AddFileGroupTab(ParentTab);
                TabControl childTabCtrl = TabHelper.AddSubTab(ParentTab, "FileGroupDescriptorW");
                MemoryStream fileGroupStream = (MemoryStream)dropData.GetData("FileGroupDescriptorW");
                TabHelper.AddRawDataTab(childTabCtrl, fileGroupStream);
                //AddRawDataTab(childTabCtrl, fileGroupStream);
                ProcessFileDescriptor(childTabCtrl, fileGroupStream);
                fileGroupStream.Close();
            }
        }
    }
}
