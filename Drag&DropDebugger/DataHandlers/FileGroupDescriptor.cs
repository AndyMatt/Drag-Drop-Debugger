using System;
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
        public enum FILE_DESCRIPTOR_FLAGS : uint
        {
            CLSID = 0x1,
            SIZEPOINT = 0x2,
            ATTRIBUTES = 0x4,
            CREATETIME = 0x8,
            ACCESSTIME = 0x10,
            WRITESTIME = 0x20,
            FILESIZE = 0x40,
            PROGRESSUI = 0x4000,
            LINKUI = 0x8000,
            UNICODE = 0x80000000,
        }

        public enum FILE_ATTRIBUTE_FLAGS : uint
        {
            READONLY = 0x1,
            HIDDEN = 0x2,
            SYSTEM = 0x4,
            DIRECTORY = 0x10,
            ARCHIVE = 0x20,
            DEVICE = 0x40,
            NORMAL = 0x80,
            TEMPORARY = 0x100,
            SPARSE_FILE = 0x200,
            REPARSE_POINT = 0x400,
            COMPRESSED = 0x800,
            OFFLINE = 0x1000,
            NOT_CONTENT_INDEXED = 0x2000,
            ENCRYPTED = 0x4000,
            INTEGRITY_STREAM = 0x8000,
            VIRTUAL = 0x10000,
            NO_SCRUB_DATA = 0x20000,
            EA_OR_RECALL_ON_OPEN = 0x40000,
            PINNED = 0x80000,
            UNPINNED = 0x100000,
            RECALL_ON_DATA_ACCESS = 0x400000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class POINTL
        {
            public int x;
            public int y;

            public override string ToString()
            {
                return $"({x},{y})";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class SIZEL
        {
            public int cx;
            public int cy;

            public override string ToString()
            {
                return $"({cx},{cy})";
            }
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
            public FILE_DESCRIPTOR_FLAGS dwFlags;
            public Guid clsid;
            public SIZEL sizel;
            public POINTL pointl;
            public FILE_ATTRIBUTE_FLAGS dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
        }

        //https://stackoverflow.com/questions/58120399/c-sharp-drag-and-drop-from-outlook-using-net-4-5-or-later
        static void ProcessFileDescriptor(TabControl TabCtrl, byte[] fileGroupDescriptorBytes)
        {
            IntPtr fileGroupDescriptorWPointer = IntPtr.Zero;
            try
            {
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

            string[] propertyStrings = StringHelper.ClassToString(desc);
            TabHelper.AddStringListTab(tabCtrl, desc.cFileName, propertyStrings);
        }

        public TabControl mTabReference;
        public FileGroupDescriptor(TabControl ParentTab, MemoryStream dropData, string dropType)
        {
            mTabReference = TabHelper.AddSubTab(ParentTab, "FileGroupDescriptorW");
            byte[] bytes = dropData.ToArray();

            ProcessFileDescriptor(mTabReference, bytes);
            TabHelper.AddRawDataTab(mTabReference, bytes);
        }
    }
}
