using System.Linq;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = this.txtUsername.Text.Trim(); // Récupère et nettoie le nom d'utilisateur du TextBox.
            string password = this.txtPassword.Password.Trim(); // Récupère et nettoie le mot de passe du PasswordBox.

            // Vérifie si l'un des champs est vide.
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Le nom d'utilisateur et le mot de passe ne peuvent pas être vides.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Stoppe l'exécution si l'un des champs est vide.
            }

            if (ValidateUser(username, password))
            {
                MainWindow mainWindow = new MainWindow(); // Crée une instance de la fenêtre principale.
                mainWindow.Show(); // Affiche la fenêtre principale.
                this.Close(); // Ferme la fenêtre de connexion.
            }
            else
            {
                // Affiche un message si les identifiants sont incorrects.
                MessageBox.Show("Identifiant ou mot de passe incorrect.", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateUser(string username, string password)
        {
            bool isUserValid;
            using (DataContext dataContext = new DataContext()) // Initialise le contexte de données.
            {
                // Vérifie si un utilisateur avec le login et le mot de passe crypté correspondant existe.
                isUserValid = dataContext.Utilisateurs.Any(user =>
                    user.Login == username &&
                    user.ProfilsData.Single().EncryptedPassword == classFonctionGenerale.CrypterChaine(password));
            }
            return isUserValid; // Retourne vrai si l'utilisateur est valide, faux autrement.
        }

        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            NewUserWindow newUserWindow = new NewUserWindow();
            newUserWindow.ShowDialog(); // Utilisez ShowDialog pour une fenêtre modale
        }

    }
}
