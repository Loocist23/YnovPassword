using System;
using System.Windows;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class AddProfileWindow : Window
    {
        private DataContext _context;

        public AddProfileWindow()
        {
            InitializeComponent();
            _context = new DataContext();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            /*var newProfile = new ProfilsData
            {
                ID = Guid.NewGuid(),
                Nom = NomTextBox.Text,
                URL = UrlTextBox.Text,
                Login = LoginTextBox.Text,
                EncryptedPassword = PasswordTextBox.Text,
                UtilisateursID = // Assurez-vous d'assigner un utilisateur ID valide ici
                DossiersID = // Assurez-vous d'assigner un dossier ID valide ici
            };

            _context.ProfilsData.Add(newProfile);
            _context.SaveChanges();
            MessageBox.Show("Profil ajouté avec succès !");
            this.Close();*/
        }
    }
}
