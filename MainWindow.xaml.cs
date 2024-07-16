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
        private ObservableCollection<ProfilsData> _profilsData = new ObservableCollection<ProfilsData>();
        private ObservableCollection<ProfilsData> _filteredProfilsData = new ObservableCollection<ProfilsData>();
        private ProfilsData _selectedProfile;

        public MainWindow()
        {
            InitializeComponent();
            LoadProfilsData();
        }

        private void LoadProfilsData()
        {
            using (var context = new DataContext())
            {
                var userId = App.LoggedInUserId;
                _profilsData = new ObservableCollection<ProfilsData>(context.ProfilsData.Where(p => p.UtilisateursID == userId).ToList());
                _filteredProfilsData = new ObservableCollection<ProfilsData>(_profilsData);
                dataGridProfils.ItemsSource = _filteredProfilsData;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchTextBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(filter))
            {
                _filteredProfilsData = new ObservableCollection<ProfilsData>(_profilsData);
            }
            else
            {
                _filteredProfilsData = new ObservableCollection<ProfilsData>(
                    _profilsData.Where(p =>
                        p.Nom.ToLower().Contains(filter) ||
                        p.URL.ToLower().Contains(filter)));
            }
            dataGridProfils.ItemsSource = _filteredProfilsData;
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProfilsData item)
            {
                if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce profil?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var context = new DataContext())
                    {
                        context.ProfilsData.Remove(item);
                        context.SaveChanges();
                        _profilsData.Remove(item);
                        _filteredProfilsData.Remove(item);
                    }
                }
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProfilsData item)
            {
                EditProfileWindow editProfileWindow = new EditProfileWindow(item);
                if (editProfileWindow.ShowDialog() == true)
                {
                    LoadProfilsData();
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Guid userId = App.LoggedInUserId;
            AddProfileWindow addProfileWindow = new AddProfileWindow(userId);
            addProfileWindow.ProfileAdded += AddProfileWindow_ProfileAdded;
            addProfileWindow.ShowDialog();
        }

        private void AddProfileWindow_ProfileAdded(object sender, ProfilsData e)
        {
            _profilsData.Add(e);
            _filteredProfilsData.Add(e);
            dataGridProfils.Items.Refresh();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProfilsData item)
            {
                _selectedProfile = item;
                PasswordTextBox.Text = classFonctionGenerale.DecrypterChaine(item.EncryptedPassword);
                PasswordLabel.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Visible;
            }
        }

        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProfilsData item)
            {
                Clipboard.SetText(classFonctionGenerale.DecrypterChaine(item.EncryptedPassword));
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
                int? divisor = null; // Définissez une variable nullable qui provoquera une division par zéro
                int result = 10 / divisor.Value; // Provoque une exception de division par zéro
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