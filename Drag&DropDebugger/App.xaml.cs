using System.Configuration;
using System.Data;
using System.Windows;
using WpfHexaEditor;
using System.IO;

namespace Drag_DropDebugger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static List<HexEditor> mHexEditors = new List<HexEditor>();
        public static List<string> mFilePathBuffer = new List<string>();
        protected override void OnExit(ExitEventArgs e)
        {
            for (int i = 0; i < mHexEditors.Count; i++)
            {
                mHexEditors[i].CloseProvider();
            }
           
            base.OnExit(e);

            for (int i = 0; i < mFilePathBuffer.Count; i++)
            {
                File.Delete(mFilePathBuffer[i]);
            }
        }
    }
}
