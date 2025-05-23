using Drag_DropDebugger.Helpers;
using Drag_DropDebugger.UI;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Drag_DropDebugger.DataHandlers.ShellIDListArray;

namespace Drag_DropDebugger.Items
{
    internal static class ShellItemHandler
    {
        enum SortIndex : byte
        {
            InternetExplorer = 0x00,
            Libraries = 0x42,
            Users = 0x44,
            MyDocuments = 0x48,
            MyComputer = 0x50,
            Network = 0x58,
            RecycleBin = 0x60,
            IExplorer = 0x68,
            Unknown = 0x70,
            MyGames = 0x80,
        }

        static Dictionary<Guid, Type> ClassIDs = new Dictionary<Guid, Type>()
        {
            {new Guid("20D04FE0-3AEA-1069-A2D8-08002B30309D"), typeof(MyComputerShellItem)},
            {new Guid("4234D49B-0245-4DF3-B780-3893943456E1"), typeof(ApplicationShellItem)},
            {new Guid("59031A47-3F72-44A7-89C5-5595FE6B30EE"), typeof(UserFolderShellItem)},
            {new Guid("24AD3AD4-A569-4530-98E1-AB02F9417AA8"), typeof(UserFolderShellItem)},
            {new Guid("088e3905-0323-4b02-9826-5d99428e115f"), typeof(UserFolderShellItem)},
            {new Guid("1cf1260c-4dd0-4ebb-811f-33c572699fde"), typeof(UserFolderShellItem)},
            {new Guid("a8cdff1c-4878-43be-b5fd-f8091c1c60d0"), typeof(UserFolderShellItem)},
            {new Guid("b4bfcc3a-db2c-424c-b029-7fe99a87c641"), typeof(UserFolderShellItem)},
            {new Guid("374de290-123f-4565-9164-39c4925e467b"), typeof(UserFolderShellItem)},
            {new Guid("3add1653-eb32-4cb0-bbd7-dfa0abb5acca"), typeof(UserFolderShellItem)},
            {new Guid("a0953c92-50dc-43bf-be83-3742fed03c9c"), typeof(UserFolderShellItem)},
        };

        public static object? Handle(TabControl parentTab, ByteReader byteReader, out TabControl? tabCtrl)
        {            
            byte indicator = byteReader.scan_byte(sizeof(ushort));

            if (indicator == 0x1f)
            {
                ushort size = byteReader.read_ushort();
                indicator = byteReader.read_byte();
                SortIndex sortIndex = (SortIndex)byteReader.read_byte();
                Guid classID = byteReader.read_guid();

                if (ClassIDs.ContainsKey(classID))
                {
                    Type ClassType = ClassIDs[classID];

                    TabControl childTab = TabHelper.AddSubTab(parentTab, ClassType.Name);
                    StackedDataTab stackedDataTab = new StackedDataTab("Properties");

                    dynamic? shellItem;

                    if (ClassType == typeof(UserFolderShellItem))
                    {

                        shellItem = new UserFolderShellItem(childTab, ref stackedDataTab, classID, byteReader);
                    }
                    else
                    {
                        shellItem = Activator.CreateInstance(ClassType, childTab, stackedDataTab, byteReader);
                    }

                    stackedDataTab.AddDataGrid("Header", new Dictionary<string, object>()
                    {
                        {"Size", $"{size} (0x{size.ToString("X")})"},
                        {"Indicator", indicator},
                        {"SortIndex", sortIndex},
                        {"GUID", classID.ToString()},
                    }, 0, 0);

                    TabHelper.AddStackTab(childTab, stackedDataTab, 0);

                    tabCtrl = childTab;

                    return shellItem;

                }
                else
                {
                    TabHelper.AddStringTab(parentTab, "MissingGuid", $"GIUD {{{classID.ToString()}}} is Missing");
                }
            }
            else if (indicator == 0x2F)
            {
                TabControl childTab = TabHelper.AddSubTab(parentTab, "RootFolderShellItem");
                tabCtrl = childTab;
                return new RootFolderShellItem(childTab, byteReader);
            }
            else if (indicator == 0x32)
            {
                TabControl childTab = TabHelper.AddSubTab(parentTab, "FileEntryShellItem");
                tabCtrl = childTab;
                return new FileEntryShellItem(childTab, byteReader);
            }
            else if (indicator == 0x74)
            {
                TabControl childTab = TabHelper.AddSubTab(parentTab, "DelegateFolderShellItem");
                tabCtrl = childTab;
                return new DelegateFolderShellItem(childTab, byteReader);
            }

            tabCtrl = null;
            return null;

        }
    }
}
