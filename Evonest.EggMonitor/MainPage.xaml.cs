
using Evonest.EggMonitor.Models;
using Evonest.EggMonitor.Services;

namespace Evonest.EggMonitor
{
    public partial class MainPage : ContentPage
    {
        

        public MainPage()
        {
            InitializeComponent();

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
                        $"📅 Başlangıç Tarihi: {hatch.StartDate:dd.MM.yyyy HH:mm}",
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

    }


}
