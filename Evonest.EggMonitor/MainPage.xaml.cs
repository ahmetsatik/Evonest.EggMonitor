using Evonest.EggMonitor.Models;
using Evonest.EggMonitor.Services;

namespace Evonest.EggMonitor
{
    public partial class MainPage : ContentPage
    {
        private readonly DayCount _dayCount;

        public MainPage()
        {
            InitializeComponent();

            _dayCount = new DayCount();

            _ = _dayCount.DayCounter();
            _ =Day();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var kuluckaGunleri = new List<object>
            {
                new { Day = "Bugun", IdealTemprature = 37.7, IdealHumudity = "Kuluçka başladı. Makineyi sabitleyin." },
                new { Day = "Yarın", IdealTemprature = 37.7, IdealHumudity = "Yumurtaları çevirmeye başlayın." }
            };

            daysCarousel.ItemsSource = kuluckaGunleri;
            daysCarousel.Position = 0;
        }
        
        private async void OnShowHatchClicked(object sender, EventArgs e)
        {
            try
            {
                var dbService = new InstallationDB();


                IncubationPresetModel hatch = await dbService.ShowHatch();

                if (hatch != null)
                {

                    await DisplayAlert("Kuluçka Detayları",
                        $"🆔 Benzersiz ID: {hatch.Id}\n\n" +
                        $"🥚 Kuluçka Adı: {hatch.Name}\n\n" +
                        $"🔢 Yumurta Sayısı: {hatch.EggCount}\n\n" +
                        $"📅 Başlangıç Tarihi: {hatch.StartDate:dd.MM.yyyy HH:mm}\n\n" +
                        $"📅 Başlangıç Tarihi: {hatch.IsActive}",
                        "Kapat");
                }
                else
                {
                    await DisplayAlert("Bilgi", "Veritabanında henüz hiçbir kayıt bulunamadı!", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Veri okunurken bir hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async Task Day()
        {
            while (true)
            {
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_dayCount.elapsedTime==0)
                    {
                        CurrentDayLabel.Text = "İlk gün";
                    }
                    else
                    {
                        CurrentDayLabel.Text = Convert.ToString(_dayCount.elapsedTime);
                    }

                });
                await Task.Delay(1000);
            }
    }


}
}
