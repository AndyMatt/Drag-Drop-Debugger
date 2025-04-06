using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Drag_DropDebugger.UI
{
    public class StackedDataTab : TabItem
    {
        List<UIElement> mChildren;
        ScrollViewer mScrollViewer;
        StackPanel mPanel;
        public StackedDataTab(string label)
        {
            mChildren = new List<UIElement>();
            Header = label;
            Height = 20;
            VerticalAlignment = VerticalAlignment.Top;
            mPanel = new StackPanel()
            {
                CanVerticallyScroll = true,
            };

            mScrollViewer = new ScrollViewer()
            {
                Content = mPanel,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            
            AddChild(mScrollViewer);
        }

        private void AddUIElement(string label, object element, int indentcount = 0, int indexPos = -1)
        {
            GroupBox groupBox = new GroupBox()
            {
                Header = label,
                Content = element,
                Margin = new Thickness((indentcount*20), 0, 0, 10),
            };

            if (!(indexPos >= 0 && indexPos < mChildren.Count))
            {
                indexPos = mChildren.Count;
            }

            mChildren.Insert(indexPos, groupBox);

            mPanel.Children.Clear();
            for (int i = 0; i < mChildren.Count; i++)
            {
                mPanel.Children.Add(mChildren[i]);
            }
        }

        public void AddDataGrid(string label, Dictionary<String, Object> data, int indentCount = 0, int indexPos = -1)
        {
            AddUIElement(label, new DictionaryGridView(data), indentCount, indexPos);
        }

        public void AddDataGrid(string label, List<KeyValuePair<string, object>> data, int indentCount = 0, int indexPos = -1)
        {
            AddUIElement(label, new DictionaryGridView(data), indentCount, indexPos);
        }

        public void AddPreviewPanel(string label, BitmapSource bitmap, int indexPos = -1)
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
                Children = { bitmapImage },
                Height = bitmap.Height + 20,
                Width = bitmap.Width + 20
            };

            AddUIElement(label, grid, indexPos);
        }       
    }
}
