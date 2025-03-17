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

        private void Window_Drop(object sender, DragEventArgs e)
        {
            TabControl tabCtrl = TabHelper.AddSubTab(tabControlParent, "Drop Data #" + ++DropDataCount);

            string[] formats = e.Data.GetFormats();
            TabHelper.AddStringListTab(tabCtrl, "Summary", formats);
            
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
                        else if(formats[i] == "Shell IDList Array")
                        {
                            dynamic filedrop = e.Data.GetData(formats[i]);
                            DataHandlers.ShellIDListArray.Handle(tabCtrl, filedrop, formats[i]);
                        }
                        else
                        {
                            dynamic filedrop = e.Data.GetData(formats[i]);
                            if (filedrop is MemoryStream)
                            {
                                TabHelper.AddRawDataTab(tabCtrl, ((MemoryStream)filedrop).ToArray(), formats[i]);
                            }
                            else if(filedrop is string[])
                            {
                                TabHelper.AddStringListTab(tabCtrl, "String Data", filedrop);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TabHelper.AddStringTab(tabCtrl, formats[i], ex.Message);
                    }
                }
            }

            tabControlParent.SelectedIndex = tabControlParent.Items.Count - 1;
        }
    }
}