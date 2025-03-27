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
    internal class FallbackHandler
    {
        public static object? Handle(TabControl ParentTab, object dropData, string dropType)
        {
            if (dropData is MemoryStream)
            {
                return TabHelper.AddRawDataTab(ParentTab, ((MemoryStream)dropData).ToArray(), dropType);
            }
            else if (dropData is string[])
            {
                string[] stringData = (string[])dropData;
                if (stringData.Length == 1)
                {
                    return stringData[0];
                }
                else
                {
                    return TabHelper.AddStringListTab(ParentTab, dropType, stringData);
                }
            }
            else if (dropData is string)
            {
                return (string)dropData;
            }

            return null;
        }
    }
}
