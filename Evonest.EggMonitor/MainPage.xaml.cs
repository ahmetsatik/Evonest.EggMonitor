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

            Day();

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
                await Task.Delay(1000);
                CurrentDayLabel.Text = Convert.ToString(_dayCount.elapsedTime);
            }
    }


}
}
