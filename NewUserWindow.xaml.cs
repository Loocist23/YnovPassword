using System;
using System.Linq;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

// Déclaration de l'espace de noms 'YnovPassword'
namespace YnovPassword
{
    // Déclaration partielle de la classe NewUserWindow héritant de Window
    public partial class NewUserWindow : Window
    {
        // Constructeur de la fenêtre NewUserWindow
        public NewUserWindow()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événements pour le bouton de sauvegarde
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupération et trim des valeurs des champs de texte
            string sLogin = txtLogin.Text.Trim();
            string sEmail = txtEmail.Text.Trim();
            string sNom = txtNom.Text.Trim();
            string sPassword = txtPassword.Password.Trim();
            string sConfirmPassword = txtConfirmPassword.Password.Trim();

            // Vérification que tous les champs sont remplis
            if (string.IsNullOrEmpty(sLogin) || string.IsNullOrEmpty(sEmail) ||
                string.IsNullOrEmpty(sNom) || string.IsNullOrEmpty(sPassword) ||
                string.IsNullOrEmpty(sConfirmPassword))
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validation des mots de passe (doivent correspondre et être d'une longueur minimale de 8 caractères)
            if (!classFonctionGenerale.ValiderMotdepasse(sPassword, sConfirmPassword))
            {
                MessageBox.Show("Les mots de passe ne correspondent pas ou ne respectent pas les critères de sécurité (minimum 8 caractères).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Utilisation d'un contexte de base de données
            using (var dcContext = new DataContext())
            {
                // Vérifiez si le dossier existe
                var dDossier = dcContext.Dossiers.FirstOrDefault(d => d.Nom == classConstantes.sTypeprofilConnection_Nom_YnovPassword);

                // Si le dossier n'existe pas, affichez un message d'erreur et arrêtez l'opération
                if (dDossier == null)
                {
                    MessageBox.Show("Le dossier spécifié n'existe pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Création de l'utilisateur
                Guid gUserId = Guid.NewGuid(); // Génération d'un nouvel ID pour l'utilisateur
                string sEncryptedPassword = classFonctionGenerale.CrypterChaine(sPassword); // Cryptage du mot de passe

                // Création d'une nouvelle instance de l'utilisateur
                var uNewUser = new Utilisateurs
                {
                    ID = gUserId,
                    Nom = sNom,
                    Login = sLogin,
                    Email = sEmail,
                    // Ajoutez d'autres propriétés si nécessaire
                };

                // Ajout de l'utilisateur au contexte
                dcContext.Utilisateurs.Add(uNewUser);

                // Création du profil associé à l'utilisateur
                var pdNewProfilsData = new ProfilsData
                {
                    ID = Guid.NewGuid(), // Génération d'un nouvel ID pour le profil
                    UtilisateursID = gUserId, // Association de l'utilisateur
                    DossiersID = dDossier.ID, // Utilisation de l'ID du dossier existant
                    Nom = sNom, // Nom du profil (ajustez selon votre besoin)
                    URL = "", // URL du profil (ajustez selon votre besoin)
                    Login = sLogin,
                    EncryptedPassword = sEncryptedPassword // Mot de passe crypté
                };

                // Ajout du profil au contexte
                dcContext.ProfilsData.Add(pdNewProfilsData);

                // Sauvegarde des changements dans la base de données
                dcContext.SaveChanges();
            }

            // Affichage d'un message de succès et fermeture de la fenêtre
            MessageBox.Show("Utilisateur enregistré avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        // Gestionnaire d'événements pour le bouton de fermeture
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Gestionnaire d'événements pour le bouton d'aide
        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }
    }
}
