using Evonest.EggMonitor.Models;
using Evonest.EggMonitor.Services;
using System.Formats.Asn1;
using System.Security.Cryptography;

namespace Evonest.EggMonitor;

public partial class NewHatchPage : ContentPage
{
    private readonly InstallationDB _database;
    public NewHatchPage()
	{
		InitializeComponent();
	}

	private async void OnNewHatchClicked(object sender, EventArgs e)
	{

        if (string.IsNullOrWhiteSpace(HatchNameEntry.Text) ||
    HatchNameEntry.Text == "Kuluckaniza isim giriniz" ||
    BirdTypePicker.SelectedIndex == -1 ||
    string.IsNullOrWhiteSpace(EggCountEntry.Text))
        {
            await DisplayAlert("Hata", "Lütfen tüm alanları doldurun ve bir kuş cinsi seçin.", "Tamam");
            return;
        }

        var dbService = new InstallationDB();
        await dbService.DeactivateCurrentHatchAsync();

        DateTime selectedDate = StartingDatePicker.Date ?? DateTime.Today;
        TimeSpan selectedTime = StartingTimePicker.Time ?? TimeSpan.Zero;
        DateTime startDateTime = selectedDate.Add(selectedTime);


        var newHatch = new IncubationPresetModel
        {
            Name = HatchNameEntry.Text,
            Species = BirdTypePicker.SelectedItem?.ToString() ?? string.Empty,
            EggId = BirdTypePicker.SelectedIndex + 1,
            EggCount = int.TryParse(EggCountEntry.Text, out int count) ? count : 0,
            IsAutomaticTurner = MachineTypeSwitch.IsToggled,
            StartDate = startDateTime,
            IsActive = true
        };

        

        int result = await dbService.SaveHatchAsync(newHatch);

        var notificationService = new NotificationsService();
        await notificationService.ScheduleNotificationsAsync(eggId: BirdTypePicker.SelectedIndex + 1, hatchId: newHatch.Id, hatchName: HatchNameEntry.Text, startDate: startDateTime);

        await DisplayAlert("Kurulum Başarılı!", $"Kuluçka Ad    ı: {newHatch.Name}", "Tamam");
        Application.Current.MainPage = new NavigationPage(new AppShell());
    }
}