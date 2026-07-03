using Evonest.EggMonitor.Services;
using SQLite;
using Microsoft.Maui.Controls;



namespace Evonest.EggMonitor
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();



            Routing.RegisterRoute("SplashScreen", typeof(SplashScreen));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("InstallationPage", typeof(InstallationPage));
        }

    }
}
