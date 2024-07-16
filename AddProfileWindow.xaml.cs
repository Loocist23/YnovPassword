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
        private ObservableCollection<Dossiers> _ocDossiers;
        private Guid _gUserId;

        public event EventHandler<ProfilsData> ProfileAdded;

        public AddProfileWindow(Guid gUserId)
        {
            InitializeComponent();
            _gUserId = gUserId;
            _ocDossiers = new ObservableCollection<Dossiers>(); // Initialize _ocDossiers
            LoadDossiers();
        }

        private void LoadDossiers()
        {
            using (var dcContext = new DataContext())
            {
                _ocDossiers = new ObservableCollection<Dossiers>(
                    dcContext.Dossiers.Where(d => d.Nom != classConstantes.sTypeprofilConnection_Nom_YnovPassword).ToList()
                );

                if (_ocDossiers.Count == 0)
                {
                    MessageBox.Show("Aucun dossier disponible. Veuillez créer un dossier d'abord.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CreateDossierWindow cdCreateDossierWindow = new CreateDossierWindow();
                    if (cdCreateDossierWindow.ShowDialog() == true && cdCreateDossierWindow.dCreatedDossier != null)
                    {
                        _ocDossiers.Add(cdCreateDossierWindow.dCreatedDossier);
                    }
                }

                // Assurez-vous que la propriété Nom de la classe Dossiers est définie
                DossierComboBox.DisplayMemberPath = "Nom"; // Spécifiez ici le nom de la propriété dans Dossiers à afficher
                DossierComboBox.ItemsSource = _ocDossiers;
                if (_ocDossiers.Count > 0)
                {
                    DossierComboBox.SelectedIndex = 0;
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string sNom = NomTextBox.Text.Trim();
            string sUrl = UrlTextBox.Text.Trim();
            string sLogin = LoginTextBox.Text.Trim();
            string sPassword = PasswordTextBox.Text.Trim();

            if (string.IsNullOrEmpty(sNom) || string.IsNullOrEmpty(sUrl) || string.IsNullOrEmpty(sLogin) || string.IsNullOrEmpty(sPassword) || DossierComboBox.SelectedItem == null)
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dSelectedDossier = DossierComboBox.SelectedItem as Dossiers;
            if (dSelectedDossier == null)
            {
                MessageBox.Show("Veuillez sélectionner un dossier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var dcContext = new DataContext())
            {
                string sEncryptedPassword = classFonctionGenerale.CrypterChaine(sPassword);

                var pdNewProfil = new ProfilsData
                {
                    ID = Guid.NewGuid(),
                    UtilisateursID = _gUserId,
                    DossiersID = dSelectedDossier.ID,
                    Nom = sNom,
                    URL = sUrl,
                    Login = sLogin,
                    EncryptedPassword = sEncryptedPassword
                };
                dcContext.ProfilsData.Add(pdNewProfil);
                dcContext.SaveChanges();

                ProfileAdded?.Invoke(this, pdNewProfil);
            }

            MessageBox.Show("Profil ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratePasswordWindow gpGeneratePasswordWindow = new GeneratePasswordWindow();
            if (gpGeneratePasswordWindow.ShowDialog() == true)
            {
                PasswordTextBox.Text = gpGeneratePasswordWindow.sGeneratedPassword;
            }
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }
    }
}
