using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using YnovPassword.general;
using YnovPassword.modele;

// Déclaration de l'espace de noms 'YnovPassword'
namespace YnovPassword
{
    // Déclaration partielle de la classe App héritant de Application
    public partial class App : Application
    {
        // Propriété statique pour stocker l'identifiant de l'utilisateur connecté
        public static Guid gLoggedInUserId { get; set; }

        // URL pour vérifier la version de l'application
        private const string sVersionCheckUrl = "http://www.indexld.com/wp-json/ynov/v1/getapp-version";

        // Méthode appelée au démarrage de l'application
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await CheckForUpdate(); // Vérifier les mises à jour

            DataContext dcLocalDataContext = null;

            try
            {
                // Initialiser et migrer la base de données
                dcLocalDataContext = new DataContext();
                dcLocalDataContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                // Gérer les erreurs en utilisant la méthode de gestion des erreurs
                classFonctionGenerale.GestionErreurLog(ex, "", false);
            }

            // Démarrage de la fenêtre de login après la vérification de mise à jour
            LoginWindow lwLoginWindow = new LoginWindow();
            lwLoginWindow.Show();
        }

        // Méthode pour vérifier les mises à jour
        private async Task CheckForUpdate()
        {
            try
            {
                // Récupérer la dernière version disponible
                var tLatestVersion = await GetLatestVersionAsync();
                var vCurrentVersion = new Version(classConstantes.iBigNumVersion, classConstantes.iSmallNumVersion);

                // Comparer la version actuelle avec la version disponible
                if (tLatestVersion.Item1 > vCurrentVersion)
                {
                    var mResult = MessageBox.Show("Une nouvelle version de l'application est disponible. Voulez-vous la télécharger ?", "Mise à jour disponible", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (mResult == MessageBoxResult.Yes)
                    {
                        // Ouvrir l'URL de la nouvelle version et fermer l'application actuelle
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = tLatestVersion.Item2,
                            UseShellExecute = true
                        });
                        Application.Current.Shutdown(); // Fermer l'application actuelle
                    }
                }
            }
            catch (Exception ex)
            {
                // Afficher un message d'erreur si la vérification des mises à jour échoue
                MessageBox.Show($"Erreur lors de la vérification des mises à jour : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Méthode pour récupérer la dernière version disponible de l'application
        private async Task<Tuple<Version, string>> GetLatestVersionAsync()
        {
            using (HttpClient hcClient = new HttpClient())
            {
                HttpResponseMessage hrResponse = await hcClient.GetAsync(sVersionCheckUrl);
                hrResponse.EnsureSuccessStatusCode();

                string sResponseBody = await hrResponse.Content.ReadAsStringAsync();
                JObject joJson = JObject.Parse(sResponseBody);

                // Parse la version majeure et mineure ainsi que l'URL de la version actuelle
                int iMajorVersion = int.Parse(joJson["major_version"].ToString());
                int iMinorVersion = int.Parse(joJson["minor_version"].ToString());
                string sUrlCurrentVersion = joJson["url_current_version"].ToString();

                return new Tuple<Version, string>(new Version(iMajorVersion, iMinorVersion), sUrlCurrentVersion);
            }
        }
    }
}
