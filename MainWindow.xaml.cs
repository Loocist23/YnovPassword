using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YnovPassword.modele;
using System.Collections.ObjectModel;
using YnovPassword.general;

namespace YnovPassword
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<ProfilsData> _ocProfilsData = new ObservableCollection<ProfilsData>();
        private ObservableCollection<ProfilsData> _ocFilteredProfilsData = new ObservableCollection<ProfilsData>();
        private ProfilsData _pdSelectedProfile;

        public MainWindow()
        {
            InitializeComponent();
            LoadProfilsData();
        }

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

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow swSettingWindow = new SettingWindow();
            swSettingWindow.Show();
        }

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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Guid gUserId = App.gLoggedInUserId;
            AddProfileWindow apwAddProfileWindow = new AddProfileWindow(gUserId);
            apwAddProfileWindow.ProfileAdded += AddProfileWindow_ProfileAdded;
            apwAddProfileWindow.ShowDialog();
        }

        private void AddProfileWindow_ProfileAdded(object sender, ProfilsData e)
        {
            _ocProfilsData.Add(e);
            _ocFilteredProfilsData.Add(e);
            dataGridProfils.Items.Refresh();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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

        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnButton && btnButton.Tag is ProfilsData pdItem)
            {
                Clipboard.SetText(classFonctionGenerale.DecrypterChaine(pdItem.EncryptedPassword));
                MessageBox.Show("Mot de passe copié dans le presse-papiers.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DataGridProfils_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PasswordLabel.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Collapsed;
        }

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

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }

    }
}
