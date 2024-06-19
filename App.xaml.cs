using System.Configuration;
using System.Data;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace YnovPassword
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Le code commenté est présent car après une démonstration du professeur
            //nous avons mis en place un système de migration pour ne plus avoir a supprimé puis recréer le fichier .db a chaque modification,
            //ce sont les migrations qui le font pour nous 
            //DatabaseFacade oLocal_DatabaseFacade = new DatabaseFacade( new DataContext()); 
            DataContext oLocal_DataContext = null;
            
            //try
            //{
            //    oLocal_DatabaseFacade.EnsureCreated();
            //}catch(Exception ex)
            //{
            //    classFonctionGenerale.GestionErreurLog(ex, false);
            //}

            try
            {
                oLocal_DataContext = new DataContext();
                oLocal_DataContext.Database.Migrate();
            }
            catch(Exception ex) { 
                classFonctionGenerale.GestionErreurLog(ex,"", false);
            }
        }
    }

}
