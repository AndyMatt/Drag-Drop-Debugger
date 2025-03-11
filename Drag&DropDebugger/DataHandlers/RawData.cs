using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfHexaEditor;
using WpfHexaEditor.Core;

namespace Drag_DropDebugger.DataHandlers
{
    internal class RawData
    {
        static HexEditor AddRawDataTab(TabControl tabCtrl, MemoryStream fileGroupStream, string typeName)
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
            newTab.Header = typeName;
            newTab.Content = newHex;
            App.mHexEditors.Add(newHex);

            tabCtrl.Items.Add(newTab);

            string tempFileName = Path.GetTempPath() + Path.GetRandomFileName();
            App.mFilePathBuffer.Add(tempFileName);

            FileStream tempFile = File.Create(tempFileName);
            fileGroupStream.Position = 0;
            fileGroupStream.CopyTo(tempFile);
            tempFile.Close();

            newHex.FileName = tempFileName;

            return newHex;
        }
        public static void Handle(TabControl ParentTab, MemoryStream dropData, string typeName)
        {
            AddRawDataTab(ParentTab, dropData, typeName);
        }
    }
}
