using Drag_DropDebugger.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.DataHandlers
{
    internal class FileOpFlagsHander
    {
        //Extracted from shellapi.h
        private enum Flags : uint
        {
            FOF_MULTIDESTFILES = 0x001,
            FOF_CONFIRMMOUSE = 0x0002,
            FOF_SILENT = 0x0004,  // don't display progress UI (confirm prompts may be displayed still)
            FOF_RENAMEONCOLLISION = 0x0008,  // automatically rename the source files to avoid the collisions
            FOF_NOCONFIRMATION = 0x0010,  // don't display confirmation UI, assume "yes" for cases that can be bypassed, "no" for those that can not
            FOF_WANTMAPPINGHANDLE = 0x0020,  // Fill in SHFILEOPSTRUCT.hNameMappings
            FOF_ALLOWUNDO = 0x0040,  // enable undo including Recycle behavior for IFileOperation::Delete()
            FOF_FILESONLY = 0x0080,  // only operate on the files (non folders), both files and folders are assumed without this
            FOF_SIMPLEPROGRESS = 0x0100,  // means don't show names of files
            FOF_NOCONFIRMMKDIR = 0x0200,  // don't dispplay confirmatino UI before making any needed directories, assume "Yes" in these cases
            FOF_NOERRORUI = 0x0400,  // don't put up error UI, other UI may be displayed, progress, confirmations
            FOF_NOCOPYSECURITYATTRIBS = 0x0800,  // dont copy file security attributes (ACLs)
            FOF_NORECURSION = 0x1000,  // don't recurse into directories for operations that would recurse
            FOF_NO_CONNECTED_ELEMENTS = 0x2000,  // don't operate on connected elements ("xxx_files" folders that go with .htm files)
            FOF_WANTNUKEWARNING = 0x4000,  // during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
            FOF_NORECURSEREPARSE = 0x8000,  // deprecated; the operations engine always does the right thing on FolderLink objects (symlinks, reparse points, folder shortcuts)
            FOF_NO_UI = 0x0614,  //(FOF_SILENT | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_NOCONFIRMMKDIR),  // don't display any UI at all
        }
        private static string FlagstoString(uint _flags)
        {
            string flagStr = "( 0x" + _flags.ToString("X").PadLeft(8, '0') + ")";
            if((_flags & (uint)Flags.FOF_NO_UI) == (uint)Flags.FOF_NO_UI)
            {
                flagStr += "| FOF_NO_UI";
                _flags = _flags | (uint)Flags.FOF_NO_UI;
            }
            foreach (Flags _flag in Enum.GetValues(typeof(Flags)))
            {
                flagStr += ((_flags & (uint)_flag) != 0) ? " | " + Enum.GetName(typeof(Flags), _flag) : "";
            }
            return flagStr;
        }

        public object? mTabReference;

        public FileOpFlagsHander(TabControl ParentTab, object dropData, string dropType)
        {
            if (dropData is MemoryStream)
            {
                ByteReader byteReader = new ByteReader(((MemoryStream)dropData).ToArray());
                uint flags = byteReader.read_uint();
                mTabReference = FlagstoString(flags);
            }
        }
    }
}
