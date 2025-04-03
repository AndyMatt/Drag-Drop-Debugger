using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Xml.Xsl;
using System.Xml;
using System.Windows.Markup;
using System.Drawing;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace Drag_DropDebugger.UI
{
    internal class HtmlTextBox : RichTextBox
    {
        enum TextColorState
        {
            PlainText,
            HTMLTag,
            Property,
            PropertyValue,
            Comment,
            NoState,
        }

        TextColorState mState;
        public string Text
        {
            get
            {
                return GetText();
            }
            set
            {
                SetText(value);
            }
        }

        Dictionary<TextColorState, System.Windows.Media.Color> StateColors = new Dictionary<TextColorState, System.Windows.Media.Color>
        {
            { TextColorState.PlainText, Colors.Black },
            { TextColorState.HTMLTag, Colors.Blue },
            { TextColorState.Property, Colors.Crimson },
            { TextColorState.PropertyValue, Colors.DarkOrchid },
            { TextColorState.Comment, Colors.ForestGreen }
        };

        public HtmlTextBox()
        {
            mState = TextColorState.PlainText;
            this.IsReadOnly = true;
        }

        public void SetText(string htmlText)
        {
            Process(htmlText);
        }

        private string GetText()
        {
            return new TextRange(Document.ContentStart, Document.ContentEnd).Text;
        }

        void Process(string html)
        {
            html = html.Replace("\r\n", "\r").Replace("\n", "\r").Replace("\r\r", "\r");
            
            string CurrentBuffer = "";
            foreach (char c in html)
            {
                if (c == '\r')
                {
                    CurrentBuffer = PrintText(CurrentBuffer, mState);
                    PrintEndLine();
                    continue;
                }

                CurrentBuffer += c;

                switch (mState)
                {
                    case TextColorState.PlainText:
                        if (CurrentBuffer.EndsWith("<"))
                        {
                            PrintText(CurrentBuffer.Substring(0, CurrentBuffer.Length - 1), TextColorState.PlainText);
                            CurrentBuffer = "<";
                            mState = TextColorState.HTMLTag;
                        }
                        break;

                    case TextColorState.HTMLTag:
                        if (CurrentBuffer.StartsWith("<!--"))
                        {
                            mState = TextColorState.Comment;
                        }
                        else if (CurrentBuffer.StartsWith("<") && CurrentBuffer.EndsWith(" "))
                        {
                            CurrentBuffer = PrintText(CurrentBuffer, TextColorState.HTMLTag);
                            mState = TextColorState.Property;
                        }
                        else if (CurrentBuffer.StartsWith("<") && CurrentBuffer.EndsWith(">"))
                        {
                            CurrentBuffer = PrintText(CurrentBuffer, TextColorState.HTMLTag);
                            mState = TextColorState.PlainText;
                        }
                        break;

                    case TextColorState.Property:
                        if (CurrentBuffer.EndsWith("="))
                        {
                            CurrentBuffer = PrintText(CurrentBuffer.Substring(0, CurrentBuffer.Length - 1), TextColorState.Property);
                            PrintText("=", TextColorState.PlainText);
                            mState = TextColorState.PropertyValue;
                        }
                        else if (CurrentBuffer.EndsWith(">"))
                        {
                            CurrentBuffer = PrintText(CurrentBuffer, TextColorState.HTMLTag);
                            mState = TextColorState.PlainText;
                        }
                        break;

                    case TextColorState.PropertyValue:
                        if (CurrentBuffer.Length > 1 && CurrentBuffer.StartsWith("\"") && CurrentBuffer.EndsWith("\""))
                        {
                            CurrentBuffer = PrintText(CurrentBuffer, TextColorState.PropertyValue);
                            mState = TextColorState.Property;
                        }
                        break;

                    case TextColorState.Comment:
                        if (CurrentBuffer.EndsWith("-->"))
                        {
                            CurrentBuffer = PrintText(CurrentBuffer, TextColorState.Comment);
                            mState = TextColorState.PlainText;
                        }
                        break;
                }
            }
        }

        private void PrintEndLine()
        {
            TextRange textRange = new TextRange(Document.ContentEnd, Document.ContentEnd);
            textRange.Text = "\r";
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(StateColors[TextColorState.PlainText]));
        }

        private string PrintText(string text, TextColorState state)
        {
            TextRange textRange = new TextRange(Document.ContentEnd, Document.ContentEnd);
            textRange.Text = text;
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(StateColors[state]));
            return "";
        }
    } 
}
