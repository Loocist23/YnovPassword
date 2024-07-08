using System;
using System.ComponentModel.DataAnnotations;

namespace YnovPassword.modele
{
    public class ProfilsData
    {
        public ProfilsData()
        {
            Nom = string.Empty;
            URL = string.Empty;
            Login = string.Empty;
            EncryptedPassword = string.Empty;
        }

        [Key]
        public Guid ID { get; set; }

        public Guid UtilisateursID { get; set; }

        public Guid DossiersID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        [StringLength(255)]
        public string URL { get; set; }

        [Required]
        [StringLength(100)]
        public string Login { get; set; }

        [Required]
        [StringLength(255)]
        public string EncryptedPassword { get; set; }

        public virtual Utilisateurs? Utilisateurs { get; set; }
        public virtual Dossiers? Dossiers { get; set; }
    }
}
