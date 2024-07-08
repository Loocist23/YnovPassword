using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YnovPassword.modele
{
    public class Utilisateurs
    {
        public Utilisateurs()
        {
            Nom = string.Empty;
            Login = string.Empty;
            Email = string.Empty;
            ProfilsData = new HashSet<ProfilsData>();
        }

        [Key]
        public Guid ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        [StringLength(100)]
        public string Login { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public virtual ICollection<ProfilsData> ProfilsData { get; set; }
    }
}
