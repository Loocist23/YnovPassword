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
            string sUsername = this.txtUsername.Text.Trim();
            string sPassword = this.txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(sUsername) || string.IsNullOrEmpty(sPassword))
            {
                MessageBox.Show("Le nom d'utilisateur et le mot de passe ne peuvent pas être vides.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

        private bool ValidateUser(string sUsername, string sPassword, out Guid gUserId)
        {
            gUserId = Guid.Empty;
            bool bIsUserValid;
            using (DataContext dcDataContext = new DataContext())
            {
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

        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            NewUserWindow nuNewUserWindow = new NewUserWindow();
            nuNewUserWindow.ShowDialog();
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }

    }
}
