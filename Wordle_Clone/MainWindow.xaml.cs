using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;


namespace Wordle_Clone {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window {
        
        static readonly Random rnd = new();
        private static readonly string solPath = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        static readonly int lineCount = File.ReadLines(solPath + @"\wordsList.txt").Count();
        int currentRow = 0;
        int currentColumn = 0;
        char[] word = new char[5];

        string okk = new string("");
        string word2 = new string("");
        Dictionary<char, int> wordCharRepetitions = new();
        char[] guess = new char[5];
  

        

        public MainWindow() {
            InitializeComponent();
        }

        


        private void Main_Loaded(object sender, RoutedEventArgs e) {
            word = (File.ReadLines(solPath + @"\wordsList.txt").Skip(rnd.Next(0, lineCount)).Take(1).First()).ToCharArray();  // Pick the word that is randomly chosen by a value between 0 and file length.
            word2 = new string(word);
            for (int i = 0; i < word.Length; i++) {
                if (wordCharRepetitions.ContainsKey(word[i])) {
                    wordCharRepetitions[word[i]]++;
                }
                else {
                    wordCharRepetitions[word[i]] = 1;
                }
            }

            lblDebug.Content = new string(word);  // Debug Purposes

           
        }



        private void Main_KeyDown(object sender, KeyEventArgs e) {
            // Enter - Move onto the next row/line
            if (e.Key == Key.Enter) {
                if (currentColumn >= 5) {
                    word2 = new string(word);
                    GuessChecker(currentRow);
                    currentRow++;
                    currentColumn = 0;
                }
            }
            // Backspace - Deleting the character in the TextBox in the (previous) column; edge cases handled
            else if (e.Key == Key.Back) {
                if (currentColumn >= 5) { currentColumn = 4; }
                else if (currentColumn <= 0) { currentColumn = 0; }
                else { currentColumn--; }

                DeleteChar();
            }
            // If not past the last row and not Enter or Backspace pressed
            else if (currentRow <= 5) {
                if ((byte)e.Key <= 69 && (byte)e.Key >= 44) {
                    FillBox(e.Key.ToString()[0]);
                }
            }
        }

        private void FillBox(char character) {
            object txt = Guesses.FindName($"TR{currentRow}C{currentColumn}");
            if (txt is TextBlock txtBlock) {
                txtBlock.Text = character.ToString();
            }
            if (currentColumn <= 5) {
                currentColumn++;
            }
        }

        private void DeleteChar() {
            object txt = Guesses.FindName($"TR{currentRow}C{currentColumn}");
            if (txt is TextBlock txtBlock) {
                txtBlock.Text = "";
            }
        }

        /// <summary>
        /// Changes the background of the TextBlock(s) of the specified row
        /// </summary>
        /// <param name="row">The row whose TextBlock(s) will have their background(s) changed</param>

        private static void ChangeBackground(Border bor, string result) {
            if (result == "correct") {
                bor.Background = Colours.colours["orange"];
            }
            else if (result == "wrong spot") {
                bor.Background = Colours.colours["blue"];
            }
            else {
                bor.Background = Colours.colours["gray"];
            }
            bor.BorderThickness = new Thickness(0);
        }
        
        private void GuessChecker(int row) {


            okk = "";
            
            for (int i = 0; i < 5; i++) {
                Border bor = (Border)Guesses.FindName($"BR{row}C{i}");
                TextBlock txtBlock = (TextBlock)bor.FindName($"TR{row}C{i}");          //make guessing stirng
                char letter = Convert.ToChar(txtBlock.Text);;
                okk = okk + letter.ToString();
                
            }
            for (int i = 0; i < 5; i++)
            {
                if (word2.Contains(okk[i]) == true)
                {
                    string yes = new string(word2.IndexOf(okk[i]).ToString());
                    string noo = new string(okk.IndexOf(okk[i]).ToString());
                    string result = "";
                    if (okk[int.Parse(noo)] == word2[i])
                    {
                        Regex regex = new Regex(okk[int.Parse(yes)].ToString());
                        word2 = regex.Replace(word2, 1.ToString(), 1);
                       
                        result = "correct";
                        Border bor = (Border)Guesses.FindName($"BR{row}C{int.Parse(yes)}");       // see if ur srihgt
                      

                        ChangeBackground(bor, result);
                    }
                    else
                    {
                        
                        Border bor = (Border)Guesses.FindName($"BR{row}C{int.Parse(noo)}");
                        result = "wrong spot";
                        ChangeBackground(bor, result);
                    }
                }

                else if (word2.Contains(okk[i]) == false) { 
                
                    Border bor = (Border)Guesses.FindName($"BR{row}C{i}");
                    string result = "";
                    result = "wrong";
                    ChangeBackground(bor, result);

                }
             
            };


            string wordperm = new string(word);

            if (wordperm[0] != okk[0]) {
                Border bor = (Border)Guesses.FindName($"BR{row}C{0}");
                string result = "";
                result = "wrong";
                ChangeBackground(bor, result);
                MessageBoxResult ryes = MessageBox.Show("got ehre");

            }
            



        }
    }


    /// <summary>
    /// All the custom brushes to be used in the app
    /// </summary>

    public static class Colours {
        // Setting up the brushes
        private static readonly BrushConverter bc = new();
        public static readonly Dictionary<string, Brush> colours = new() {
            ["gray"] = ObjToBrush(bc.ConvertFrom("#3A3A3C")),

            ["blue"] = ObjToBrush(bc.ConvertFrom("#85C0F9")),
            ["orange"] = ObjToBrush(bc.ConvertFrom("#F5793A")),

            ["yellow"] = ObjToBrush(bc.ConvertFrom("#B59F3B")),
            ["green"] = ObjToBrush(bc.ConvertFrom("#538D4E")),

        };

        private static Brush ObjToBrush(object? o) {
            return o switch {
                Brush b => b,
                _ => throw new InvalidCastException()
            };
        }
    }
}
