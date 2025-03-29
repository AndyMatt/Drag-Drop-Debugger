using Drag_DropDebugger.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Drag_DropDebugger.UI
{
    /* <TabControl x:Name="tabControlParent" Background="{x:Null}">
            <TabItem Header="Property Sets">
                <DockPanel LastChildFill="true" Margin="5,5,5,5">
                    <ListBox x:Name="listBox" d:ItemsSource="{d:SampleData ItemCount=5}" Margin="0,0,5,0"/>

                    <TabControl x:Name="tabControl">
                        <TabItem Header="TabItem">
                            <Grid Background="#FFE5E5E5"/>
                        </TabItem>
                        <TabItem Header="TabItem">
                            <Grid Background="#FFE5E5E5"/>
                        </TabItem>
                    </TabControl>

                </DockPanel>
            </TabItem>
        </TabControl>
    */

    public class PropertySetTab : TabItem
    {
        public TabControl mPropertyTabControl;
        DockPanel mDockPanel;
        List<WindowsPropertySet> mPropertySets;
        public PropertySetTab()
        {
            mPropertySets = new List<WindowsPropertySet>();
            mPropertyTabControl = new TabControl();
            mDockPanel = new DockPanel()
            {
                LastChildFill = true,
                Margin = new System.Windows.Thickness(5)
            };
        }

        void RefreshUI()
        {
            if (mPropertySets.Count > 1)
            {
                mDockPanel.Children.Clear();
                mPropertyTabControl.Items.Clear();

                ListBox mPropertyList = new ListBox() { Margin = new System.Windows.Thickness(0, 0, 5, 0) };

                for (int i = 0; i < mPropertySets.Count; i++)
                {
                    mPropertyList.Items.Add(mPropertySets[i].PropertySetName);
                    mPropertyTabControl.Items.Add(new TabItem() 
                    { 
                        Header = mPropertySets[i].PropertySetName,
                        Content = mPropertySets[i].mTabReference
                    }
                    );
                }

                mDockPanel.Children.Add(mPropertyList);
                mDockPanel.Children.Add(mPropertyTabControl);

                Header = "Property Sets";
                Content = mDockPanel;
            }
            else if(mPropertySets.Count == 1)
            {
                Header = mPropertySets[0].PropertySetName;
                Content = mPropertySets[0].mTabReference;
            }
        }

        public void AddPropertySet(WindowsPropertySet propertySet)
        {
            /*
            if(propertySet.mTabReference.GetType() == typeof(TabControl))
            {
                TabControl tabCtrl = (TabControl)propertySet.mTabReference;

                if(tabCtrl.Parent != null && tabCtrl.Parent.GetType() == typeof(TabItem))
                {
                    TabItem tabItem = (TabItem)tabCtrl.Parent;
                    tabItem.Content = null;
                }
            }*/

            /*mTabItem = new TabItem()
            {
                Header = propertySet.PropertySetName,
                Content = propertySet.mTabReference
            };*/
            mPropertySets.Add(propertySet);
            //AddChild(mTabItem);
            RefreshUI();
        }

        
    }
}
