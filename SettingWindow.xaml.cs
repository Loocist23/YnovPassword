using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class SettingWindow : Window
    {
        private ObservableCollection<Dossiers> _ocDossiers;

        public SettingWindow()
        {
            InitializeComponent();
            LoadDossiers();
        }

        private void LoadDossiers()
        {
            using (var dcContext = new DataContext())
            {
                _ocDossiers = new ObservableCollection<Dossiers>(dcContext.Dossiers.ToList());
                dataGridDossiers.ItemsSource = _ocDossiers;
            }
        }

        private void CreateDossier_Click(object sender, RoutedEventArgs e)
        {
            string sDossierName = txtNewDossierName.Text.Trim();
            if (string.IsNullOrEmpty(sDossierName))
            {
                MessageBox.Show("Le nom du dossier ne peut pas être vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var dcContext = new DataContext())
            {
                // Vérifier si un dossier avec le même nom existe déjà ou s'il s'agit de YNOVPASSWORD avec toute variation d'orthographe
                bool bDossierExists = dcContext.Dossiers.Any(d => d.Nom.Equals(sDossierName, StringComparison.OrdinalIgnoreCase)) ||
                                     IsYnovPasswordVariant(sDossierName);
                if (bDossierExists)
                {
                    MessageBox.Show("Un dossier avec ce nom existe déjà ou est réservé. Veuillez choisir un autre nom.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dNewDossier = new Dossiers
                {
                    ID = Guid.NewGuid(),
                    Nom = sDossierName
                };
                dcContext.Dossiers.Add(dNewDossier);
                dcContext.SaveChanges();
                _ocDossiers.Add(dNewDossier);
            }

            txtNewDossierName.Clear();
            MessageBox.Show("Dossier créé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditDossier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnButton && btnButton.Tag is Dossiers dDossier)
            {
                // Empêcher la modification du dossier YNOVPASSWORD avec toute variation d'orthographe
                if (IsYnovPasswordVariant(dDossier.Nom))
                {
                    MessageBox.Show("Le dossier YNOVPASSWORD ne peut pas être modifié.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string sNewName = Microsoft.VisualBasic.Interaction.InputBox("Entrez le nouveau nom du dossier :", "Modifier Dossier", dDossier.Nom);
                if (string.IsNullOrEmpty(sNewName))
                {
                    MessageBox.Show("Le nom du dossier ne peut pas être vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var dcContext = new DataContext())
                {
                    var dDossierToUpdate = dcContext.Dossiers.Find(dDossier.ID);
                    if (dDossierToUpdate != null)
                    {
                        // Vérifier si un dossier avec le nouveau nom existe déjà ou s'il s'agit de YNOVPASSWORD avec toute variation d'orthographe
                        bool bDossierExists = dcContext.Dossiers.Any(d => d.Nom.Equals(sNewName, StringComparison.OrdinalIgnoreCase) && d.ID != dDossier.ID) ||
                                             IsYnovPasswordVariant(sNewName);
                        if (bDossierExists)
                        {
                            MessageBox.Show("Un dossier avec ce nom existe déjà ou est réservé. Veuillez choisir un autre nom.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        dDossierToUpdate.Nom = sNewName;
                        dcContext.SaveChanges();
                    }
                }

                MessageBox.Show("Dossier modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDossiers(); // Rafraîchir la liste des dossiers après modification
            }
        }

        private void DeleteDossier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnButton && btnButton.Tag is Dossiers dDossier)
            {
                // Empêcher la suppression du dossier YNOVPASSWORD avec toute variation d'orthographe
                if (IsYnovPasswordVariant(dDossier.Nom))
                {
                    MessageBox.Show("Le dossier YNOVPASSWORD ne peut pas être supprimé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Empêcher la suppression d'un dossier s'il contient des ProfilsData
                using (var dcContext = new DataContext())
                {
                    bool bHasProfilsData = dcContext.ProfilsData.Any(pd => pd.DossiersID == dDossier.ID);
                    if (bHasProfilsData)
                    {
                        MessageBox.Show("Le dossier contient des ProfilsData et ne peut pas être supprimé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var mResult = MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce dossier ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (mResult == MessageBoxResult.Yes)
                {
                    using (var dcContext = new DataContext())
                    {
                        var dDossierToDelete = dcContext.Dossiers.Find(dDossier.ID);
                        if (dDossierToDelete != null)
                        {
                            dcContext.Dossiers.Remove(dDossierToDelete);
                            dcContext.SaveChanges();
                            _ocDossiers.Remove(dDossier);
                        }
                    }

                    MessageBox.Show("Dossier supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ImportDictionary_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdOpenFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Importer un dictionnaire"
            };

            if (ofdOpenFileDialog.ShowDialog() == true)
            {
                string sFilePath = ofdOpenFileDialog.FileName;
                List<string> lsMots = File.ReadAllLines(sFilePath).Distinct().ToList(); // Lire toutes les lignes et éliminer les doublons

                using (var dcContext = new DataContext())
                {
                    foreach (string sMot in lsMots)
                    {
                        var dDicoEntry = new Dictionnaire { ID = Guid.NewGuid(), Mot = sMot };
                        dcContext.Dictionnaires.Add(dDicoEntry); // Ajouter chaque mot dans la table Dictionnaires
                    }

                    dcContext.SaveChanges(); // Sauvegarder les changements dans la base de données
                }

                MessageBox.Show($"{lsMots.Count} mots ont été importés dans le dictionnaire.", "Importation Réussie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool IsYnovPasswordVariant(string sDossierName)
        {
            return sDossierName.Equals("YNOVPASSWORD", StringComparison.OrdinalIgnoreCase) ||
                   sDossierName.Replace(" ", "").Equals("YNOVPASSWORD", StringComparison.OrdinalIgnoreCase) ||
                   sDossierName.Replace("-", "").Equals("YNOVPASSWORD", StringComparison.OrdinalIgnoreCase) ||
                   sDossierName.Replace("_", "").Equals("YNOVPASSWORD", StringComparison.OrdinalIgnoreCase);
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }
    }
}
