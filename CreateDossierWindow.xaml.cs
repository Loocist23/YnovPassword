using System;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

// Déclaration de l'espace de noms 'YnovPassword'
namespace YnovPassword
{
    // Déclaration partielle de la classe CreateDossierWindow héritant de Window
    public partial class CreateDossierWindow : Window
    {
        // Propriété pour stocker le dossier créé
        public Dossiers? dCreatedDossier { get; private set; }

        // Constructeur de la classe CreateDossierWindow
        public CreateDossierWindow()
        {
            InitializeComponent();
        }

        // Méthode appelée lors du clic sur le bouton Créer
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le nom du dossier à partir de la zone de texte
            string sDossierName = DossierNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(sDossierName))
            {
                // Afficher un message d'erreur si le nom du dossier est vide
                MessageBox.Show("Le nom du dossier ne peut pas être vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Créer un nouveau dossier dans la base de données
            using (var dcContext = new DataContext())
            {
                var dNewDossier = new Dossiers
                {
                    ID = Guid.NewGuid(),
                    Nom = sDossierName
                };
                dcContext.Dossiers.Add(dNewDossier);
                dcContext.SaveChanges();
                dCreatedDossier = dNewDossier; // Stocker le dossier créé
            }

            // Afficher un message de succès et fermer la fenêtre
            MessageBox.Show("Dossier créé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        // Méthode appelée lors du clic sur le bouton d'aide
        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }
    }
}
