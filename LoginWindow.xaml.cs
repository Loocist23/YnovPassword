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
            string username = this.txtUsername.Text.Trim();
            string password = this.txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Le nom d'utilisateur et le mot de passe ne peuvent pas être vides.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ValidateUser(username, password, out Guid userId))
            {
                App.LoggedInUserId = userId; // Stocker l'ID utilisateur après une connexion réussie
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Identifiant ou mot de passe incorrect.", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateUser(string username, string password, out Guid userId)
        {
            userId = Guid.Empty;
            bool isUserValid;
            using (DataContext dataContext = new DataContext())
            {
                var user = dataContext.Utilisateurs.FirstOrDefault(u =>
                    u.Login == username &&
                    u.ProfilsData.Single().EncryptedPassword == classFonctionGenerale.CrypterChaine(password));

                if (user != null)
                {
                    isUserValid = true;
                    userId = user.ID; // Récupérer l'ID utilisateur
                }
                else
                {
                    isUserValid = false;
                }
            }
            return isUserValid;
        }

        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            NewUserWindow newUserWindow = new NewUserWindow();
            newUserWindow.ShowDialog();
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }

    }
}
