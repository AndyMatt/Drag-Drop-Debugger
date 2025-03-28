using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using WpfHexaEditor.Core;
using WpfHexaEditor;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static Drag_DropDebugger.MainWindow;
using System.Windows.Data;
using System.Reflection;
using System.Reflection.PortableExecutable;
using Drag_DropDebugger.UI;
using TheArtOfDev.HtmlRenderer.WPF;

namespace Drag_DropDebugger.Helpers
{
    internal class TabHelper
    {

        public static TabControl AddSubTab(TabControl parentTab, string header)
        {
            TabItem newTab = new TabItem()
            {
                Header = header,
                //Background = ,
            };
            Grid newGrid = new Grid()
            {
                //Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0xE5, 0xE5)),
                Margin = new Thickness(5.0, 5.0, 5.0, 0.0),
            };

            TabControl childTabCtrl = new TabControl();

            newTab.Content = newGrid;
            newGrid.Children.Add(childTabCtrl);

            parentTab.Items.Add(newTab);

            return childTabCtrl;
        }

        public static TabItem AddRawDataTab(TabControl tabCtrl, MemoryStream stream, string header = "Raw")
        {
            return AddRawDataTab(tabCtrl, stream.ToArray(), header);
        }

        public static TabItem AddRawDataTab(TabControl tabCtrl, byte[] bytes)
        {
            return AddRawDataTab(tabCtrl, bytes, "Raw");
        }

        public static TabItem AddRawDataTab(TabControl tabCtrl, byte[] bytes, string Header)
        {
            HexEditor newHex = new HexEditor()
            {
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
            newTab.Header = Header;
            newTab.Content = newHex;
            App.mHexEditors.Add(newHex);

            tabCtrl.Items.Add(newTab);

            string tempFileName = Path.GetTempPath() + Path.GetRandomFileName();
            App.mFilePathBuffer.Add(tempFileName);

            File.WriteAllBytes(tempFileName, bytes);
            newHex.FileName = tempFileName;

            return newTab;
        }

        public static TabItem AddStringTab(TabControl tabCtrl, string Label, string data)
        {
            ListBox listBox = new ListBox()
            {
                Margin = new Thickness(5.0, 0, 0, 0)
            };
            listBox.KeyDown += ListBox_KeyDown;
            listBox.Items.Add(data);


            Grid grid = new Grid();

            grid.Children.Add(listBox);

            TabItem newTab = new TabItem()
            {
                Header = Label,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Content = grid
            };

            tabCtrl.Items.Add(newTab);
            return newTab;
        }

        public static TabItem AddStringListTab(TabControl tabCtrl, string Label, string[] data, int indexPos = -1)
        {
            ListBox listBox = new ListBox()
            {
                Margin = new Thickness(5.0, 0, 0, 0)
            };

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == "")
                {
                    listBox.Items.Add(new ListBoxItem() { IsEnabled = false });
                    listBox.Items.Add(new ListBoxItem() { IsEnabled = false });
                }
                else
                {
                    listBox.Items.Add(data[i]);
                }
            }
            listBox.KeyDown += ListBox_KeyDown;
            Grid grid = new Grid();

            grid.Children.Add(listBox);

            TabItem newTab = new TabItem()
            {
                Header = Label,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Content = grid
            };

            if (indexPos == -1 || tabCtrl.Items.Count == 0)
            {
                tabCtrl.Items.Add(newTab);
            }
            else
            {
                tabCtrl.Items.Insert(indexPos, newTab);
            }

            return newTab;
        }

        public static TabItem AddBitmapTab(TabControl tabCtrl, string Label, BitmapSource bitmap, int indexPos = -1)
        {
            Image bitmapImage = new Image()
            {
                Source = bitmap,
                Width = bitmap.Width,
                Height = bitmap.Height,
            };

            DrawingBrush BackgroundPNG = new DrawingBrush()
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, 32, 32),
                ViewportUnits = BrushMappingMode.Absolute,
                Drawing = new GeometryDrawing()
                {
                    Geometry = Geometry.Parse("M0,0 H1 V1 H2 V2 H1 V1 H0Z"),
                    Brush = new SolidColorBrush(Colors.LightGray),
                }
            };

            Grid grid = new Grid()
            {
                Background = BackgroundPNG,
                Children = { bitmapImage }
            };

            TabItem newTab = new TabItem()
            {
                Header = Label,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Content = grid,
            };

            if (indexPos == -1 || tabCtrl.Items.Count == 0)
            {
                tabCtrl.Items.Add(newTab);
            }
            else
            {
                tabCtrl.Items.Insert(indexPos, newTab);
            }
            return newTab;
        }

        public static TabItem AddDataGridTab(TabControl tabCtrl, string Label, Dictionary<String, Object> data, int indexPos = -1)
        {
            DictionaryGridView dataGrid = new DictionaryGridView(data);

            Grid grid = new Grid()
            {
                Children = { dataGrid }
            };

            TabItem newTab = new TabItem()
            {
                Header = Label,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Content = grid
            };

            if (indexPos == -1 || tabCtrl.Items.Count == 0)
            {
                tabCtrl.Items.Add(newTab);
            }
            else
            {
                tabCtrl.Items.Insert(indexPos, newTab);
            }

            return newTab;
        }

        public static TabItem AddHTMLTab(TabControl tabCtrl, string Label, String html, int indexPos = -1)
        {
            HtmlPanel htmlPanel = new HtmlPanel()
            {
                Text = html
            };

            Grid grid = new Grid()
            {
                Children = { htmlPanel }
            };

            TabItem newTab = new TabItem()
            {
                Header = Label,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Content = grid
            };

            if (indexPos == -1 || tabCtrl.Items.Count == 0)
            {
                tabCtrl.Items.Add(newTab);
            }
            else
            {
                tabCtrl.Items.Insert(indexPos, newTab);
            }

            return newTab;

        }

        private static void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
            {
                StringBuilder copy_buffer = new StringBuilder();
                foreach (object item in ((ListBox)sender).SelectedItems)
                    copy_buffer.AppendLine(item.ToString());
                if (copy_buffer.Length > 0)
                    Clipboard.SetText(copy_buffer.ToString());
            }
        }
    }
}
