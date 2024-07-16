using System;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class CreateDossierWindow : Window
    {
        public Dossiers? dCreatedDossier { get; private set; }

        public CreateDossierWindow()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string sDossierName = DossierNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(sDossierName))
            {
                MessageBox.Show("Le nom du dossier ne peut pas être vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var dcContext = new DataContext())
            {
                var dNewDossier = new Dossiers
                {
                    ID = Guid.NewGuid(),
                    Nom = sDossierName
                };
                dcContext.Dossiers.Add(dNewDossier);
                dcContext.SaveChanges();
                dCreatedDossier = dNewDossier;
            }

            MessageBox.Show("Dossier créé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }
    }
}
