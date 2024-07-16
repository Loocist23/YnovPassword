using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;
using System.Collections.ObjectModel;

// Déclaration de l'espace de noms 'YnovPassword'
namespace YnovPassword
{
    // Déclaration partielle de la classe AddProfileWindow héritant de Window
    public partial class AddProfileWindow : Window
    {
        // Variables privées
        private ObservableCollection<Dossiers> _ocDossiers;
        private Guid _gUserId;

        // Événement déclenché lorsqu'un profil est ajouté
        public event EventHandler<ProfilsData> ProfileAdded;

        // Constructeur de la classe AddProfileWindow
        public AddProfileWindow(Guid gUserId)
        {
            InitializeComponent();
            _gUserId = gUserId;
            _ocDossiers = new ObservableCollection<Dossiers>(); // Initialiser _ocDossiers
            LoadDossiers();
        }

        // Méthode pour charger les dossiers
        private void LoadDossiers()
        {
            using (var dcContext = new DataContext())
            {
                // Charger les dossiers qui ne sont pas du type "YNOVPASSWORD"
                _ocDossiers = new ObservableCollection<Dossiers>(
                    dcContext.Dossiers.Where(d => d.Nom != classConstantes.sTypeprofilConnection_Nom_YnovPassword).ToList()
                );

                // Si aucun dossier n'est disponible, afficher un message d'erreur et créer un dossier
                if (_ocDossiers.Count == 0)
                {
                    MessageBox.Show("Aucun dossier disponible. Veuillez créer un dossier d'abord.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CreateDossierWindow cdCreateDossierWindow = new CreateDossierWindow();
                    if (cdCreateDossierWindow.ShowDialog() == true && cdCreateDossierWindow.dCreatedDossier != null)
                    {
                        _ocDossiers.Add(cdCreateDossierWindow.dCreatedDossier);
                    }
                }

                // Assurer que la propriété Nom de la classe Dossiers est définie
                DossierComboBox.DisplayMemberPath = "Nom"; // Spécifiez ici le nom de la propriété dans Dossiers à afficher
                DossierComboBox.ItemsSource = _ocDossiers;
                if (_ocDossiers.Count > 0)
                {
                    DossierComboBox.SelectedIndex = 0;
                }
            }
        }

        // Méthode appelée lors du clic sur le bouton Ajouter
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les valeurs des champs de texte
            string sNom = NomTextBox.Text.Trim();
            string sUrl = UrlTextBox.Text.Trim();
            string sLogin = LoginTextBox.Text.Trim();
            string sPassword = PasswordTextBox.Text.Trim();

            // Vérifier que tous les champs sont remplis et qu'un dossier est sélectionné
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
                // Crypter le mot de passe
                string sEncryptedPassword = classFonctionGenerale.CrypterChaine(sPassword);

                // Créer un nouvel objet ProfilsData
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

                // Ajouter le nouveau profil à la base de données et sauvegarder les changements
                dcContext.ProfilsData.Add(pdNewProfil);
                dcContext.SaveChanges();

                // Déclencher l'événement ProfileAdded
                ProfileAdded?.Invoke(this, pdNewProfil);
            }

            // Afficher un message de succès et fermer la fenêtre
            MessageBox.Show("Profil ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        // Méthode appelée lors du clic sur le bouton Générer mot de passe
        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratePasswordWindow gpGeneratePasswordWindow = new GeneratePasswordWindow();
            if (gpGeneratePasswordWindow.ShowDialog() == true)
            {
                PasswordTextBox.Text = gpGeneratePasswordWindow.sGeneratedPassword;
            }
        }

        // Méthode appelée lors du clic sur le bouton d'aide
        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }
    }
}
