using System.Windows;
using System.Windows.Controls;

namespace Drag_DropDebugger.UI
{
    public class StackedDataTab : TabItem
    {
        List<UIElement> mChildren;
        StackPanel mPanel;
        public StackedDataTab(string label)
        {
            mChildren = new List<UIElement>();
            Header = label;
            Height = 20;
            VerticalAlignment = VerticalAlignment.Top;
            mPanel = new StackPanel();
            AddChild(mPanel);
        }

        public void AddDataGrid(string label, Dictionary<String, Object> data, int indexPos = -1)
        {
            GroupBox groupBox = new GroupBox()
            {
                Header = label,
                Content = new DictionaryGridView(data),
                Margin = new Thickness(0, 0, 0, 10)
            };

            if (!(indexPos >= 0 && indexPos < mChildren.Count))
            {
                indexPos = mChildren.Count;
            }

            mChildren.Insert(indexPos, groupBox);



            mPanel.Children.Clear();
            for(int i =  0; i < mChildren.Count; i++)
            {
                mPanel.Children.Add(mChildren[i]);
            }
        }
    }
}
