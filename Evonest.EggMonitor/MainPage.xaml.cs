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

        private bool _isBusy = false;

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
            if (_isBusy || selectedStage == null) return;
            _isBusy = true;

            try
            {
                var popup = new IncubationStagesPopup();
                popup.BindingContext = selectedStage;
                await this.ShowPopupAsync(popup);
            }
            finally
            {
                _isBusy = false;
            }
        }

        private async void OnShowHatchClicked(object sender, EventArgs e)
        {
            if (_isBusy) return;
            _isBusy = true;

            try
            {
                var dbService = new InstallationDB();
                IncubationPresetModel hatch = await dbService.ShowHatch();

                if (hatch != null)
                {
                    var popup = new HatchDetailsPopup(hatch);
                    await this.ShowPopupAsync(popup);
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
            finally
            {
                _isBusy = false;
            }
        }

        private async Task Day()
        {
            while (true)
            {
                MainThread.BeginInvokeOnMainThread(async() =>
                {
                    var dbService = new InstallationDB();
                    var activeHatch = await dbService.GetActiveHatchAsync();
                    int gun = _dayCount.elapsedTime;


                    if (activeHatch == null || activeHatch.IsFinished == true)
                    {
                        CurrentDayLabel.Text = "-"; 
                    }
                    else { 
                            
                    if (gun == 0)
                    {
                        CurrentDayLabel.Text = "-";
                    }
                    else
                    {
                        CurrentDayLabel.Text = Convert.ToString(gun);
                    }
                    }
                });

                await Task.Delay(1000);
            }
        }

        private async void TrackSaveClicked(object sender, EventArgs e)
        {
            if (_isBusy) return;
            _isBusy = true;

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

                        if (!userWantsToSave) return;
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
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Kayıt sırasında bir sorun oluştu: {ex.Message}", "Tamam");
            }
            finally
            {
                _isBusy = false;
            }
        }

        private async void OnAddHatchClicked(object sender, EventArgs e)
        {
            if (_isBusy) return;
            _isBusy = true;

            try { await Navigation.PushAsync(new NewHatchPage()); }
            finally { _isBusy = false; }
        }

        private async void OnSwitchHatch(object sender, EventArgs e)
        {
            if (_isBusy) return;
            _isBusy = true;

            try
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
            finally
            {
                _isBusy = false;
            }
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
                CurrentDayLabel.Text = "-";
                await DisplayAlert("Bilgi", "Şu an devam eden aktif bir kuluçkanız bulunmuyor.", "Tamam");
            }
        }

        private async Task CheckTriggersAsync()
        {
            var dbService = new InstallationDB();
            var activeHatch = await dbService.GetActiveHatchAsync();

            if (activeHatch == null || activeHatch.IsFinished == true) return;

            int currentDay = (DateTime.Now.Date - activeHatch.StartDate.Date).Days + 1;

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
                        Preferences.Set(prefKey, true);
                        await DisplayAlert("Kaydedildi", "Çıkım öncesi son kontrol veriniz işlendi.", "Tamam");
                    }
                }
            }

            lastPhase = instructions.OrderByDescending(i => i.EndingDay).FirstOrDefault();

            if (lastPhase != null && currentDay >= lastPhase.EndingDay && activeHatch.IsFinished == false)
            {
                string promptKey = $"CikimSoruldu_{activeHatch.Id}_{currentDay}";
                bool isAskedToday = Preferences.Get(promptKey, false);

                if (!isAskedToday)
                {
                    string result = await DisplayPromptAsync("Çıkım Süreci Devam Ediyor 🐣",
                        $"{currentDay}. gündeyiz. Şu ana kadar toplam {activeHatch.HatchedChickCount} civciv çıktı.\n\nBugün yeni çıkan civciv varsa sayısını giriniz (Yoksa boş bırakın veya 0 yazın):",
                        keyboard: Keyboard.Numeric);

                    if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int bugunCikan) && bugunCikan > 0)
                    {
                        activeHatch.HatchedChickCount += bugunCikan;
                        await dbService.UpdateHatchAsync(activeHatch);
                        await DisplayAlert("Harika!", $"Bugün çıkanlarla birlikte toplam {activeHatch.HatchedChickCount} civciviniz oldu.", "Tamam");
                    }

                    bool bitir = await DisplayAlert("Kuluçka Durumu",
                        "Makinede çıkım beklediğiniz başka sağlam yumurta kaldı mı?\n\nEğer çıkımlar tamamen bittiyse ve makineyi kapatacaksanız kuluçkayı sonlandırın.",
                        "Evet, Tamamen Bitir",
                        "Hayır, Beklemeye Devam");

                    if (bitir)
                    {
                        activeHatch.IsFinished = true;
                        activeHatch.IsActive = false;

                        int baseCount = activeHatch.FertileEggCount > 0 ? activeHatch.FertileEggCount : activeHatch.EggCount;
                        if (baseCount > 0)
                        {
                            activeHatch.SuccessRate = Math.Round((double)activeHatch.HatchedChickCount / baseCount * 100, 1);
                        }

                        await dbService.UpdateHatchAsync(activeHatch);
                        await DisplayAlert("Tebrikler! 🎉", $"Kuluçka raporu oluşturuldu.\nToplam Çıkan: {activeHatch.HatchedChickCount}\nBaşarı Oranınız: %{activeHatch.SuccessRate}", "Tamam");

                        await RefreshMainPageDataAsync();
                    }
                    else
                    {
                        Preferences.Set(promptKey, true);
                    }
                }
            }
        }

        private async void OnManual(object sender, EventArgs e)
        {
            if (_isBusy) return;
            _isBusy = true;

            try { await Navigation.PushAsync(new ManualPage()); }
            finally { _isBusy = false; }
        }

        private async void OnSettings(object sender, EventArgs e)
        {
            if (_isBusy) return;
            _isBusy = true;

            try { await Navigation.PushAsync(new Settings()); }
            finally { _isBusy = false; }
        }

        private async void OnHistory(object sender, EventArgs e)
        {
            if (_isBusy) return;
            _isBusy = true;

            try { await Navigation.PushAsync(new HistoryPage()); }
            finally { _isBusy = false; }
        }

        private async void OnAdvanceDayClicked(object sender, EventArgs e)
        {
            if (_isBusy) return;
            _isBusy = true;

            try
            {
                var dbService = new InstallationDB();
                var activeHatch = await dbService.GetActiveHatchAsync();

                if (activeHatch != null)
                {
                    activeHatch.StartDate = activeHatch.StartDate.AddDays(-1);
                    await dbService.UpdateHatchAsync(activeHatch);

                    int gercekGun = (DateTime.Now.Date - activeHatch.StartDate.Date).Days + 1;
                    await CheckTriggersAsync();
                }
            }
            finally
            {
                _isBusy = false;
            }
        }
    }
}