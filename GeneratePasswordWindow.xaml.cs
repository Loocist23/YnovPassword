using System;
using System.Linq;
using System.Text;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class GeneratePasswordWindow : Window
    {
        public string sGeneratedPassword { get; private set; }

        public GeneratePasswordWindow()
        {
            InitializeComponent();
            sGeneratedPassword = string.Empty; // Initialize sGeneratedPassword
            LengthSlider.ValueChanged += LengthSlider_ValueChanged; // Ensure event handler is attached after initialization
            LengthSlider_ValueChanged(null, null); // Initialize the TextBlock with the default slider value
        }

        private void GeneratePassphrase_Checked(object sender, RoutedEventArgs e)
        {
            LengthSlider.Minimum = 3;
            LengthSlider.Maximum = 16;
            LengthSlider.Value = 4; // default passphrase length
        }

        private void GeneratePassphrase_Unchecked(object sender, RoutedEventArgs e)
        {
            LengthSlider.Minimum = 8;
            LengthSlider.Maximum = 256;
            LengthSlider.Value = 16; // default password length
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GeneratePassphrase.IsChecked == true)
                {
                    sGeneratedPassword = GeneratePassphraseMethod((int)LengthSlider.Value);
                }
                else
                {
                    sGeneratedPassword = GeneratePassword((int)LengthSlider.Value, IncludeSpecialChars.IsChecked == true, IncludeNumbers.IsChecked == true);
                }

                GeneratedPasswordTextBox.Text = sGeneratedPassword;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private string GeneratePassword(int iLength, bool bIncludeSpecialChars, bool bIncludeNumbers)
        {
            const string sLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string sNumbers = "0123456789";
            const string sSpecialChars = "!@#$%^&*()_-+=<>?";

            StringBuilder sbCharSet = new StringBuilder(sLetters);
            if (bIncludeNumbers)
            {
                sbCharSet.Append(sNumbers);
            }
            if (bIncludeSpecialChars)
            {
                sbCharSet.Append(sSpecialChars);
            }

            Random rRandom = new Random();
            return new string(Enumerable.Repeat(sbCharSet.ToString(), iLength).Select(s => s[rRandom.Next(s.Length)]).ToArray());
        }

        private string GeneratePassphraseMethod(int iWordCount)
        {
            using (var dcContext = new DataContext())
            {
                var lwWords = dcContext.Dictionnaires.Select(d => d.Mot).ToList();
                if (lwWords.Count < iWordCount)
                {
                    throw new InvalidOperationException("Pas assez de mots dans le dictionnaire pour générer une passphrase.");
                }

                Random rRandom = new Random();
                var lwSelectedWords = lwWords.OrderBy(x => rRandom.Next()).Take(iWordCount).ToArray();
                var sPassphrase = string.Join("+", lwSelectedWords) + rRandom.Next(0, 10);

                return sPassphrase;
            }
        }

        private void LengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PasswordLengthTextBlock != null)
            {
                PasswordLengthTextBlock.Text = LengthSlider.Value.ToString("N0");
            }
        }
        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }
    }
}
