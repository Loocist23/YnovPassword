using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YnovPassword.general;
using DllYnov;

namespace YnovPassword.general
{
    public abstract class classFonctionGenerale
    {

        public static void GestionErreurLog(Exception? ex, string? MessageLibre, bool boolFermeture)
        {
            string MessageFinal = "";
            if (!string.IsNullOrEmpty(MessageLibre))
            {
                MessageFinal += MessageLibre;
            }
            if (ex != null)
            {
                MessageFinal += "\r\n Une Erreur a eu lieu : " + ex.ToString();
                CreerFichierJournalErreurImprevisible(MessageFinal);
                EcrireJournalEvenementsWindows(MessageFinal);
            }

            MessageBox.Show(MessageFinal, "Erreur Rencontrée", MessageBoxButton.OK, MessageBoxImage.Error);

            if (boolFermeture)
            {
                Application.Current.Shutdown();
            }
        }

        public static void CreerFichierJournalErreurImprevisible(string messageErreur)
        {
            try
            {
                string machineName = Environment.MachineName;
                string logFileName = $"{machineName}_{DateTime.Now:yyyy-MM-dd}.log";
                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", logFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.Now}] {messageErreur}");
                    writer.WriteLine("--------------------------------------------------");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erreur lors de la création du fichier journal : {e.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void EcrireJournalEvenementsWindows(string messageErreur)
        {
            try
            {
                string eventSource = ".NET Runtime";
                string logName = "Application";

                if (!EventLog.SourceExists(eventSource))
                {
                    EventLog.CreateEventSource(eventSource, logName);
                }

                EventLog.WriteEntry(eventSource, messageErreur, EventLogEntryType.Error, 1000);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erreur lors de l'écriture dans le journal des événements Windows : {e.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string? CrypterChaine(string sPar_ChaineACrypter)
        {
            try
            {
                return Cryptage.DLLCrypterChaine(sPar_ChaineACrypter);
            }
            catch (Exception ex)
            {
                GestionErreurLog(ex, "Erreur lors du cryptage de la chaîne", false);
                return null;
            }
        }

        public static string? DecrypterChaine(string sPar_ChaineCryptee)
        {
            try
            {
                return Cryptage.DLLDecrypterChaine(sPar_ChaineCryptee);
            }
            catch (Exception ex)
            {
                GestionErreurLog(ex, "Erreur lors du décryptage de la chaîne", false);
                return null;
            }
        }

        public static bool ValiderMotdepasse(string motDePasse, string confirmerMotDePasse)
        {
            return motDePasse == confirmerMotDePasse && motDePasse.Length >= 8;
        }

        public static void CreerSuperAdmin(MigrationBuilder oLocal_migrationBuilder)
        {
            Guid gLocal_IdUtilisateur = Guid.NewGuid();
            Guid gLocal_IdDossier = Guid.NewGuid();
            Guid gLocal_IdProfilsData = Guid.NewGuid();
            string sLocal_PasswordSuperAdmin = "";

            //Insertion des valeurs par defaut
            //On crypte le mots de passe de l'utilisateur 
            sLocal_PasswordSuperAdmin = classFonctionGenerale.CrypterChaine(classConstantes.sUtilisateur_Password_Superadmin);

            oLocal_migrationBuilder.InsertData("Utilisateurs", new[] { "ID", "Nom", "Login", "Email" }, new object[] { gLocal_IdUtilisateur, classConstantes.sUtilisateur_Nom_Superadmin, classConstantes.sUtilisateur_Login_Superadmin, "" });

            oLocal_migrationBuilder.InsertData("Dossiers", new[] { "ID", "Nom" }, new object[] { gLocal_IdDossier, classConstantes.sTypeprofilConnection_Nom_YnovPassword });

            oLocal_migrationBuilder.InsertData("ProfilsData", new[] { "ID", "DossiersID", "UtilisateursID", "Nom", "URL", "Login", "EncryptedPassword" }, new object[] { gLocal_IdProfilsData, gLocal_IdDossier, gLocal_IdUtilisateur, classConstantes.sProfilConection_Nom_YnovPassword, "", classConstantes.sUtilisateur_Nom_Superadmin, sLocal_PasswordSuperAdmin });
        }

        

    }
}