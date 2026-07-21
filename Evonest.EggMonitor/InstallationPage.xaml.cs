using CommunityToolkit.Maui.Converters;
using Evonest.EggMonitor.Models;
using Evonest.EggMonitor.Services;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using SQLite;
using System;

namespace Evonest.EggMonitor
{
    public partial class InstallationPage : ContentPage
    {
        private readonly InstallationDB _databaseService;
        public InstallationPage()
        {
            InitializeComponent();
            CopyDatabaseFromRawAsync();
            StartingTimePicker.Time = DateTime.Now.TimeOfDay;

            _databaseService = new InstallationDB();
        }

        private async void OnInstallationClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HatchNameEntry.Text) ||
                HatchNameEntry.Text == "Kuluckaniza isim giriniz" ||
                BirdTypePicker.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(EggCountEntry.Text))
            {
                await DisplayAlert("Hata", "Lütfen tüm alanları doldurun ve bir kuş cinsi seçin.", "Tamam");
                return;
            }

            DateTime selectedDate = StartingDatePicker.Date ?? DateTime.Today;
            TimeSpan selectedTime = StartingTimePicker.Time ?? TimeSpan.Zero;
            DateTime startDateTime = selectedDate.Add(selectedTime);

            var newHatch = new IncubationPresetModel
            {
                Name = HatchNameEntry.Text,
                Species = BirdTypePicker.SelectedItem?.ToString() ?? string.Empty,
                EggId = BirdTypePicker.SelectedIndex+1,
                EggCount = int.TryParse(EggCountEntry.Text, out int count) ? count : 0,
                IsAutomaticTurner = MachineTypeSwitch.IsToggled,
                StartDate = startDateTime,
                IsActive = true
            };

            int result = await _databaseService.SaveHatchAsync(newHatch);

            var notificationService = new NotificationsService();
            await notificationService.ScheduleNotificationsAsync(eggId: BirdTypePicker.SelectedIndex + 1, hatchId: newHatch.Id, startDate: startDateTime);

            await DisplayAlert("Kurulum Başarılı!", $"Kuluçka Ad    ı: {newHatch.Name}", "Tamam");
            Application.Current.MainPage = new NavigationPage(new AppShell());
        }

        private void CopyDatabaseFromRawAsync()
        {
            Task.Run(async () =>
            {
                try
                {
                    string appDataPath = FileSystem.AppDataDirectory;
                    string dbPath = Path.Combine(appDataPath, "EggInfo.db");

                    using var rawStream = await FileSystem.OpenAppPackageFileAsync("EggInfo.db");
                    using var fileStream = File.Create(dbPath);
                    await rawStream.CopyToAsync(fileStream);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Hata: {ex.Message}");
                }
            });
        }
    }
}