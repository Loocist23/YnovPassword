using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YnovPassword.modele;
using System.Collections.ObjectModel;
using YnovPassword.general;

// Déclaration de l'espace de noms 'YnovPassword'
namespace YnovPassword
{
    // Déclaration partielle de la classe MainWindow héritant de Window
    public partial class MainWindow : Window
    {
        // Collections observables pour les données des profils
        private ObservableCollection<ProfilsData> _ocProfilsData = new ObservableCollection<ProfilsData>();
        private ObservableCollection<ProfilsData> _ocFilteredProfilsData = new ObservableCollection<ProfilsData>();
        private ProfilsData _pdSelectedProfile;

        // Constructeur de la classe MainWindow
        public MainWindow()
        {
            InitializeComponent();
            LoadProfilsData();
        }

        // Méthode pour charger les données des profils
        private void LoadProfilsData()
        {
            using (var dcContext = new DataContext())
            {
                var gUserId = App.gLoggedInUserId;
                _ocProfilsData = new ObservableCollection<ProfilsData>(dcContext.ProfilsData.Where(p => p.UtilisateursID == gUserId).ToList());
                _ocFilteredProfilsData = new ObservableCollection<ProfilsData>(_ocProfilsData);
                dataGridProfils.ItemsSource = _ocFilteredProfilsData;
            }
        }

        // Méthode appelée lorsque le texte dans la zone de recherche change
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string sFilter = SearchTextBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(sFilter))
            {
                _ocFilteredProfilsData = new ObservableCollection<ProfilsData>(_ocProfilsData);
            }
            else
            {
                _ocFilteredProfilsData = new ObservableCollection<ProfilsData>(
                    _ocProfilsData.Where(p =>
                        p.Nom.ToLower().Contains(sFilter) ||
                        p.URL.ToLower().Contains(sFilter)));
            }
            dataGridProfils.ItemsSource = _ocFilteredProfilsData;
        }

        // Méthode appelée lors du clic sur le bouton "Paramètres"
        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow swSettingWindow = new SettingWindow();
            swSettingWindow.Show();
        }

        // Méthode appelée lors du clic sur le bouton "Supprimer"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnButton && btnButton.Tag is ProfilsData pdItem)
            {
                if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce profil?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var dcContext = new DataContext())
                    {
                        dcContext.ProfilsData.Remove(pdItem);
                        dcContext.SaveChanges();
                        _ocProfilsData.Remove(pdItem);
                        _ocFilteredProfilsData.Remove(pdItem);
                    }
                }
            }
        }

        // Méthode appelée lors du clic sur le bouton "Éditer"
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnButton && btnButton.Tag is ProfilsData pdItem)
            {
                EditProfileWindow epwEditProfileWindow = new EditProfileWindow(pdItem);
                if (epwEditProfileWindow.ShowDialog() == true)
                {
                    LoadProfilsData();
                }
            }
        }

        // Méthode appelée lors du clic sur le bouton "Ajouter"
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Guid gUserId = App.gLoggedInUserId;
            AddProfileWindow apwAddProfileWindow = new AddProfileWindow(gUserId);
            apwAddProfileWindow.ProfileAdded += AddProfileWindow_ProfileAdded;
            apwAddProfileWindow.ShowDialog();
        }

        // Méthode appelée lorsque un profil est ajouté
        private void AddProfileWindow_ProfileAdded(object sender, ProfilsData e)
        {
            _ocProfilsData.Add(e);
            _ocFilteredProfilsData.Add(e);
            dataGridProfils.Items.Refresh();
        }

        // Méthode appelée lors du clic sur le bouton "Fermer"
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Méthode appelée lors du clic sur le bouton "Afficher le mot de passe"
        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnButton && btnButton.Tag is ProfilsData pdItem)
            {
                _pdSelectedProfile = pdItem;
                PasswordTextBox.Text = classFonctionGenerale.DecrypterChaine(pdItem.EncryptedPassword);
                PasswordLabel.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Visible;
            }
        }

        // Méthode appelée lors du clic sur le bouton "Copier le mot de passe"
        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnButton && btnButton.Tag is ProfilsData pdItem)
            {
                Clipboard.SetText(classFonctionGenerale.DecrypterChaine(pdItem.EncryptedPassword));
                MessageBox.Show("Mot de passe copié dans le presse-papiers.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Méthode appelée lors du changement de sélection dans le DataGrid
        private void DataGridProfils_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PasswordLabel.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Collapsed;
        }

        // Méthode appelée lors du clic sur le bouton "CrashApi"
        private void CrashApi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? iDivisor = null; // Définissez une variable nullable qui provoquera une division par zéro
                int iResult = 10 / iDivisor.Value; // Provoque une exception de division par zéro
            }
            catch (Exception ex)
            {
                classFonctionGenerale.GestionErreurLog(ex, "Erreur lors de la division par zéro dans CrashApi_Click", false);
            }
        }

        // Méthode appelée lors du clic sur le bouton d'aide
        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }
    }
}
