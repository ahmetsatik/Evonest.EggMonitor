using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Evonest.EggMonitor.Models;
using Evonest.EggMonitor.Services;
using SQLite;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Evonest.EggMonitor
{
    public partial class MainPage : ContentPage
    {
        private readonly DayCount _dayCount;

        private SQLiteAsyncConnection _db;
        public ObservableCollection<InstructionModel> IncubationStages { get; set; } = new();

        public ICommand StageSelectedCommand { get; set; }
        public MainPage()
        {
            InitializeComponent();

            _dayCount = new DayCount();

            _ = _dayCount.DayCounter();
            _ =Day();
            CopyDatabaseFromRawAsync();
            BindingContext = this;
            StageSelectedCommand = new Command<InstructionModel>(OnStageSelected);

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RequestNotificationPermissionAsync();
            await LoadIncubationStagesAsync();

            await Task.Delay(1000);
            await LoadIncubationStagesAsync();

        }

        private async Task RequestNotificationPermissionAsync()
        {
            try
            {
                
                if (await Plugin.LocalNotification.LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
                {
                    await Plugin.LocalNotification.LocalNotificationCenter.Current.RequestNotificationPermission();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"İzin istenirken hata oluştu: {ex.Message}");
            }
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

        private async Task LoadIncubationStagesAsync()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "EggInfo.db");
            _db = new SQLiteAsyncConnection(dbPath);

            
            var databaseResult = await _db.Table<InstructionModel>()
                                      .Where(x => x.EggID == _dayCount.ActiveEggId)
                                      .ToListAsync();

            IncubationStages.Clear();
            foreach (var instruction in databaseResult)
            {
                IncubationStages.Add(instruction);
            }
        }

        private async void OnStageSelected(InstructionModel selectedStage)
        {
            
            if (selectedStage == null) return;

            var popup = new IncubationStagesPopup();
            popup.BindingContext = selectedStage;
            await this.ShowPopupAsync(popup);
            
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
        private async void TrackSaveClicked(object sender, EventArgs e)
        {
            try
            {
                var dbService = new InstallationDB();
                var activeHatch = await dbService.GetActiveHatchAsync();
                var dbService2 = new TrackersService();
                int currentDay = _dayCount.elapsedTime;

                if (activeHatch == null)
                {
                    await DisplayAlert("Hata", "Lütfen önce bir kuluçka kurulumu yapın!", "Tamam");
                    return;
                }

                else
                {
                    bool isRecordExists = await dbService2.CheckRecordExistsAsync(activeHatch.Id, currentDay);

                    if (isRecordExists)
                    {

                        await DisplayAlert("Uyarı", $"Bugün ({currentDay}. gün) için zaten bir veri girdiniz. Aynı güne birden fazla kayıt eklenemez!", "Tamam");
                        return;
                    }
                    var newTracker = new TrackersModel
                    {
                        IncubationId = activeHatch.Id,
                        DayNumber = currentDay,
                        DateText = DateTime.Now.ToString("dd MMMM yyyy"),
                        TempAndHumidity = $"{DailyTemp.Text}°C / {DailyHumudity.Text}%",
                        Description = DailyDescription.Text
                    };
                    await dbService2.SaveTrackerAsync(newTracker);
                    await DisplayAlert("Başarılı", "Günlük veriler kaydedildi.", "Tamam");
                }

            }
            catch (Exception ex) { await DisplayAlert("Hata", $"Kayıt sırasında bir sorun oluştu: {ex.Message}", "Tamam"); } 
            }

}
}
    