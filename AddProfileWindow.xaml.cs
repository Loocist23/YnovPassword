using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;
using System.Collections.ObjectModel;

namespace YnovPassword
{
    public partial class AddProfileWindow : Window
    {
        private ObservableCollection<Dossiers> _dossiers;
        private Guid _userId;

        public event EventHandler<ProfilsData> ProfileAdded;

        public AddProfileWindow(Guid userId)
        {
            InitializeComponent();
            _userId = userId;
            _dossiers = new ObservableCollection<Dossiers>(); // Initialize _dossiers
            LoadDossiers();
        }

        private void LoadDossiers()
        {
            using (var context = new DataContext())
            {
                _dossiers = new ObservableCollection<Dossiers>(
                    context.Dossiers.Where(d => d.Nom != classConstantes.sTypeprofilConnection_Nom_YnovPassword).ToList()
                );

                if (_dossiers.Count == 0)
                {
                    MessageBox.Show("Aucun dossier disponible. Veuillez créer un dossier d'abord.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CreateDossierWindow createDossierWindow = new CreateDossierWindow();
                    if (createDossierWindow.ShowDialog() == true && createDossierWindow.CreatedDossier != null)
                    {
                        _dossiers.Add(createDossierWindow.CreatedDossier);
                    }
                }

                // Assurez-vous que la propriété Nom de la classe Dossiers est définie
                DossierComboBox.DisplayMemberPath = "Nom"; // Spécifiez ici le nom de la propriété dans Dossiers à afficher
                DossierComboBox.ItemsSource = _dossiers;
                if (_dossiers.Count > 0)
                {
                    DossierComboBox.SelectedIndex = 0;
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string nom = NomTextBox.Text.Trim();
            string url = UrlTextBox.Text.Trim();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();

            if (string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || DossierComboBox.SelectedItem == null)
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedDossier = DossierComboBox.SelectedItem as Dossiers;
            if (selectedDossier == null)
            {
                MessageBox.Show("Veuillez sélectionner un dossier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new DataContext())
            {
                string encryptedPassword = classFonctionGenerale.CrypterChaine(password);

                var newProfil = new ProfilsData
                {
                    ID = Guid.NewGuid(),
                    UtilisateursID = _userId,
                    DossiersID = selectedDossier.ID,
                    Nom = nom,
                    URL = url,
                    Login = login,
                    EncryptedPassword = encryptedPassword
                };
                context.ProfilsData.Add(newProfil);
                context.SaveChanges();

                ProfileAdded?.Invoke(this, newProfil);
            }

            MessageBox.Show("Profil ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratePasswordWindow generatePasswordWindow = new GeneratePasswordWindow();
            if (generatePasswordWindow.ShowDialog() == true)
            {
                PasswordTextBox.Text = generatePasswordWindow.GeneratedPassword;
            }
        }
    }
}
