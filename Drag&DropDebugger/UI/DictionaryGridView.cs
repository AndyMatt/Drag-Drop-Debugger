using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using Drag_DropDebugger.Helpers;

namespace Drag_DropDebugger.UI
{
    public static class VisualExtension
    {
        public static T? GetAncestorOfType<T>(this Visual element)
        where T : Visual => element != null ? GetAncestorOfType(element, typeof(T)) as T : null;

        public static Visual? GetAncestorOfType(this Visual element, Type type)
        {
            if (element != null && type != null)
            {
                (element as FrameworkElement)?.ApplyTemplate();
                if ((VisualTreeHelper.GetParent(element) is Visual parent))
                {
                    return type.IsInstanceOfType(parent) ? parent : GetAncestorOfType(parent, type);
                }
            }

            return type == null ? throw new ArgumentException(nameof(type)) : null;
        }
    }

    internal class DictionaryGridView : DataGrid
    {
        public class LinkValue
        {
            public String Key { get; set; }
            public String? Text { get; set; }
            public Object? mObject;
            public LinkValue(String key, Object? value) {
                Key = key;
                mObject = value;
                Text = "EMPTY";
               
                if (mObject != null)
                {
                    Text = mObject.ToString();
                    string _type = mObject.GetType().Name;

                    switch (_type)
                    {
                        case "StackedDataTab":
                        case "PropertySetTab":
                        case "TabItem":
                            Text = "...";
                            break;

                        case "TabControl":
                            Text = mObject.ToString();
                            TabItem? tabItem = GetTabItemFromControl((TabControl)mObject);
                            if (tabItem != null)
                            {
                                Text = "...";
                                mObject = tabItem;
                            }
                            break;
                        case "Guid":
                            Text = $"{{{mObject.ToString()}}}";
                            break;
                        case "UInt32":
                            if(key == "FileSize")
                            {
                                Text = StringHelper.GetFileSizeString((uint)mObject);
                            }
                            break;
                    }
                    return;
                }
            }

            TabItem? GetTabItemFromControl(TabControl tabCtrl)
            {
                if (tabCtrl.Parent != null && tabCtrl.Parent.GetType() == typeof(Grid))
                {
                    Grid grid = (Grid)tabCtrl.Parent;
                    if (grid.Parent.GetType() == typeof(TabItem))
                    {
                        return (TabItem)grid.Parent;
                    }
                }
                return null;
            }
        }
           
        public DictionaryGridView(Dictionary<String, Object> data)
        {
            IsReadOnly = true;
            HorizontalGridLinesBrush = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
            VerticalGridLinesBrush = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
            SelectionMode = DataGridSelectionMode.Single;
            SelectionUnit = DataGridSelectionUnit.Cell;
            Columns.Add(new DataGridTextColumn() { Header = "Property", Binding = new Binding("Key") });
            Columns.Add(new DataGridTextColumn() { Header = "Value", Binding = new Binding("Text") });

            for (int i = 0; i < data.Count; i++)
            {
                Items.Add(new LinkValue(data.ElementAt(i).Key, data.ElementAt(i).Value));
            }
        }

        public DictionaryGridView(List<KeyValuePair<string, object>> data)
        {
            IsReadOnly = true;
            HorizontalGridLinesBrush = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
            VerticalGridLinesBrush = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
            SelectionMode = DataGridSelectionMode.Single;
            SelectionUnit = DataGridSelectionUnit.Cell;
            Columns.Add(new DataGridTextColumn() { Header = "Property", Binding = new Binding("Key") });
            Columns.Add(new DataGridTextColumn() { Header = "Value", Binding = new Binding("Text") });

            for (int i = 0; i < data.Count; i++)
            {
                Items.Add(new LinkValue(data.ElementAt(i).Key, data.ElementAt(i).Value));
            }
        }

        private void SelectCell(MouseButtonEventArgs e)
        {
            SelectedCells.Clear();

            IInputElement element = this.InputHitTest(e.GetPosition(this));
            var cell = ((Visual)element).GetAncestorOfType<DataGridCell>();

            if (cell != null)
            {
                CurrentCell = new DataGridCellInfo(cell);
                SelectedCells.Add(CurrentCell);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (SelectedCells.Count > 0 && SelectedCells[0].Column.DisplayIndex == 1)
            {
                LinkValue link = (LinkValue)SelectedCells[0].Item;
                if (link.mObject != null)
                {
                    if (link.mObject.GetType() == typeof(TabItem) ||
                        link.mObject.GetType().IsSubclassOf(typeof(TabItem)))
                    {
                        TabItem tabItem = (TabItem)link.mObject;
                        TabControl tabControl = (TabControl)tabItem.Parent;
                        tabControl.SelectedValue = tabItem;
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            SelectCell(e);

            if (e.RightButton == MouseButtonState.Pressed)
            {
                ContextMenu cm = new ContextMenu()
                {
                    PlacementTarget = (UIElement)this.SelectedItem,
                    IsOpen = true,
                };
                Action<object> copyCell = delegate (object obj)
                {
                    if (SelectedCells.Count > 0)
                    {
                        object str = SelectedCells[0].Column.DisplayIndex == 0 ? ((dynamic)SelectedCells[0].Item).Key : ((dynamic)SelectedCells[0].Item).mObject;
                        Clipboard.SetText(str.ToString());
                    }
                };
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Copy";
                menuItem.Command = new CopyCommand(copyCell);

                cm.Items.Add(menuItem);
            }
        }

        public class CopyCommand : ICommand
        {
            private readonly Action<object?> execute;
            private readonly Func<object, bool> canExecute;

            public event EventHandler? CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public CopyCommand(Action<object?> execute, Func<object, bool> canExecute = null)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                return  canExecute == null || (parameter != null && canExecute != null ? canExecute(parameter) : true);
            }

            public void Execute(object? parameter)
            {
                execute(parameter);
            }
        }
    }
}
