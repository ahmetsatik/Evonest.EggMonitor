using Microsoft.Extensions.DependencyInjection;
using System;

namespace Evonest.EggMonitor
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                System.Diagnostics.Debug.WriteLine($"[KRİTİK ÇÖKME YAKALANDI]: {exception?.Message}");

            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new SplashScreen());
        }
    }
}