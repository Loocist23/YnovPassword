using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace YnovPassword.modele
{
    class Utilisateurs
    {

        public Utilisateurs() { 
            this.ProfilsData = new HashSet<ProfilsData>();
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
