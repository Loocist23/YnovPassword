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
        public string GeneratedPassword { get; private set; }

        public GeneratePasswordWindow()
        {
            InitializeComponent();
            GeneratedPassword = string.Empty; // Initialize GeneratedPassword
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
                    GeneratedPassword = GeneratePassphraseMethod((int)LengthSlider.Value);
                }
                else
                {
                    GeneratedPassword = GeneratePassword((int)LengthSlider.Value, IncludeSpecialChars.IsChecked == true, IncludeNumbers.IsChecked == true);
                }

                GeneratedPasswordTextBox.Text = GeneratedPassword;
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

        private string GeneratePassword(int length, bool includeSpecialChars, bool includeNumbers)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()_-+=<>?";

            StringBuilder charSet = new StringBuilder(letters);
            if (includeNumbers)
            {
                charSet.Append(numbers);
            }
            if (includeSpecialChars)
            {
                charSet.Append(specialChars);
            }

            Random random = new Random();
            return new string(Enumerable.Repeat(charSet.ToString(), length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GeneratePassphraseMethod(int wordCount)
        {
            using (var context = new DataContext())
            {
                var words = context.Dictionnaires.Select(d => d.Mot).ToList();
                if (words.Count < wordCount)
                {
                    throw new InvalidOperationException("Pas assez de mots dans le dictionnaire pour générer une passphrase.");
                }

                Random random = new Random();
                var selectedWords = words.OrderBy(x => random.Next()).Take(wordCount).ToArray();
                var passphrase = string.Join("+", selectedWords) + random.Next(0, 10);

                return passphrase;
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
