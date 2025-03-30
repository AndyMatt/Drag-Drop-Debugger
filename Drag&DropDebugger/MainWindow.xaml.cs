using System.Windows;
using System.Windows.Controls;
using Drag_DropDebugger.Helpers;
using Drag_DropDebugger.DataHandlers;

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

        static Dictionary<string, Type> Handlers = new Dictionary<string, Type>()
        {
            {"FileGroupDescriptorW", typeof(FileGroupDescriptor)},
            {"Shell IDList Array", typeof(ShellIDListArray)},
            {"DragImageBits", typeof(DragImageBits)},
            {"FileOpFlags", typeof(FileOpFlagsHander)},
            {"Paste Succeeded", typeof(DropEffectHandler)},
            {"Preferred DropEffect", typeof(DropEffectHandler)},
            {"DragContext", typeof(DragContextHandler)},
            {"DragSourceHelperFlags", typeof(DragSourceHelperFlagsHandler)},
            {"UniformResourceLocatorW", typeof(HTMLHandler)},
            {"text/x-moz-url", typeof(HTMLHandler)},
            {"UniformResourceLocator", typeof(HTMLHandler)},
            {"chromium/x-renderer-taint", typeof(HTMLHandler)},
            {"text/html", typeof(HTMLHandler)},
        };
        private void Window_Drop(object sender, DragEventArgs e)
        {
            TabControl tabCtrl = TabHelper.AddSubTab(tabControlParent, "Drop Data #" + ++DropDataCount);

            string[] formats = e.Data.GetFormats();
            object[] values = new object[formats.Length];
            
            for (int i = 0; i < formats.Length; i++)
            {
                if (e.Data.GetDataPresent(formats[i]))
                {
                    try
                    {
                        dynamic filedrop = e.Data.GetData(formats[i]);
                        if (Handlers.ContainsKey(formats[i])) {
                            Type ClassType = Handlers[formats[i]];
                            dynamic handler = Activator.CreateInstance(ClassType, tabCtrl, filedrop, formats[i]);
                            values[i] = handler.mTabReference;
                        }
                        else
                        {
                            values[i] = FallbackHandler.Handle(tabCtrl, filedrop, formats[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        values[i] = $"Error {ex.Message}";
                    }
                }
            }

            Dictionary<String, Object> mSummaryData = new Dictionary<String, Object>();
            for (int i = 0; i < values.Length; i++)
            {
                mSummaryData.Add(formats[i], values[i]);
            }
            TabHelper.AddDataGridTab(tabCtrl, "Summary", mSummaryData, 0);
            tabControlParent.SelectedIndex = tabControlParent.Items.Count - 1;
        }
    }
}