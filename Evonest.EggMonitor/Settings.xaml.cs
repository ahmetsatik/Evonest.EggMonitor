namespace Evonest.EggMonitor;

public partial class Settings : ContentPage
{
    public Settings()
    {
        InitializeComponent();
        NotificationSwitch.IsToggled = Preferences.Default.Get("NotificationsEnabled", true);

    }

    private async void OnBuyMachineClicked(object sender, EventArgs e)
    {
        string url = "https://www.evonestkulucka.com/kulucka-makinesi-1";
        await Launcher.OpenAsync(new Uri(url));
    }

    private async void OnAccessoriesClicked(object sender, EventArgs e)
    {
        string url = "https://www.evonestkulucka.com/kulucka-yedek-parcalari";
        await Launcher.OpenAsync(new Uri(url));
    }

    private async void OnContactClicked(object sender, EventArgs e)
    {
        string url = "https://www.evonestkulucka.com/iletisim";
        await Launcher.OpenAsync(new Uri(url));
    }

    private async void OnPrivacyClicked(object sender, EventArgs e)
    {
        string url = "https://www.evonestkulucka.com/gizlilik-sozlesmesi";
        await Launcher.OpenAsync(new Uri(url));
    }

    private async void OnThemeClicked(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet("Uygulama Teması Seçin", "İptal", null, "Açık Mod", "Koyu Mod", "Sistem Varsayılanı");

        if (action == "Açık Mod")
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            ThemeStatusLabel.Text = "Açık Mod";
        }
        else if (action == "Koyu Mod")
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
            ThemeStatusLabel.Text = "Koyu Mod";
        }
        else if (action == "Sistem Varsayılanı")
        {
            Application.Current.UserAppTheme = AppTheme.Unspecified;
            ThemeStatusLabel.Text = "Sistem Varsayılanı";
        }
    }

    private async void OnNotificationToggled(object sender, ToggledEventArgs e)
    {
        bool isEnabled = e.Value;

        Preferences.Default.Set("NotificationsEnabled", isEnabled);

        if (!isEnabled)
        {
            await DisplayAlert("Uyarı", "Bildirimleri ve alarmları kapattınız. Kritik sıcaklık veya nem düşüşlerinde cihazınız uyarı vermeyecektir!", "Anladım");
        }
    }
}