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
            Routing.RegisterRoute("TrackerPage", typeof(TrackerPage));
            Routing.RegisterRoute("NewHatchPage", typeof(NewHatchPage));
            Routing.RegisterRoute("ManualPage", typeof(ManualPage));
            Routing.RegisterRoute("Settings", typeof(Settings));
        }

    }
}
