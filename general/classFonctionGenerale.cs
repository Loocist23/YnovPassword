using DLLYnov;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace YnovPassword.general
{
    public abstract class classFonctionGenerale
    {
        public static void GestionErreurLog(Exception? ex, string? sMessageLibre, bool bFermeture)
        {
            string sMessageFinal = "";
            if (!string.IsNullOrEmpty(sMessageLibre))
            {
                sMessageFinal += sMessageLibre;
            }
            if (ex != null)
            {
                sMessageFinal += "\r\n Une Erreur a eu lieu : " + ex.ToString();
                CreerFichierJournalErreurImprevisible(sMessageFinal);
                EcrireJournalEvenementsWindows(sMessageFinal);
            }

            MessageBox.Show(sMessageFinal, "Erreur Rencontrée", MessageBoxButton.OK, MessageBoxImage.Error);

            if (bFermeture)
            {
                Application.Current.Shutdown();
            }
        }

        public static void CreerFichierJournalErreurImprevisible(string sMessageErreur)
        {
            try
            {
                string sMachineName = Environment.MachineName;
                string sLogFileName = $"{sMachineName}_{DateTime.Now:yyyy-MM-dd}.log";
                string sLogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", sLogFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(sLogFilePath));

                using (StreamWriter swWriter = new StreamWriter(sLogFilePath, true))
                {
                    swWriter.WriteLine($"[{DateTime.Now}] {sMessageErreur}");
                    swWriter.WriteLine("--------------------------------------------------");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erreur lors de la création du fichier journal : {e.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void EcrireJournalEvenementsWindows(string sMessageErreur)
        {
            try
            {
                string sEventSource = ".NET Runtime";
                string sLogName = "Application";

                if (!EventLog.SourceExists(sEventSource))
                {
                    EventLog.CreateEventSource(sEventSource, sLogName);
                }

                EventLog.WriteEntry(sEventSource, sMessageErreur, EventLogEntryType.Error, 1000);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erreur lors de l'écriture dans le journal des événements Windows : {e.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string? CrypterChaine(string sParChaineACrypter)
        {
            try
            {
                return Cryptage.DLLCrypterChaine(sParChaineACrypter);
            }
            catch (Exception ex)
            {
                GestionErreurLog(ex, "Erreur lors du cryptage de la chaîne", false);
                return null;
            }
        }

        public static string? DecrypterChaine(string sParChaineCryptee)
        {
            try
            {
                return Cryptage.DLLDecrypterChaine(sParChaineCryptee);
            }
            catch (Exception ex)
            {
                GestionErreurLog(ex, "Erreur lors du décryptage de la chaîne", false);
                return null;
            }
        }

        public static bool ValiderMotdepasse(string sMotDePasse, string sConfirmerMotDePasse)
        {
            return sMotDePasse == sConfirmerMotDePasse && sMotDePasse.Length >= 8;
        }

        public static void CreerSuperAdmin(MigrationBuilder mbMigrationBuilder)
        {
            Guid gIdUtilisateur = Guid.NewGuid();
            Guid gIdDossier = Guid.NewGuid();
            Guid gIdProfilsData = Guid.NewGuid();
            string sPasswordSuperAdmin = "";

            //Insertion des valeurs par défaut
            //On crypte le mot de passe de l'utilisateur 
            sPasswordSuperAdmin = classFonctionGenerale.CrypterChaine(classConstantes.sUtilisateur_Password_Superadmin);

            mbMigrationBuilder.InsertData("Utilisateurs", new[] { "ID", "Nom", "Login", "Email" }, new object[] { gIdUtilisateur, classConstantes.sUtilisateur_Nom_Superadmin, classConstantes.sUtilisateur_Login_Superadmin, "" });

            mbMigrationBuilder.InsertData("Dossiers", new[] { "ID", "Nom" }, new object[] { gIdDossier, classConstantes.sTypeprofilConnection_Nom_YnovPassword });

            mbMigrationBuilder.InsertData("ProfilsData", new[] { "ID", "DossiersID", "UtilisateursID", "Nom", "URL", "Login", "EncryptedPassword" }, new object[] { gIdProfilsData, gIdDossier, gIdUtilisateur, classConstantes.sProfilConection_Nom_YnovPassword, "", classConstantes.sUtilisateur_Nom_Superadmin, sPasswordSuperAdmin });
        }

        public static void OpenHelp()
        {
            try
            {
                string sHelpFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "YnovPasswordHelp.chm");
                if (File.Exists(sHelpFilePath))
                {
                    ProcessStartInfo psiProcessStartInfo = new ProcessStartInfo
                    {
                        FileName = sHelpFilePath,
                        UseShellExecute = true
                    };
                    Process.Start(psiProcessStartInfo);
                }
                else
                {
                    MessageBox.Show("Le fichier d'aide est introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                GestionErreurLog(ex, "Erreur lors de l'ouverture du fichier d'aide", false);
            }
        }
    }
}
