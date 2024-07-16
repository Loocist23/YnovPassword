using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Déclaration de l'espace de noms 'YnovPassword.general'
namespace YnovPassword.general
{
    // Déclaration d'une classe interne nommée 'classConstantes'
    internal class classConstantes
    {
        // Déclaration de constantes pour des valeurs de chaîne utilisées dans l'application

        // Clé de cryptage utilisée pour chiffrer les données
        public const string sGeneral_CleCryptage = "Bk_Yk-#d859U3X=c";

        // Nom de l'utilisateur superadmin
        public const string sUtilisateur_Nom_Superadmin = "SUPERADMIN";

        // Identifiant de connexion pour l'utilisateur superadmin
        public const string sUtilisateur_Login_Superadmin = "SUPERADMIN";

        // Mot de passe pour l'utilisateur superadmin
        public const string sUtilisateur_Password_Superadmin = "YNOV";

        // Nom du type de profil de connexion utilisé par l'application
        public const string sTypeprofilConnection_Nom_YnovPassword = "YNOVPASSWORD";

        // Nom du profil de connexion utilisé par l'application
        public const string sProfilConection_Nom_YnovPassword = "YNOVPASSWORD";

        // Numéro de version majeure de l'application
        public const int iBigNumVersion = 9;

        // Numéro de version mineure de l'application
        public const int iSmallNumVersion = 0;
    }
}
