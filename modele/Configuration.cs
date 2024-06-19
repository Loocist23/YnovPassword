using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YnovPassword.modele
{
    internal class Configuration
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [StringLength(100)]
        public string VersionMajeure { get; set; }

        [Required]
        [StringLength(100)]
        public string VersionMineure { get; set; }
    }

}
