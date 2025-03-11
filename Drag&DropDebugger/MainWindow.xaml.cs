using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WpfHexaEditor.Core;
using WpfHexaEditor;
using Microsoft.Win32;
using System.Windows.Ink;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization.Formatters.Binary;
using WpfHexaEditor.Core.MethodExtention;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;

namespace Drag_DropDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static int DropDataCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            AllowDrop = true;
        }

        void AddSummaryTab(TabControl tabCtrl, string[] DataFormats)
        {
            ListBox listBox = new ListBox()
            {
                Margin = new Thickness(5.0, 0, 0, 0)
            };

            for (int i = 0; i < DataFormats.Length; i++)
            {
                listBox.Items.Add(DataFormats[i]);
            }

                
            Grid grid = new Grid();

            grid.Children.Add(new Label() {
                Content = "Formats in Drop Data",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5.0, 0, 0, 0) 
            });

            grid.Children.Add(listBox);

            TabItem newTab = new TabItem()
            {
                Header = "Summary",
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Content = grid
            };

            tabCtrl.Items.Add(newTab);
        }

        void AddErrorTab(TabControl tabCtrl, string DataType, string errorMsg)
        {
            ListBox listBox = new ListBox()
            {
                Margin = new Thickness(5.0, 0, 0, 0),
            };
            listBox.KeyDown += ListBox_KeyDown;

            listBox.Items.Add(errorMsg);


            Grid grid = new Grid();

            grid.Children.Add(new Label()
            {
                Content = "Issue Parsing Data:",
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

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ( (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                System.Text.StringBuilder copy_buffer = new System.Text.StringBuilder();
                foreach (object item in ((ListBox)sender).SelectedItems)
                    copy_buffer.AppendLine(item.ToString());
                if (copy_buffer.Length > 0)
                    Clipboard.SetText(copy_buffer.ToString());
            }
        }

        TabControl AddNewFileTab()
        {
            TabItem newTab = new TabItem();
            Grid newGrid = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0xE5, 0xE5)),
                Margin = new Thickness(5.0, 5.0, 5.0, 0.0),
            };

            TabControl TabCtrl = new TabControl();

            newTab.Content = newGrid;
            newGrid.Children.Add(TabCtrl);

            tabControlParent.Items.Add(newTab);

            return TabCtrl;
        }

        void SetFileTabName(TabControl tabCtrl, string name)
        {
            if (tabCtrl.Parent is Grid)
            {
                if (((Grid)tabCtrl.Parent).Parent is TabItem)
                {
                    ((TabItem)((Grid)tabCtrl.Parent).Parent).Header = name;
                }
            }
        }


        private void Window_Drop(object sender, DragEventArgs e)
        {
            TabControl tabCtrl = AddNewFileTab();

            string[] formats = e.Data.GetFormats();
            AddSummaryTab(tabCtrl, formats);
            SetFileTabName(tabCtrl, "Drop Data #" + ++DropDataCount);

            for (int i = 0; i < formats.Length; i++)
            {
                if (e.Data.GetDataPresent(formats[i]))
                {
                    try
                    {
                        if (formats[i] == "FileGroupDescriptorW")
                        {
                            DataHandlers.FileGroupDescriptor.Handle(tabCtrl, e.Data);
                        }
                        else
                        {
                            dynamic filedrop = e.Data.GetData(formats[i]);
                            if (filedrop is MemoryStream)
                            {
                                DataHandlers.RawData.Handle(tabCtrl, filedrop, formats[i]);
                            }
                            else if(filedrop is string[])
                            {
                                DataHandlers.StringArray.Handle(tabCtrl,filedrop, formats[i]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddErrorTab(tabCtrl, formats[i], ex.Message);
                    }
                }
            }

            tabControlParent.SelectedIndex = tabControlParent.Items.Count - 1;
        }
    }
}