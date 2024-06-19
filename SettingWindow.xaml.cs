using Microsoft.Win32;
using System.IO;
using System.Windows;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
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
                        var dicoEntry = new Dictionnaire { ID = new Guid() ,Mot = mot };
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
    }
}
