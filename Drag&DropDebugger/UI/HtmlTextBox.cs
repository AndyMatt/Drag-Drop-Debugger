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
            HTMLTagStart,
            HTMLTag,
            HTMLTagEnd,
            Property,
            PropertyEquals,
            PropertyValueStart,
            PropertyValue,
            PropertyValueEnd,
            Comment,
            NoState,
        }

        int TabOffset = 0;
        TextColorState mState;
        TextColorState mNextState;

        Dictionary<TextColorState, System.Windows.Media.Color> StateColors = new Dictionary<TextColorState, System.Windows.Media.Color>
        {
            { TextColorState.PlainText, Colors.Black },
            { TextColorState.HTMLTagStart, Colors.Blue },
            { TextColorState.HTMLTag, Colors.Blue },
            { TextColorState.HTMLTagEnd, Colors.Blue },
            { TextColorState.Property, Colors.Crimson },
            { TextColorState.PropertyEquals, Colors.Black },
            { TextColorState.PropertyValueStart, Colors.DarkOrchid },
            { TextColorState.PropertyValue, Colors.DarkOrchid },
            { TextColorState.PropertyValueEnd, Colors.DarkOrchid },
            { TextColorState.Comment, Colors.ForestGreen }
        };

        public HtmlTextBox()
        {
            mState = TextColorState.PlainText;
            mNextState = TextColorState.NoState;

            this.IsReadOnly = true;
            //string testString = "Version:0.9\r\nStartHTML:0000000136\r\nEndHTML:0000000390\r\nStartFragment:0000000172\r\nEndFragment:0000000354\r\nSourceURL:about:blank#blocked\r\n<html>\r\n<body>\r\n<!--StartFragment--><img src=\"https://learn.microsoft.com/en-us/dotnet/media/art-color-table.png?view=windowsdesktop-9.0\" alt=\"Predefined colors\" title=\"Predefined colors\" data-linktype=\"relative-path\"><!--EndFragment-->\r\n</body>\r\n</html>";
            //string testString = "<!-- 11p76e0o0c5bu54plak -->\r\n<script>window.ue && ue.count && ue.count('CSMLibrarySize', 10217)</script>\r\n<!-- sp:end-feature:csm:head-open-part2 -->\r\n<!-- sp:feature:aui-assets -->\r\n<link rel=\"stylesheet\" href=\"https://m.media-amazon.com/images/I/11EIQ5IGqaL._RC|01ZTHTZObnL.css,01MLbr4kpjL.css,51FIeTurRAL.css,31fFxDf00KL.css,012SlIQp3XL.css,11D3BPoiHRL.css,01qDClimA1L.css,01pOTCa2wPL.css,413Vvv3GONL.css,11TIuySqr6L.css,01Rw4F+QU6L.css,11JJsNcqOIL.css,01J3raiFJrL.css,01IdKcBuAdL.css,014QJx7nWqL.css,21Otytu1xYL.css,01Sv7-fQIGL.css,51mfNuT7JUL.css,01XPHJk60-L.css,11ChJlzZQoL.css,01UgxIH-BSL.css,01fxuupJToL.css,21+W7u4fDzL.css,01oATFSeEjL.css,21RWaJb6t+L.css,11I+YZzE7kL.css,211Xmr7zN2L.css,01CFUgsA-YL.css,31WWobXdJQL.css,11PDZ29p-PL.css,111bsezNMhL.css,11tNhCU--0L.css,11msBd9oOTL.css,11BO1RWH3kL.css,011ylpySXkL.css,21aIYZpUYPL.css,11Wb9slw7JL.css,01uDrkI-EcL.css,215Q9RsWvdL.css,113EFChyAjL.css,11hvENnYNUL.css,11Qek6G6pNL.css,01890+Vwk8L.css,014VAMpg+ZL.css,01qiwJ7qDfL.css,21TAMzcrOKL.css,016mfgi+D2L.css,01gU3ljx0fL.css,21l8QuSB5IL.css,013-xYw+SRL.css_.css?AUIClients/AmazonUI#page_type-WebflowDetail.not-trident.991784-T1.1073045-T2.1074116-T2.1074121-T2.1074853-T1.1074849-T2\" />\r\n<link rel=\"preload\" as=\"script\" crossorigin=\"anonymous\" href=\"https://m.media-amazon.com/images/I/61xJcNKKLXL.js?AUIClients/AmazonUIjQuery\" />\r\n<link rel=\"preload\" as=\"script\" crossorigin=\"anonymous\" href=\"https://m.media-amazon.com/images/I/11zuylp74DL._RC|11Y+5x+kkTL.js,51F3LXOLEtL.js,11EeeaacI2L.js,11GgN1+C7hL.js,01+z+uIeJ-L.js,01VRMV3FBdL.js,21u+kGQyRqL.js,012FVc3131L.js,11aD5q6kNBL.js,11rRjDLdAVL.js,51zH7YD-TsL.js,11nAhXzgUmL.js,11dIAzUNpxL.js,1110g-SvlBL.js,116PwN2VXHL.js,21+WA5wfjfL.js,0190vxtlzcL.js,51xvEQZx5oL.js,01JYHc2oIlL.js,31nfKXylf6L.js,01ktRCtOqKL.js,01VIlNGiFCL.js,11bEz2VIYrL.js,31o2NGTXThL.js,01rpauTep4L.js,31N+6dLod0L.js,01tvglXfQOL.js,11+FwJUUPNL.js,014gnDeJDsL.js,11vb6P5C5AL.js,015+pUPweLL.js_.js?AUIClients/AmazonUI#372963-T1.1072613-T2\" />";
            string testString = "<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Sample HTML Page</title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n            margin: 20px;\r\n        }\r\n        header {\r\n            background-color: #f8f9fa;\r\n            padding: 10px;\r\n            text-align: center;\r\n        }\r\n        ul {\r\n            list-style-type: none;\r\n            padding: 0;\r\n        }\r\n        li {\r\n            background-color: #e9ecef;\r\n            margin: 5px 0;\r\n            padding: 10px;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <header>\r\n        <h1>Welcome to My Sample Page</h1>\r\n    </header>\r\n    <main>\r\n        <p>This is a simple HTML page to demonstrate basic structure and elements.</p>\r\n        <h2>Things I Like:</h2>\r\n        <ul>\r\n            <li>Reading</li>\r\n            <li>Traveling</li>\r\n            <li>Cooking</li>\r\n        </ul>\r\n    </main>\r\n</body>\r\n</html>\r\n";
            string testBuffer = "";
            string CurrentBuffer = "";
            bool newLine = false;
            bool closingTag = false;
            foreach(char c in testString)
            {
                if (mState == TextColorState.HTMLTagStart)
                {
                    if (c == '!')
                    {
                        CurrentBuffer += '<';
                        mState = TextColorState.Comment;
                    }
                    else
                    {
                        PrintText("<", TextColorState.HTMLTagStart);
                        mState = TextColorState.HTMLTag;
                        TabOffset++;

                    }
                }

                testBuffer += c;
                switch (c)
                {
                    case '<':
                        if (mState != TextColorState.Comment)
                        {
                            mNextState = TextColorState.HTMLTagStart;
                        }
                        break;

                    case '>':
                        if (mState != TextColorState.Comment)
                        {
                            mNextState = TextColorState.HTMLTagEnd;
                        }
                        else if(CurrentBuffer.EndsWith("--"))
                        {
                            mNextState = TextColorState.PlainText;
                        }
                        break;

                    case ' ':
                        if(mState == TextColorState.HTMLTag || mState == TextColorState.PropertyValueEnd)
                        {
                            mNextState = TextColorState.Property;
                        }
                        break;

                    case '/':
                        if (mState == TextColorState.Property)
                        {
                            mState = TextColorState.HTMLTag;
                            closingTag = true;
                        }
                        else if (mState == TextColorState.HTMLTag)
                        {
                            closingTag = true;
                        }
                        break;

                    case '\"':
                        
                        if (mState == TextColorState.PropertyValueStart)
                        {
                            mState = TextColorState.PropertyValue;
                        }
                        else if (mState == TextColorState.PropertyValue)
                        {
                            mState = TextColorState.PropertyValueEnd;
                        }
                        break;


                    case '=':
                        if (mState == TextColorState.Property)
                        {
                            mNextState = TextColorState.PropertyEquals;
                        }
                        break;

                    case '\n':
                        newLine = true;
                        break;

                    case '-':
                        if(CurrentBuffer.EndsWith("<!-"))
                        {
                            mState = TextColorState.Comment;
                            TabOffset--;
                        }
                        break;
                }

                if(!newLine && mNextState != TextColorState.PropertyEquals && mNextState != TextColorState.HTMLTagEnd && mNextState != TextColorState.HTMLTagStart)
                    CurrentBuffer +=  c;

                if(newLine || (mNextState != TextColorState.NoState && mState != mNextState))
                {
                    CurrentBuffer = PrintText(CurrentBuffer, mState, newLine);
                }

                if (mNextState == TextColorState.PropertyEquals)
                {
                    PrintText("=", TextColorState.PropertyEquals);
                    mState = TextColorState.PropertyValueStart;
                }
                else if (mNextState == TextColorState.HTMLTagEnd)
                {
                    PrintText(">", TextColorState.HTMLTagEnd);
                    if (closingTag)
                    {
                        TabOffset-=2;
                        closingTag = false;
                    }
                    mState = TextColorState.PlainText;
                }
                else if ((mNextState != TextColorState.NoState && mState != mNextState))
                {
                    mState = mNextState;
                }

                mNextState = TextColorState.NoState;
                newLine = false;
            }
        }

        private string PrintText(string text, TextColorState state, bool newLine = false)
        {
            TextRange textRange = new TextRange(Document.ContentEnd, Document.ContentEnd);
            textRange.Text = text;

            if (newLine)
                textRange.Text += getTabOffset();

            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(StateColors[state]));
            return "";
        }
          
        private string getTabOffset()
        {
            string result = "";
            for (int i = 0; i < TabOffset; i++) {
                result += "\t";
            }
            return result;
        }
    } 
}
