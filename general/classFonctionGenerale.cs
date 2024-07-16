using DLLYnov;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

// Déclaration de l'espace de noms 'YnovPassword.general'
namespace YnovPassword.general
{
    // Déclaration d'une classe abstraite nommée 'classFonctionGenerale'
    public abstract class classFonctionGenerale
    {
        // Méthode pour gérer les erreurs et les logs
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
                CreerFichierJournalErreurImprevisible(sMessageFinal); // Créer un fichier journal d'erreur
                EcrireJournalEvenementsWindows(sMessageFinal); // Écrire dans le journal des événements Windows
            }

            // Afficher un message d'erreur à l'utilisateur
            MessageBox.Show(sMessageFinal, "Erreur Rencontrée", MessageBoxButton.OK, MessageBoxImage.Error);

            // Fermer l'application si bFermeture est vrai
            if (bFermeture)
            {
                Application.Current.Shutdown();
            }
        }

        // Méthode pour créer un fichier journal des erreurs imprévisibles
        public static void CreerFichierJournalErreurImprevisible(string sMessageErreur)
        {
            try
            {
                string sMachineName = Environment.MachineName;
                string sLogFileName = $"{sMachineName}_{DateTime.Now:yyyy-MM-dd}.log";
                string sLogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", sLogFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(sLogFilePath));

                // Écrire le message d'erreur dans le fichier journal
                using (StreamWriter swWriter = new StreamWriter(sLogFilePath, true))
                {
                    swWriter.WriteLine($"[{DateTime.Now}] {sMessageErreur}");
                    swWriter.WriteLine("--------------------------------------------------");
                }
            }
            catch (Exception e)
            {
                // Afficher une erreur si la création du fichier journal échoue
                MessageBox.Show($"Erreur lors de la création du fichier journal : {e.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Méthode pour écrire dans le journal des événements Windows
        public static void EcrireJournalEvenementsWindows(string sMessageErreur)
        {
            try
            {
                string sEventSource = ".NET Runtime";
                string sLogName = "Application";

                // Vérifier si la source de l'événement existe, sinon la créer
                if (!EventLog.SourceExists(sEventSource))
                {
                    EventLog.CreateEventSource(sEventSource, sLogName);
                }

                // Écrire l'entrée dans le journal des événements Windows
                EventLog.WriteEntry(sEventSource, sMessageErreur, EventLogEntryType.Error, 1000);
            }
            catch (Exception e)
            {
                // Afficher une erreur si l'écriture dans le journal des événements échoue
                MessageBox.Show($"Erreur lors de l'écriture dans le journal des événements Windows : {e.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Méthode pour crypter une chaîne
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

        // Méthode pour décrypter une chaîne
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

        // Méthode pour valider le mot de passe
        public static bool ValiderMotdepasse(string sMotDePasse, string sConfirmerMotDePasse)
        {
            return sMotDePasse == sConfirmerMotDePasse && sMotDePasse.Length >= 8;
        }

        // Méthode pour créer un super admin
        public static void CreerSuperAdmin(MigrationBuilder mbMigrationBuilder)
        {
            Guid gIdUtilisateur = Guid.NewGuid();
            Guid gIdDossier = Guid.NewGuid();
            Guid gIdProfilsData = Guid.NewGuid();
            string sPasswordSuperAdmin = "";

            // Cryptage du mot de passe du super admin
            sPasswordSuperAdmin = classFonctionGenerale.CrypterChaine(classConstantes.sUtilisateur_Password_Superadmin);

            // Insertion des valeurs par défaut dans les tables de la base de données
            mbMigrationBuilder.InsertData("Utilisateurs", new[] { "ID", "Nom", "Login", "Email" }, new object[] { gIdUtilisateur, classConstantes.sUtilisateur_Nom_Superadmin, classConstantes.sUtilisateur_Login_Superadmin, "" });

            mbMigrationBuilder.InsertData("Dossiers", new[] { "ID", "Nom" }, new object[] { gIdDossier, classConstantes.sTypeprofilConnection_Nom_YnovPassword });

            mbMigrationBuilder.InsertData("ProfilsData", new[] { "ID", "DossiersID", "UtilisateursID", "Nom", "URL", "Login", "EncryptedPassword" }, new object[] { gIdProfilsData, gIdDossier, gIdUtilisateur, classConstantes.sProfilConection_Nom_YnovPassword, "", classConstantes.sUtilisateur_Nom_Superadmin, sPasswordSuperAdmin });
        }

        // Méthode pour ouvrir le fichier d'aide
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
                    // Afficher un message d'erreur si le fichier d'aide est introuvable
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
