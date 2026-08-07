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
            _ = Day();
            BindingContext = this;
            StageSelectedCommand = new Command<InstructionModel>(OnStageSelected);

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadIncubationStagesAsync();

            await Task.Delay(1000);
            await LoadIncubationStagesAsync();
            await CheckTriggersAsync();


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
                    if (Convert.ToInt32(CurrentDayLabel.Text) == 0)
                    {
                        CurrentDayLabel.Text = "1";
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

                    string warning = await dbService2.TempAndHumunityCheckAsync(
                                activeHatch.EggId,
                                currentDay,
                                DailyHumudity.Text,
                                DailyTemp.Text);

                    if (!string.IsNullOrWhiteSpace(warning))
                    {
                        bool userWantsToSave = await DisplayAlert("Kritik Uyarı",
                                                                  warning + "\n\nYine de bu tehlikeli değerleri kaydetmek istiyor musunuz?",
                                                                  "Evet, Kaydet",
                                                                  "İptal Et");

                        if (!userWantsToSave)
                        {
                            return;
                        }
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

        private async void OnAddHatchClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewHatchPage());
        }

        private async void OnSwitchHatch(object sender, EventArgs e)
        {
            var dbService = new InstallationDB();

            var allHatches = await dbService.GetAllHatchesAsync();

            if (allHatches.Count == 0)
            {
                await DisplayAlert("Bilgi", "Kayıtlı kuluçka bulunamadı.", "Tamam");
                return;
            }

            var popup = new SwitchHatchPopup(allHatches);

            await this.ShowPopupAsync(popup);

            await RefreshMainPageDataAsync();
        }
        private async Task RefreshMainPageDataAsync()
        {
            var dbService = new InstallationDB();
            var activeHatch = await dbService.GetActiveHatchAsync();

            if (activeHatch != null)
            {
                _dayCount.ActiveEggId = activeHatch.EggId;
                await _dayCount.DayCounter();
                await CheckTriggersAsync();
                await LoadIncubationStagesAsync();
            }
            else
            {
                _dayCount.ActiveEggId = 0;
                IncubationStages.Clear();
                CurrentDayLabel.Text = "Kuluçka yok";

                await DisplayAlert("Bilgi", "Şu an devam eden aktif bir kuluçkanız bulunmuyor.", "Tamam");
            }
        }

        private async Task CheckTriggersAsync()
        {
            var dbService = new InstallationDB();
            var activeHatch = await dbService.GetActiveHatchAsync();
            

            if (activeHatch == null || activeHatch.IsFinished == true) return;

            int currentDay = (DateTime.Now.Date - activeHatch.StartDate.Date).Days;

            string eggInfoPath = Path.Combine(FileSystem.AppDataDirectory, "EggInfo.db");
            var eggInfoConnection = new SQLiteAsyncConnection(eggInfoPath);

            var instructions = await eggInfoConnection.Table<InstructionModel>()
                                                      .Where(x => x.EggID == activeHatch.EggId)
                                                      .ToListAsync();

            if (instructions == null || !instructions.Any()) return;

            var currentPhase = instructions.FirstOrDefault(i => currentDay >= i.StartingDay && currentDay <= i.EndingDay);
            var lastPhase = instructions.OrderByDescending(i => i.EndingDay).FirstOrDefault();

            if (currentPhase != null)
            {
                bool containsDol = currentPhase.Instructions.IndexOf("döl", StringComparison.OrdinalIgnoreCase) >= 0;

                if (containsDol && activeHatch.FertileEggCount == 0)
                {
                    string result = await DisplayPromptAsync("1. Döl Kontrolü Vakti! 🔦",
                        $"{currentDay}. Güne ulaştık. (Evre: {currentPhase.StartingDay}-{currentPhase.EndingDay}. Gün)\n\nBaşlangıçta {activeHatch.EggCount} yumurta koymuştunuz. Boşları ayırıp kalan SAĞLAM yumurta sayısını giriniz:",
                        keyboard: Keyboard.Numeric);

                    if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int fertileCount))
                    {
                        activeHatch.FertileEggCount = fertileCount;
                        await dbService.UpdateHatchAsync(activeHatch);
                        await DisplayAlert("Kaydedildi", "Döl kontrolü veriniz başarıyla işlendi.", "Tamam");
                    }
                }
            }

            if (lastPhase != null && currentDay >= lastPhase.StartingDay && currentDay < lastPhase.EndingDay)
            {
                string prefKey = $"SonKontrolYapildi_{activeHatch.Id}";
                bool isSecondCheckDone = Preferences.Get(prefKey, false);

                if (!isSecondCheckDone)
                {
                    int mevcutYumurta = activeHatch.FertileEggCount > 0 ? activeHatch.FertileEggCount : activeHatch.EggCount;

                    string result = await DisplayPromptAsync("Çıkım Öncesi Son Kontrol! 🔦",
                        $"{currentDay}. Güne ulaştık ve son evre başladı.\n\nSistemde {mevcutYumurta} sağlam yumurta görünüyor. Çıkım sepetine alırken elediğiniz fireler varsa, kalan NET SAĞLAM yumurta sayısını giriniz:",
                        keyboard: Keyboard.Numeric);

                    if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int finalFertileCount))
                    {
                        activeHatch.FertileEggCount = finalFertileCount;
                        await dbService.UpdateHatchAsync(activeHatch);
                        Preferences.Set(prefKey, true); // Bir daha sormaması için işaretliyoruz
                        await DisplayAlert("Kaydedildi", "Çıkım öncesi son kontrol veriniz işlendi.", "Tamam");
                    }
                }
            }



            lastPhase = instructions.OrderByDescending(i => i.EndingDay).FirstOrDefault();

            if (lastPhase != null && currentDay >= lastPhase.EndingDay && activeHatch.IsFinished == false)
            {
                string result = await DisplayPromptAsync("Kuluçka Tamamlandı! 🎉",
                    $"{currentDay}. Güne ulaştık, kuluçka sürecinin son günü! Lütfen çıkan sağlıklı civciv sayısını giriniz:",
                    keyboard: Keyboard.Numeric);

                if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int hatchedCount))
                {
                    activeHatch.HatchedChickCount = hatchedCount;
                    activeHatch.IsFinished = true;
                    activeHatch.IsActive = false;

                    int baseCount = activeHatch.FertileEggCount > 0 ? activeHatch.FertileEggCount : activeHatch.EggCount;
                    if (baseCount > 0)
                    {
                        activeHatch.SuccessRate = Math.Round((double)hatchedCount / baseCount * 100, 1);
                    }

                    await dbService.UpdateHatchAsync(activeHatch);
                    await DisplayAlert("Tebrikler!", $"Kuluçka raporu başarıyla oluşturuldu.\nBaşarı Oranınız: %{activeHatch.SuccessRate}", "Tamam");

                    await RefreshMainPageDataAsync();
                }
            }
        }

        private async void OnManual(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ManualPage());
        }

        private async void OnSettings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings());
        }

        private async void OnHistory(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPage());
        }

        private async void OnAdvanceDayClicked(object sender, EventArgs e)
        {
            var dbService = new InstallationDB();
            var activeHatch = await dbService.GetActiveHatchAsync();

            if (activeHatch != null)
            {
                // 1. Sistemi Kandırma: Veritabanındaki başlangıç tarihini 1 gün geriye alıyoruz
                activeHatch.StartDate = activeHatch.StartDate.AddDays(-1);
                await dbService.UpdateHatchAsync(activeHatch);

                // 2. Yeni Günü Gerçek Zamanlı Hesaplama (Saat küsuratları olmadan, temiz gün)
                int gercekGun = (DateTime.Now.Date - activeHatch.StartDate.Date).Days;

                // 3. Ekranda Günü Gösteren Label'ı Zorla Güncelleme
                // DİKKAT: XAML tarafında günü gösterdiğin Label'ın x:Name'i neyse onu yaz (Örn: GunLabel)
                // GunLabel.Text = gercekGun.ToString(); 

                // 4. Hiçbir salt okunur değişkene dokunmadan, bulduğumuz günü direkt metoda yolluyoruz!
                await CheckTriggersAsync();
            }
        }
    }
}
