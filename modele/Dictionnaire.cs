using System;
using System.ComponentModel.DataAnnotations;

// Déclaration de l'espace de noms 'YnovPassword.modele'
namespace YnovPassword.modele
{
    // Déclaration d'une classe interne nommée 'Dictionnaire'
    internal class Dictionnaire
    {
        // Constructeur de la classe 'Dictionnaire'
        public Dictionnaire()
        {
            Mot = string.Empty;
        }

        // Propriété ID avec l'attribut Key indiquant qu'il s'agit de la clé primaire
        [Key]
        public Guid ID { get; set; }

        // Propriété Mot avec des attributs de validation de données
        [Required]
        [StringLength(100)]
        public string Mot { get; set; }
    }
}
