using System;
using System.ComponentModel.DataAnnotations;

// Déclaration de l'espace de noms 'YnovPassword.modele'
namespace YnovPassword.modele
{
    // Déclaration d'une classe publique nommée 'ProfilsData'
    public class ProfilsData
    {
        // Constructeur de la classe 'ProfilsData'
        public ProfilsData()
        {
            Nom = string.Empty;
            URL = string.Empty;
            Login = string.Empty;
            EncryptedPassword = string.Empty;
        }

        // Propriété ID avec l'attribut Key indiquant qu'il s'agit de la clé primaire
        [Key]
        public Guid ID { get; set; }

        // Propriétés pour les relations avec les entités Utilisateurs et Dossiers
        public Guid UtilisateursID { get; set; }
        public Guid DossiersID { get; set; }

        // Propriété Nom avec des attributs de validation de données
        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        // Propriété URL avec des attributs de validation de données
        [Required]
        [StringLength(255)]
        public string URL { get; set; }

        // Propriété Login avec des attributs de validation de données
        [Required]
        [StringLength(100)]
        public string Login { get; set; }

        // Propriété EncryptedPassword avec des attributs de validation de données
        [Required]
        [StringLength(255)]
        public string EncryptedPassword { get; set; }

        // Propriétés de navigation virtuelle pour les relations
        public virtual Utilisateurs? Utilisateurs { get; set; }
        public virtual Dossiers? Dossiers { get; set; }
    }
}
