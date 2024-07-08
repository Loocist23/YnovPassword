using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YnovPassword.modele
{
    public class Dossiers
    {
        public Dossiers()
        {
            Nom = string.Empty;
            ProfilsData = new HashSet<ProfilsData>();
        }

        [Key]
        public Guid ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        public virtual ICollection<ProfilsData> ProfilsData { get; set; }
    }
}
