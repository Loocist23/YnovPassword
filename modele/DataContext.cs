using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Déclaration de l'espace de noms 'YnovPassword.modele'
namespace YnovPassword.modele
{
    /// <summary>
    /// Classe DataContext, héritée de la classe Entity DBContext et référençant l'ensemble des classes
    /// </summary>
    class DataContext : DbContext
    {
        /// <summary>
        /// Évènement permettant de définir le nom de la base de données utilisée/créée dans le contexte
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configuration de la source de données pour utiliser une base de données SQLite
                optionsBuilder.UseSqlite("Data Source = YnovPassword.db");
            }
        }

        // Lignes déclarant la génération des tables de la base de données
        public DbSet<Utilisateurs> Utilisateurs { get; set; }
        public DbSet<Dossiers> Dossiers { get; set; }
        public DbSet<Dictionnaire> Dictionnaires { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<ProfilsData> ProfilsData { get; set; }
    }
}
