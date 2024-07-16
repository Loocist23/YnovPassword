using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Déclaration de l'espace de noms 'YnovPassword.modele'
namespace YnovPassword.modele
{
    // Déclaration d'une classe publique nommée 'Dossiers'
    public class Dossiers
    {
        // Constructeur de la classe 'Dossiers'
        public Dossiers()
        {
            Nom = string.Empty;
            ProfilsData = new HashSet<ProfilsData>();
        }

        // Propriété ID avec l'attribut Key indiquant qu'il s'agit de la clé primaire
        [Key]
        public Guid ID { get; set; }

        // Propriété Nom avec des attributs de validation de données
        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        // Propriété ProfilsData avec l'attribut Required et déclarée comme collection virtuelle
        [Required]
        public virtual ICollection<ProfilsData> ProfilsData { get; set; }
    }
}
