using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class App : Application
    {
        public static Guid LoggedInUserId { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            DataContext oLocal_DataContext = null;

            try
            {
                oLocal_DataContext = new DataContext();
                oLocal_DataContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                classFonctionGenerale.GestionErreurLog(ex, "", false);
            }
        }
    }
}
