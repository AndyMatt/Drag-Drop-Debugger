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
        List<HexEditor> editors = new List<HexEditor>();
        static List<string> FilesToDelete = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            AllowDrop = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            for(int i = 0; i < editors.Count; i++)
            {
                editors[i].CloseProvider();
            }
            for (int i = 0; i < FilesToDelete.Count; i++)
            {
                File.Delete(FilesToDelete[i]);
            }

            base.OnClosed(e);
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

        void AddStringTab(TabControl tabCtrl, string DataType, string[] data)
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

        HexEditor AddNewDataTab(TabControl tabCtrl, string DataType, int TabIndex)
        {
            HexEditor newHex = new HexEditor()
            {
                Name = "DropData" + TabIndex.ToString(),
                Margin = new Thickness(2, 2, 2, 2),
                AllowByteCount = true,
                AllowCustomBackgroundBlock = true,
                AllowDrop = false,
                AllowExtend = false,
                AppendNeedConfirmation = true,
                BorderThickness = new Thickness(1.0),
                ByteGrouping = ByteSpacerGroup.EightByte,
                ByteSpacerPositioning = ByteSpacerPosition.HexBytePanel,
                ByteSpacerVisualStyle = ByteSpacerVisual.Empty,
                ByteSpacerWidthTickness = ByteSpacerWidth.VerySmall,
                BytePerLine = 32,
                DataStringVisual = DataVisualType.Hexadecimal,
                DefaultCopyToClipboardMode = CopyPasteMode.HexaString,
                ForegroundSecondColor = new SolidColorBrush(Colors.Blue),
                OffSetStringVisual = DataVisualType.Hexadecimal,
                PreloadByteInEditorMode = PreloadByteInEditor.None,
                VisualCaretMode = CaretMode.Overwrite
            };

            TabItem newTab = new TabItem();
            newTab.Header = DataType;
            newTab.Content = newHex;
            editors.Add(newHex);

            tabCtrl.Items.Add(newTab);

            return newHex;
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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct FILEDESCRIPTOR
        {
            public UInt32 dwFlags;
            public Guid clsid;
            public System.Drawing.Size sizel;
            public System.Drawing.Point pointl;
            public UInt32 dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public UInt32 nFileSizeHigh;
            public UInt32 nFileSizeLow;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String cFileName;
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

            for (int i = 0; i < formats.Length; i++)
            {
                if (e.Data.GetDataPresent(formats[i]))
                {
                    try
                    {
                        dynamic filedrop = e.Data.GetData(formats[i]);

                        if (filedrop is MemoryStream)
                        {
                            if (formats[i] == "FileGroupDescriptorW")
                            {
                                byte[] bytes = ((MemoryStream)filedrop).ToArray();
                                List<byte> filenameStr = new List<byte>();
                                int pos = 76;
                                do
                                {
                                    filenameStr.Add(bytes[pos]);
                                    filenameStr.Add(bytes[pos+1]);
                                    pos += 2;
                                } while (bytes[pos] != 0x0 | bytes[pos + 1] != 0x0);
                                var mystring = Encoding.Unicode.GetString(filenameStr.ToArray(), 0, filenameStr.Count);
                                SetFileTabName(tabCtrl, mystring);
                            }

                            HexEditor newHex = AddNewDataTab(tabCtrl, formats[i], i);
                            string tempFileName = Path.GetTempPath() + Path.GetRandomFileName();
                            FilesToDelete.Add(tempFileName);

                            FileStream tempFile = File.Create(tempFileName);
                            ((MemoryStream)filedrop).CopyTo(tempFile);
                            tempFile.Close();

                            newHex.FileName = tempFileName;
                        }

                        if (filedrop is string[])
                        {
                            AddStringTab(tabCtrl, formats[i], (string[])filedrop);
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