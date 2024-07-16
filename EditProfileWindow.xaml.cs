using System;
using System.Windows;
using YnovPassword.modele;
using YnovPassword.general;

namespace YnovPassword
{
    public partial class EditProfileWindow : Window
    {
        private ProfilsData _profil;

        public EditProfileWindow(ProfilsData profil)
        {
            InitializeComponent();
            _profil = profil;
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            NomTextBox.Text = _profil.Nom;
            URLTextBox.Text = _profil.URL;
            LoginTextBox.Text = _profil.Login;
            PasswordTextBox.Password = classFonctionGenerale.DecrypterChaine(_profil.EncryptedPassword);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _profil.Nom = NomTextBox.Text;
                _profil.URL = URLTextBox.Text;
                _profil.Login = LoginTextBox.Text;
                _profil.EncryptedPassword = classFonctionGenerale.CrypterChaine(PasswordTextBox.Password);

                using (var context = new DataContext())
                {
                    context.ProfilsData.Update(_profil);
                    context.SaveChanges();
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
            GeneratePasswordWindow generatePasswordWindow = new GeneratePasswordWindow();
            if (generatePasswordWindow.ShowDialog() == true)
            {
                PasswordTextBox.Password = generatePasswordWindow.GeneratedPassword;
            }
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }

    }
}
