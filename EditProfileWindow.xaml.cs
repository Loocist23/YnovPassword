using System;
using System.Windows;
using YnovPassword.modele;
using YnovPassword.general;

// Déclaration de l'espace de noms 'YnovPassword'
namespace YnovPassword
{
    // Déclaration partielle de la classe EditProfileWindow héritant de Window
    public partial class EditProfileWindow : Window
    {
        // Variable privée pour stocker le profil à éditer
        private ProfilsData _pdProfil;

        // Constructeur de la classe EditProfileWindow
        public EditProfileWindow(ProfilsData pdProfil)
        {
            InitializeComponent();
            _pdProfil = pdProfil;
            LoadProfileData();
        }

        // Méthode pour charger les données du profil dans les champs de texte
        private void LoadProfileData()
        {
            NomTextBox.Text = _pdProfil.Nom;
            URLTextBox.Text = _pdProfil.URL;
            LoginTextBox.Text = _pdProfil.Login;
            PasswordTextBox.Password = classFonctionGenerale.DecrypterChaine(_pdProfil.EncryptedPassword);
        }

        // Méthode appelée lors du clic sur le bouton Enregistrer
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mettre à jour les propriétés du profil avec les valeurs des champs de texte
                _pdProfil.Nom = NomTextBox.Text;
                _pdProfil.URL = URLTextBox.Text;
                _pdProfil.Login = LoginTextBox.Text;
                _pdProfil.EncryptedPassword = classFonctionGenerale.CrypterChaine(PasswordTextBox.Password);

                // Mettre à jour le profil dans la base de données
                using (var dcContext = new DataContext())
                {
                    dcContext.ProfilsData.Update(_pdProfil);
                    dcContext.SaveChanges();
                }

                // Afficher un message de succès et fermer la fenêtre
                MessageBox.Show("Profil mis à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                // Gérer les erreurs en utilisant la méthode de gestion des erreurs
                classFonctionGenerale.GestionErreurLog(ex, "Erreur lors de la mise à jour du profil", false);
            }
        }

        // Méthode appelée lors du clic sur le bouton Annuler
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Méthode appelée lors du clic sur le bouton Générer mot de passe
        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratePasswordWindow gpGeneratePasswordWindow = new GeneratePasswordWindow();
            if (gpGeneratePasswordWindow.ShowDialog() == true)
            {
                PasswordTextBox.Password = gpGeneratePasswordWindow.sGeneratedPassword;
            }
        }

        // Méthode appelée lors du clic sur le bouton d'aide
        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }

    }
}
