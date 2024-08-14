using System.Linq;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

// Déclaration de l'espace de noms 'YnovPassword'
namespace YnovPassword
{
    // Déclaration partielle de la classe LoginWindow héritant de Window
    public partial class LoginWindow : Window
    {
        // Constructeur de la classe LoginWindow
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Méthode appelée lors du clic sur le bouton Login
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les valeurs des champs de texte pour le nom d'utilisateur et le mot de passe
            string sUsername = this.txtUsername.Text.Trim();
            string sPassword = this.txtPassword.Password.Trim();

            // Vérifier que le nom d'utilisateur et le mot de passe ne sont pas vides
            if (string.IsNullOrEmpty(sUsername) || string.IsNullOrEmpty(sPassword))
            {
                MessageBox.Show("Le nom d'utilisateur et le mot de passe ne peuvent pas être vides.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Valider l'utilisateur et ouvrir la fenêtre principale si la validation réussit
            if (ValidateUser(sUsername, sPassword, out Guid gUserId))
            {
                App.gLoggedInUserId = gUserId; // Stocker l'ID utilisateur après une connexion réussie
                MainWindow mwMainWindow = new MainWindow();
                mwMainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Identifiant ou mot de passe incorrect.", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Méthode pour valider l'utilisateur
        private bool ValidateUser(string sUsername, string sPassword, out Guid gUserId)
        {
            gUserId = Guid.Empty;
            bool bIsUserValid;
            using (DataContext dcDataContext = new DataContext())
            {
                // Rechercher l'utilisateur avec le login et le mot de passe crypté correspondants
                var uUser = dcDataContext.Utilisateurs.FirstOrDefault(u =>
                    u.Login == sUsername &&
                    u.ProfilsData.Single().EncryptedPassword == classFonctionGenerale.CrypterChaine(sPassword));

                if (uUser != null)
                {
                    bIsUserValid = true;
                    gUserId = uUser.ID; // Récupérer l'ID utilisateur
                }
                else
                {
                    bIsUserValid = false;
                }
            }
            return bIsUserValid;
        }

        // Méthode appelée lors du clic sur le bouton Créer un utilisateur
        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            NewUserWindow nuNewUserWindow = new NewUserWindow();
            nuNewUserWindow.ShowDialog();
        }

        // Méthode appelée lors du clic sur le bouton d'aide
        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }

        private void txtUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
