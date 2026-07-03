using System;
using Microsoft.Maui.Controls;
using Evonest.EggMonitor.Models;

namespace Evonest.EggMonitor
{
    public partial class InstallationPage : ContentPage
    {
        public InstallationPage()
        {
            InitializeComponent();
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

            var newHatch = new IncubationModel
            {
                Name = HatchNameEntry.Text,
                Species = BirdTypePicker.SelectedItem?.ToString() ?? string.Empty,
                EggCount = int.TryParse(EggCountEntry.Text, out int count) ? count : 0,
                IsAutomaticTurner = MachineTypeSwitch.IsToggled,
                StartDate = startDateTime
            };

            await DisplayAlert("Kurulum Başarılı!", $"Kuluçka Adı: {newHatch.Name}", "Tamam");
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}