using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YnovPassword.modele;
using System.Collections.ObjectModel;
using System.Windows.Input;
using YnovPassword.general;

namespace YnovPassword
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<ProfilsData> _profilsData = new ObservableCollection<ProfilsData>();
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
                dataGridProfils.ItemsSource = _profilsData;
            }
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
                using (var context = new DataContext())
                {
                    context.ProfilsData.Remove(item);
                    context.SaveChanges();
                    _profilsData.Remove(item);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProfilsData item)
            {
                // Logique de modification de l'élément 'item'
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
    }
}
