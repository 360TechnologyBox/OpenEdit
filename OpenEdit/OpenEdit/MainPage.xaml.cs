using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OpenEdit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            richTxtCodeArea.IsSpellCheckEnabled = false;
            richTxtCodeArea.IsTextPredictionEnabled = true;
        }

        private async void richTxtCodeArea_KeyUp(object sender, KeyRoutedEventArgs e)
        {

            Exception myex = null;
            string errorme = "no error";
            try
            {
                HighlightKeyWords(JavaScriptkeyWords);
                Windows.UI.Popups.PopupMenu menu = new PopupMenu();
                //menu.ShowForSelectionAsync(GetElementRect)
                //var visual = richTxtCodeArea.TransformToVisual(null);
                //var point = visual.TransformPoint(new Point());
                //var point = Window.Current.CoreWindow.
                Point mypoint;
                string code;
                richTxtCodeArea.Document.GetText(TextGetOptions.None, out code);
                richTxtCodeArea.Document.GetRange(0,code.Length).GetPoint(HorizontalCharacterAlignment.Left, VerticalCharacterAlignment.Baseline, PointOptions.AllowOffClient, out mypoint);
                Rect r = new Rect(mypoint, new Size( 10 , 10 ));
                menu.Commands.Add(new UICommand("hello"));                
                await menu.ShowForSelectionAsync(r, Placement.Below);
                //await menu.ShowAsync(mypoint);                
                //await menu.ShowForSelectionAsync(r);
                 

                

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }            
        }

        string[] JavaScriptkeyWords = { "break", "case", "class", "catch", "const", "continue", "debugger", "default", "delete", "do", "else", "export", "extends", "finally", "for", "function", "if", "import", "in", "instanceof", "let", "new", "return", "super", "switch", "this", "throw", "try", "typeof", "var", "void", "while", "with", "yield" };

        public void HighlightKeyWords(string[] keyWords)
        {
            Dictionary<string,string> foundList = new Dictionary<string,string>();
            Dictionary<int, string> positionList = new Dictionary<int, string>();
            //getting code from the codeArea
            string code;
            richTxtCodeArea.Document.GetText(TextGetOptions.None, out code);

            //split the lines of code into array
            var lineArray = code.Split('\r');
            
            //split each line into array 
            string[][] jaggedLine = new string[lineArray.Length][];

            //split each word in the line into array
            for(int i = 0 ; i < lineArray.Length ; i++)
            {
                jaggedLine[i] = lineArray[i].Split(' ');
            }

            for(int i = 0  ; i < lineArray.Length ; i++)
            {
                for(int j = 0 ; j < jaggedLine[i].Length ; j++ )
                {
                    if (keyWords.Contains(jaggedLine[i][j]))
                        foundList.Add(i.ToString() + "," + j.ToString(), jaggedLine[i][j]);

                }
            }

            foreach(var item in foundList)
            {
                var lineDetails = item.Key.Split(',');
                var lineIndex = Convert.ToInt32(lineDetails[0]);
                var spaceIndex = Convert.ToInt32(lineDetails[1]);
                int charCount = 0;
                for(int i = 0 ; i < spaceIndex  ; i++ )
                {
                    charCount += jaggedLine[lineIndex][i].Length;
                }
                
                if(lineIndex > 0 )
                {
                    for(int i = 0 ; i < lineIndex ; i++)
                    {
                        foreach(var itemIn in jaggedLine[i])
                        {
                            charCount += itemIn.Length;
                        }
                        charCount += ( jaggedLine[i].Length -  1 ) ;
                    }
                    charCount += lineIndex + spaceIndex;
                }
                else
                    charCount += lineIndex + spaceIndex;
                
                positionList.Add(charCount, item.Value);
            }

            //highlight all keyword in the respective position
            foreach(var item in positionList)
            {
                ITextRange rangeOfKeyWord = richTxtCodeArea.Document.GetRange(item.Key, item.Key + item.Value.Length);
                rangeOfKeyWord.CharacterFormat.ForegroundColor = Windows.UI.Colors.Blue;

            }

        }
    }
}
