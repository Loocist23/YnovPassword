using System;
using System.ComponentModel.DataAnnotations;

namespace YnovPassword.modele
{
    internal class Configuration
    {
        public Configuration()
        {
            VersionMajeure = string.Empty;
            VersionMineure = string.Empty;
        }

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
