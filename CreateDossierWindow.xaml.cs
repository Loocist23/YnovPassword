using System;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class CreateDossierWindow : Window
    {
        public Dossiers? CreatedDossier { get; private set; }

        public CreateDossierWindow()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string dossierName = DossierNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(dossierName))
            {
                MessageBox.Show("Le nom du dossier ne peut pas être vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new DataContext())
            {
                var newDossier = new Dossiers
                {
                    ID = Guid.NewGuid(),
                    Nom = dossierName
                };
                context.Dossiers.Add(newDossier);
                context.SaveChanges();
                CreatedDossier = newDossier;
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
