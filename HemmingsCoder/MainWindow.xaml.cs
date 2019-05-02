using System;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HemmingsCoder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Regex binary = new Regex("^[01]{1,32}$", RegexOptions.Compiled);

        private string _text;

        private byte _error = 0;
        private bool _doubleErrorOnePos;

        private int[,] HMatrix =
        {
            {0,0,0,1,1,1,1 },
            {0,1,1,0,0,1,1 },
            {1,0,1,0,1,0,1 }
        };

        private void FirstButtClick(object sender, RoutedEventArgs e)
        {
            if (roundDoubleError.IsChecked != true && roundError.IsChecked != true) roundNoError.IsChecked = true;
            _error = 0;
            _doubleErrorOnePos = false;
            ConsoleBlock.Text = "";
            _text = EnterTextBox.Text;
            if (binary.IsMatch(_text) && _text.Length != 4) ConsoleBlock.Text += "Длинною в четыре бита, пожалуйста.\n";
            else if (binary.IsMatch(_text))
            {
                ConsoleBlock.Text += $"Введено число: {_text}\n\n";
                ConsoleBlock.Text += "Алгоритмы, что-то происходит, считается, кодер работает...\n\n";
                int[] Translation = CodeFourSymbols();                
                ConsoleBlock.Text += $"К числу добавляются проверочные символы и все это передается в канал: {string.Join(string.Empty, Translation)}\n\n";
                int checker = 0;
                for (int i = 0; i < Translation.Length; i++) checker += Translation[i];
                if (roundError.IsChecked == true)
                {
                    int errorPos;
                    if (int.TryParse(ErrorPossTextBox.Text, out errorPos)) { }
                    else if (ErrorPossTextBox.Text == "")
                    {
                        Random rand = new Random();
                        errorPos = rand.Next(0, Translation.Length);
                    }
                    else ConsoleBlock.Text += "Введите во втором поле адекватный номер позиции ошибки.\n\n";
                    if (errorPos < 1 || errorPos > 7) ConsoleBlock.Text += "Введите во втором поле адекватный номер позиции ошибки.\n\n";
                    else
                    {
                        if (Translation[errorPos - 1] == 0) Translation[errorPos - 1] = 1;
                        else Translation[errorPos - 1] = 0;
                    }
                    
                    _error = 1;
                    ConsoleBlock.Text += $"Появляется ошибка: {string.Join(string.Empty, Translation)}\n\n";
                }
                else if (roundDoubleError.IsChecked == true)
                {
                    int errorPos1;
                    int errorPos2;
                    if (int.TryParse(ErrorPossTextBox.Text, out int result))
                    {
                        try
                        {
                            errorPos1 =
                                int.Parse(ErrorPossTextBox.Text.Substring(0, ErrorPossTextBox.Text.Length - 1)) - 1;
                            errorPos2 = int.Parse(ErrorPossTextBox.Text.Substring(1)) - 1;
                            if (errorPos1 < 0 || errorPos2 < 0 || errorPos1 > 6 || errorPos2 > 6)
                                ConsoleBlock.Text += "Введите во втором поле адекватные номера позиции ошибок.\n\n";
                            else
                            {
                                if (errorPos1 == errorPos2) _doubleErrorOnePos = true;
                                if (Translation[errorPos1] == 1) Translation[errorPos1] = 0;
                                else Translation[errorPos1] = 1;
                                if (Translation[errorPos2] == 1) Translation[errorPos2] = 0;
                                else Translation[errorPos2] = 1;
                            }
                        }
                        catch
                        {
                            ConsoleBlock.Text += "Введите во втором поле два числа слитно.\n\n";
                        }
                    }
                    else if (ErrorPossTextBox.Text == "")
                    {
                        Random rand = new Random();
                        errorPos1 = rand.Next(0, Translation.Length);
                        errorPos2 = rand.Next(0, Translation.Length);
                        if (errorPos1 == errorPos2 && errorPos2 == Translation.Length - 1)
                        {
                            errorPos2 = errorPos1 - 1;
                            if (Translation[errorPos1] == 1) Translation[errorPos1] = 0;
                            else Translation[errorPos1] = 1;
                            if (Translation[errorPos2] == 1) Translation[errorPos2] = 0;
                            else Translation[errorPos2] = 1;
                        }
                        else if (errorPos1 == errorPos2)
                        {
                            errorPos2 = errorPos1 + 1;
                            if (Translation[errorPos1] == 1) Translation[errorPos1] = 0;
                            else Translation[errorPos1] = 1;
                            if (Translation[errorPos2] == 1) Translation[errorPos2] = 0;
                            else Translation[errorPos2] = 1;
                        }
                        else
                        {
                            if (Translation[errorPos1] == 1) Translation[errorPos1] = 0;
                            else Translation[errorPos1] = 1;
                            if (Translation[errorPos2] == 1) Translation[errorPos2] = 0;
                            else Translation[errorPos2] = 1;
                        }
                    }
                    else ConsoleBlock.Text += "Введите во втором поле адекватные номера позиции ошибок.\n\n";

                    _error = 2;

                    if (_doubleErrorOnePos) ConsoleBlock.Text += $"Типа появляются две ошибки, но они в одной позиции, поэтому одна исправляет другую, и никакой ошибки в итоге нет, и, ну, в общем понятно: {string.Join(string.Empty, Translation)}\n\n";
                    else ConsoleBlock.Text += $"Появляются две ошибки: {string.Join(string.Empty, Translation)}\n\n";

                }
                string decodedSymb = DecodeFourSymbols(Translation, checker);
                //if (_error == 1) ;
                //ConsoleBlock.Text +=
                //$"Декодер, аки умничка находит и устраняет одну ошибку и выдает нам комбинацию: {decodedSymb}\n\n";
                //else if (_error == 2)
                //    ConsoleBlock.Text +=
                //        $"Декодер пыхтит-старается, находит больше, чем одну ошибку и не может справиться со всеми: {decodedSymb}\n\n";
                //else
                //    ConsoleBlock.Text +=
                //        $"Декодер делает черную магию, не находит ошибки и убирает проверочные символы: {decodedSymb}\n\n";
            }
            else ConsoleBlock.Text += "Введите число в двоичной СС.\n";
        }

        private void NewMethod() {
            _doubleErrorOnePos = true;
        }

        private int[] CodeFourSymbols()
        {
            int[] VMatrixStart = {0,0,0,0,0,0,0};
            VMatrixStart[2] = int.Parse(_text.Substring(0, _text.Length - 3));
            VMatrixStart[4] = int.Parse(_text.Substring(1, _text.Length - 3));
            VMatrixStart[5] = int.Parse(_text.Substring(2, _text.Length - 3));
            VMatrixStart[6] = int.Parse(_text.Substring(3));

            int[] ParHNG = SdelatKrasivo(HMatrix, VMatrixStart);
            int[] VMatrixNew = VMatrixStart;
            VMatrixNew[0] = ParHNG[2];
            VMatrixNew[1] = ParHNG[1];
            VMatrixNew[3] = ParHNG[0];
            return VMatrixNew;

        }

        private string DecodeFourSymbols(int[] ComingV, int checker)
        {
            //int[] eithMatrix = {ComingV[0], ComingV[1], ComingV[2], ComingV[3], ComingV[4], ComingV[5], ComingV[6], 0};
            
            //if (checker % 2 != 0) eithMatrix[7] = 1;
            //ConsoleBlock.Text += $"При декодировании добавляется еще один проверочный символ: {string.Join(string.Empty, eithMatrix)}\n\n" +
            //                     $"И теперь, если сумма символов четна и наличествует ошибка, значит ошибка не одна. " +
            //                     $"Если же сумма символов нечетна, следовательно ошибка всего одна.\n\n";
            //int eithMatrixCheck = 0;
            //for (int i = 0; i < eithMatrix.Length; i++) eithMatrixCheck += eithMatrix[i];
            //int[] CheckMatrix = SdelatKrasivo(HMatrix, ComingV);
            //if (CheckMatrix[0] == 1 || CheckMatrix[1] == 1 || CheckMatrix[2] == 1)
            //{
            //    int error = CheckMatrix[0] * 4 + CheckMatrix[1] * 2 + CheckMatrix[2];
            //    if (ComingV[error] == 0) ComingV[error] = 1;
            //    else ComingV[error] = 0;
            //}
            int firstCheck = 0, secondCheck = 0, thirdCheck = 0;

            if ((ComingV[0] + ComingV[2] + ComingV[4] + ComingV[6]) % 2 != 0) firstCheck = 1;
            if ((ComingV[1] + ComingV[2] + ComingV[5] + ComingV[6]) % 2 != 0) secondCheck = 2;
            if ((ComingV[3] + ComingV[4] + ComingV[5] + ComingV[6]) % 2 != 0) thirdCheck = 4;
            if (firstCheck + secondCheck + thirdCheck != 0)
            {
                int errorPos = firstCheck + secondCheck + thirdCheck - 1;
                if (ComingV[errorPos] == 0) ComingV[errorPos] = 1;
                else ComingV[errorPos] = 0;
                if (_text != $"{ComingV[2]}{ComingV[4]}{ComingV[5]}{ComingV[6]}")
                    ConsoleBlock.Text +=
                        $"Декодер пыхтит-старается, думает, что правильно нашел и исправил ошибку, но на самом деле он ничего не может поделать с более, чем одной ошибкой, бедняга, простите его, он старался: {ComingV[2]}{ComingV[4]}{ComingV[5]}{ComingV[6]}\n";
                else
                    ConsoleBlock.Text +=
                        $"Декодер находит и исправляет ошибку, а затем убирает проверочные символы: {ComingV[2]}{ComingV[4]}{ComingV[5]}{ComingV[6]}\n\n";
            }
            else
                ConsoleBlock.Text +=
                    $"Декодер делает черную магию, не находит ошибки и убирает проверочные символы: {ComingV[2]}{ComingV[4]}{ComingV[5]}{ComingV[6]}\n\n";

            return $"{ComingV[2]}{ComingV[4]}{ComingV[5]}{ComingV[6]}";
        }


        public int[] SdelatKrasivo(int[,] A, int[] B)
        {
            int[,] M = new int[3,7];
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    M[i, j] = A[i, j] * B[j];
                }
            }

            int[] S = new int[3];
            for (int i = 0; i < M.GetLength(0); i++)
            {
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    S[i] += M[i, j];
                }
            }
            for (int i = 0; i < S.Length; i++)
            {
                if (S[i] % 2 != 0) S[i] = 1;
                else S[i] = 0;
            }
            return S;
        }


        private void EnterTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FirstButtClick(this, null);
            }
        }


        private void RoundDoubleError_OnChecked(object sender, RoutedEventArgs e)
        {
            if (roundNoError.IsChecked == true)
            {
                ErrorPossTextBox.Focusable = false;
                ErrorPossTextBox.Background = new SolidColorBrush(Colors.DarkGray);
                ErrorPossTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(112, 112, 112));
            }
            else
            {
                ErrorPossTextBox.Focusable = true;
                ErrorPossTextBox.Background = new SolidColorBrush(Colors.Honeydew);
                ErrorPossTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(32, 128, 186));
            }
        }
    }
}
