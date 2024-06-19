using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YnovPassword.modele
{
    class Dictionnaire
    {
        [Key]
        public Guid ID { get; set; } = new Guid();

        [Required]
        [StringLength(100)]
        public string Mot { get; set; }
    }
}
