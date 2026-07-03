using Evonest.EggMonitor.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Evonest.EggMonitor
{
    public partial class SplashScreen : ContentPage
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(1000);

            var dbService = new InstallationDB();
            var activeHatch = await dbService.GetActiveHatchAsync();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (activeHatch != null)
                {
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new InstallationPage());
                }
            });
        }
    }
}