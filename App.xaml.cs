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
        public static Guid LoggedInUserId { get; set; }

        private const string VersionCheckUrl = "http://www.indexld.com/wp-json/ynov/v1/getapp-version";

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await CheckForUpdate();

            DataContext oLocal_DataContext = null;

            try
            {
                oLocal_DataContext = new DataContext();
                oLocal_DataContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                classFonctionGenerale.GestionErreurLog(ex, "", false);
            }

            // Démarrage de la fenêtre de login après la vérification de mise à jour
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        private async Task CheckForUpdate()
        {
            try
            {
                var latestVersion = await GetLatestVersionAsync();
                var currentVersion = new Version(classConstantes.iBigNumVersion, classConstantes.iSmallNumVersion);

                if (latestVersion.Item1 > currentVersion)
                {
                    var result = MessageBox.Show("Une nouvelle version de l'application est disponible. Voulez-vous la télécharger ?", "Mise à jour disponible", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = latestVersion.Item2,
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
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(VersionCheckUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);

                int majorVersion = int.Parse(json["major_version"].ToString());
                int minorVersion = int.Parse(json["minor_version"].ToString());
                string urlCurrentVersion = json["url_current_version"].ToString();

                return new Tuple<Version, string>(new Version(majorVersion, minorVersion), urlCurrentVersion);
            }
        }
    }
}
