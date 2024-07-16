using System;
using System.ComponentModel.DataAnnotations;

// Déclaration de l'espace de noms 'YnovPassword.modele'
namespace YnovPassword.modele
{
    // Déclaration d'une classe interne nommée 'Configuration'
    internal class Configuration
    {
        // Constructeur de la classe 'Configuration'
        public Configuration()
        {
            VersionMajeure = string.Empty;
            VersionMineure = string.Empty;
        }

        // Propriété ID avec l'attribut Key indiquant qu'il s'agit de la clé primaire
        [Key]
        public Guid ID { get; set; }

        // Propriété VersionMajeure avec des attributs de validation de données
        [Required]
        [StringLength(100)]
        public string VersionMajeure { get; set; }

        // Propriété VersionMineure avec des attributs de validation de données
        [Required]
        [StringLength(100)]
        public string VersionMineure { get; set; }
    }
}
