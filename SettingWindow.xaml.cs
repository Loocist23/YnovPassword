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
        private ObservableCollection<Dossiers> _dossiers;

        public SettingWindow()
        {
            InitializeComponent();
            LoadDossiers();
        }

        private void LoadDossiers()
        {
            using (var context = new DataContext())
            {
                _dossiers = new ObservableCollection<Dossiers>(context.Dossiers.ToList());
                dataGridDossiers.ItemsSource = _dossiers;
            }
        }

        private void CreateDossier_Click(object sender, RoutedEventArgs e)
        {
            string dossierName = txtNewDossierName.Text.Trim();
            if (string.IsNullOrEmpty(dossierName))
            {
                MessageBox.Show("Le nom du dossier ne peut pas être vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new DataContext())
            {
                // Vérifier si un dossier avec le même nom existe déjà ou s'il s'agit de YNOVPASSWORD avec toute variation d'orthographe
                bool dossierExists = context.Dossiers.Any(d => d.Nom.Equals(dossierName, StringComparison.OrdinalIgnoreCase)) ||
                                     IsYnovPasswordVariant(dossierName);
                if (dossierExists)
                {
                    MessageBox.Show("Un dossier avec ce nom existe déjà ou est réservé. Veuillez choisir un autre nom.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newDossier = new Dossiers
                {
                    ID = Guid.NewGuid(),
                    Nom = dossierName
                };
                context.Dossiers.Add(newDossier);
                context.SaveChanges();
                _dossiers.Add(newDossier);
            }

            txtNewDossierName.Clear();
            MessageBox.Show("Dossier créé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditDossier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Dossiers dossier)
            {
                // Empêcher la modification du dossier YNOVPASSWORD avec toute variation d'orthographe
                if (IsYnovPasswordVariant(dossier.Nom))
                {
                    MessageBox.Show("Le dossier YNOVPASSWORD ne peut pas être modifié.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string newName = Microsoft.VisualBasic.Interaction.InputBox("Entrez le nouveau nom du dossier :", "Modifier Dossier", dossier.Nom);
                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show("Le nom du dossier ne peut pas être vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var context = new DataContext())
                {
                    var dossierToUpdate = context.Dossiers.Find(dossier.ID);
                    if (dossierToUpdate != null)
                    {
                        // Vérifier si un dossier avec le nouveau nom existe déjà ou s'il s'agit de YNOVPASSWORD avec toute variation d'orthographe
                        bool dossierExists = context.Dossiers.Any(d => d.Nom.Equals(newName, StringComparison.OrdinalIgnoreCase) && d.ID != dossier.ID) ||
                                             IsYnovPasswordVariant(newName);
                        if (dossierExists)
                        {
                            MessageBox.Show("Un dossier avec ce nom existe déjà ou est réservé. Veuillez choisir un autre nom.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        dossierToUpdate.Nom = newName;
                        context.SaveChanges();
                    }
                }

                MessageBox.Show("Dossier modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDossiers(); // Rafraîchir la liste des dossiers après modification
            }
        }

        private void DeleteDossier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Dossiers dossier)
            {
                // Empêcher la suppression du dossier YNOVPASSWORD avec toute variation d'orthographe
                if (IsYnovPasswordVariant(dossier.Nom))
                {
                    MessageBox.Show("Le dossier YNOVPASSWORD ne peut pas être supprimé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Empêcher la suppression d'un dossier s'il contient des ProfilsData
                using (var context = new DataContext())
                {
                    bool hasProfilsData = context.ProfilsData.Any(pd => pd.DossiersID == dossier.ID);
                    if (hasProfilsData)
                    {
                        MessageBox.Show("Le dossier contient des ProfilsData et ne peut pas être supprimé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce dossier ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new DataContext())
                    {
                        var dossierToDelete = context.Dossiers.Find(dossier.ID);
                        if (dossierToDelete != null)
                        {
                            context.Dossiers.Remove(dossierToDelete);
                            context.SaveChanges();
                            _dossiers.Remove(dossier);
                        }
                    }

                    MessageBox.Show("Dossier supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ImportDictionary_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Importer un dictionnaire"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                List<string> mots = File.ReadAllLines(filePath).Distinct().ToList(); // Lire toutes les lignes et éliminer les doublons

                using (var context = new DataContext())
                {
                    foreach (string mot in mots)
                    {
                        var dicoEntry = new Dictionnaire { ID = Guid.NewGuid(), Mot = mot };
                        context.Dictionnaires.Add(dicoEntry); // Ajouter chaque mot dans la table Dictionnaires
                    }

                    context.SaveChanges(); // Sauvegarder les changements dans la base de données
                }

                MessageBox.Show($"{mots.Count} mots ont été importés dans le dictionnaire.", "Importation Réussie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool IsYnovPasswordVariant(string dossierName)
        {
            return dossierName.Equals("YNOVPASSWORD", StringComparison.OrdinalIgnoreCase) ||
                   dossierName.Replace(" ", "").Equals("YNOVPASSWORD", StringComparison.OrdinalIgnoreCase) ||
                   dossierName.Replace("-", "").Equals("YNOVPASSWORD", StringComparison.OrdinalIgnoreCase) ||
                   dossierName.Replace("_", "").Equals("YNOVPASSWORD", StringComparison.OrdinalIgnoreCase);
        }
    }
}
