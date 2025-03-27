using Drag_DropDebugger.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static Drag_DropDebugger.UI.DictionaryGridView;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace Drag_DropDebugger.UI
{
    public delegate void UpdateIndex(int index);

    public class PropertySetTab : TabItem
    {
        public event UpdateIndex onUpdateIndex;

        DockPanel mDockPanel;
        List<WindowsPropertySet> mPropertySets;
        PropertySetListBox mPropertyList;
        TabControl? mCurrentTabControl;
        int mCurrentIndex;
        public PropertySetTab()
        {
            mPropertySets = new List<WindowsPropertySet>();
          
            mDockPanel = new DockPanel()
            {
                LastChildFill = true,
                Margin = new System.Windows.Thickness(5)
            };

            mPropertyList = new PropertySetListBox() { 
                Margin = new System.Windows.Thickness(0, 0, 5, 0) 
            };
            mPropertyList.OnUpdateIndex += UpdateIndexView;
            mDockPanel.Children.Add(mPropertyList);

            Content = mDockPanel;
            mCurrentIndex = -1;
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            if(mPropertySets.Count > 1 && mPropertyList.SelectedIndex == -1)
            {
                mPropertyList.SelectedIndex = 0;
                UpdateIndexView(mPropertyList.SelectedIndex);
            }

            base.OnSelected(e);
        }

        void RefreshUI()
        {
            if (mPropertySets.Count > 1)
            {
                Header = "Property Sets";

                if (Content != mDockPanel)
                    Content = mDockPanel;

                UpdateIndexView(0);
            }
            else if(mPropertySets.Count == 1)
            {
                Header = mPropertySets[0].PropertySetName;
                Content = mPropertySets[0].mTabReference;
            }
        }

        public void AddPropertySet(WindowsPropertySet propertySet)
        {           
            mPropertyList.Items.Add(propertySet.PropertySetName);
            mPropertySets.Add(propertySet);
            RefreshUI();
        }

        private void UpdateIndexView(int index)
        {
            if (mCurrentIndex == index)
                return;

            if(mCurrentTabControl != null)
                mDockPanel.Children.Remove(mCurrentTabControl);

            mCurrentTabControl = (TabControl)mPropertySets[index].mTabReference;
            mDockPanel.Children.Add(mCurrentTabControl);
            mCurrentIndex = index;
        }
    }

    class PropertySetListBox : ListBox
    {
        public delegate void UpdateIndex(int index);
        public event UpdateIndex OnUpdateIndex;

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            OnUpdateIndex(SelectedIndex);
        }
    }
}
