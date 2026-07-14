namespace Evonest.EggMonitor;

using Evonest.EggMonitor.Models;
using Evonest.EggMonitor.Services;
using System;
using System.Collections.ObjectModel;

public partial class TrackerPage : ContentPage
{
    public ObservableCollection<TrackersModel> TrackerItems { get; set; }

    public TrackerPage()
    {
        InitializeComponent();

        TrackerItems = new ObservableCollection<TrackersModel>();

        BindingContext = this;

        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTrackerDataAsync();
    }

    private async Task LoadTrackerDataAsync()
    {
        try
        {
            var dbService = new InstallationDB();
            var trackerService = new TrackersService();

            
            var activeHatch = await dbService.GetActiveHatchAsync();

            
            if (activeHatch == null)
            {
                TrackerItems.Clear();
                return;
            }

            
            var records = await trackerService.GetTrackersAsync(activeHatch.Id);

            
            TrackerItems.Clear();

            foreach (var record in records)
            {
                TrackerItems.Add(record);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Veriler yüklenirken bir sorun oluştu: {ex.Message}", "Tamam");
        }
    }
}