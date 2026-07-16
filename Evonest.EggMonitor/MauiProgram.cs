using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using Plugin.LocalNotification;

namespace Evonest.EggMonitor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Batteries.Init();
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
