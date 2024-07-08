using System;
using System.ComponentModel.DataAnnotations;

namespace YnovPassword.modele
{
    internal class Dictionnaire
    {
        public Dictionnaire()
        {
            Mot = string.Empty;
        }

        [Key]
        public Guid ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Mot { get; set; }
    }
}
