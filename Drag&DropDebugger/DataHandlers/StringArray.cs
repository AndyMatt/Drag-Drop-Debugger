using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Drag_DropDebugger.DataHandlers
{
    internal class StringArray
    {
        static void AddStringTab(TabControl tabCtrl, string[] data, string DataType)
        {
            ListBox listBox = new ListBox()
            {
                Margin = new Thickness(5.0, 0, 0, 0)
            };

            for (int i = 0; i < data.Length; i++)
            {
                listBox.Items.Add(data[i]);
            }


            Grid grid = new Grid();

            grid.Children.Add(new Label()
            {
                Content = "String Data:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5.0, 0, 0, 0)
            });

            grid.Children.Add(listBox);

            TabItem newTab = new TabItem()
            {
                Header = DataType,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Content = grid
            };

            tabCtrl.Items.Add(newTab);
        }
        public static void Handle(TabControl ParentTab, string[] data, string typeName)
        {
            AddStringTab(ParentTab, data, typeName);
        }
    }
}
