using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class App : Application
    {
        public static Guid gLoggedInUserId { get; set; }

        private const string sVersionCheckUrl = "http://www.indexld.com/wp-json/ynov/v1/getapp-version";

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await CheckForUpdate();

            DataContext dcLocalDataContext = null;

            try
            {
                dcLocalDataContext = new DataContext();
                dcLocalDataContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                classFonctionGenerale.GestionErreurLog(ex, "", false);
            }

            // Démarrage de la fenêtre de login après la vérification de mise à jour
            LoginWindow lwLoginWindow = new LoginWindow();
            lwLoginWindow.Show();
        }

        private async Task CheckForUpdate()
        {
            try
            {
                var tLatestVersion = await GetLatestVersionAsync();
                var vCurrentVersion = new Version(classConstantes.iBigNumVersion, classConstantes.iSmallNumVersion);

                if (tLatestVersion.Item1 > vCurrentVersion)
                {
                    var mResult = MessageBox.Show("Une nouvelle version de l'application est disponible. Voulez-vous la télécharger ?", "Mise à jour disponible", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (mResult == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = tLatestVersion.Item2,
                            UseShellExecute = true
                        });
                        Application.Current.Shutdown(); // Ferme l'application actuelle
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification des mises à jour : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<Tuple<Version, string>> GetLatestVersionAsync()
        {
            using (HttpClient hcClient = new HttpClient())
            {
                HttpResponseMessage hrResponse = await hcClient.GetAsync(sVersionCheckUrl);
                hrResponse.EnsureSuccessStatusCode();

                string sResponseBody = await hrResponse.Content.ReadAsStringAsync();
                JObject joJson = JObject.Parse(sResponseBody);

                int iMajorVersion = int.Parse(joJson["major_version"].ToString());
                int iMinorVersion = int.Parse(joJson["minor_version"].ToString());
                string sUrlCurrentVersion = joJson["url_current_version"].ToString();

                return new Tuple<Version, string>(new Version(iMajorVersion, iMinorVersion), sUrlCurrentVersion);
            }
        }
    }
}
