using System;
using System.Linq;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class NewUserWindow : Window
    {
        public NewUserWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string email = txtEmail.Text.Trim();
            string nom = txtNom.Text.Trim();
            string password = txtPassword.Password.Trim();
            string confirmPassword = txtConfirmPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new DataContext())
            {
                // Création de l'utilisateur
                Guid userId = Guid.NewGuid();
                string encryptedPassword = classFonctionGenerale.CrypterChaine(password); // Utilisez votre méthode de cryptage

                var newUser = new Utilisateurs
                {
                    ID = userId,
                    Nom = nom,
                    Login = login,
                    Email = email,
                    // Ajoutez d'autres propriétés si nécessaire
                };
                context.Utilisateurs.Add(newUser);

                // Création du profil associé à l'utilisateur (ajoutez des vérifications si nécessaire pour lier à des dossiers existants)
                var newProfilsData = new ProfilsData
                {
                    ID = Guid.NewGuid(),
                    UtilisateursID = userId,
                    DossiersID = Guid.NewGuid(), // Ceci devrait être ajusté pour relier à un dossier existant si nécessaire
                    Nom = nom, // Ajustez selon votre besoin
                    URL = "", // Ajustez selon votre besoin
                    Login = login,
                    EncryptedPassword = encryptedPassword
                };
                context.ProfilsData.Add(newProfilsData);

                context.SaveChanges();
            }

            MessageBox.Show("Utilisateur enregistré avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
