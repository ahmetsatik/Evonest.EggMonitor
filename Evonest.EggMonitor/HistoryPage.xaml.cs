using Evonest.EggMonitor.Services;

namespace Evonest.EggMonitor;

public partial class HistoryPage : ContentPage
{
    public HistoryPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadHistoryDataAsync();
    }

    private async Task LoadHistoryDataAsync()
    {
        var dbService = new InstallationDB();

        var finishedHatches = await dbService.GetFinishedHatchesAsync();

        if (finishedHatches != null && finishedHatches.Count > 0)
        {
            HistoryCollectionView.ItemsSource = finishedHatches;
            EmptyStateLabel.IsVisible = false; 
        }
        else
        {
            HistoryCollectionView.ItemsSource = null;
            EmptyStateLabel.IsVisible = true;
        }
    }
}