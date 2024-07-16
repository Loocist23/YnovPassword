using System;
using System.Windows;
using YnovPassword.modele;
using YnovPassword.general;

namespace YnovPassword
{
    public partial class EditProfileWindow : Window
    {
        private ProfilsData _pdProfil;

        public EditProfileWindow(ProfilsData pdProfil)
        {
            InitializeComponent();
            _pdProfil = pdProfil;
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            NomTextBox.Text = _pdProfil.Nom;
            URLTextBox.Text = _pdProfil.URL;
            LoginTextBox.Text = _pdProfil.Login;
            PasswordTextBox.Password = classFonctionGenerale.DecrypterChaine(_pdProfil.EncryptedPassword);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _pdProfil.Nom = NomTextBox.Text;
                _pdProfil.URL = URLTextBox.Text;
                _pdProfil.Login = LoginTextBox.Text;
                _pdProfil.EncryptedPassword = classFonctionGenerale.CrypterChaine(PasswordTextBox.Password);

                using (var dcContext = new DataContext())
                {
                    dcContext.ProfilsData.Update(_pdProfil);
                    dcContext.SaveChanges();
                }

                MessageBox.Show("Profil mis à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                classFonctionGenerale.GestionErreurLog(ex, "Erreur lors de la mise à jour du profil", false);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratePasswordWindow gpGeneratePasswordWindow = new GeneratePasswordWindow();
            if (gpGeneratePasswordWindow.ShowDialog() == true)
            {
                PasswordTextBox.Password = gpGeneratePasswordWindow.sGeneratedPassword;
            }
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }

    }
}
