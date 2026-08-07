using CommunityToolkit.Maui.Views;
using Evonest.EggMonitor.Models;
using Evonest.EggMonitor.Services;

namespace Evonest.EggMonitor;

public partial class SwitchHatchPopup : Popup
{
    public SwitchHatchPopup(List<IncubationPresetModel> hatches)
    {
        InitializeComponent();
        Dispatcher.Dispatch(() =>
        {
            HatchesCollection.ItemsSource = hatches;
        });
    }

    private async void OnHatchSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is IncubationPresetModel selectedHatch)
        {
            var dbService = new InstallationDB();

            await dbService.SwitchActiveHatchAsync(selectedHatch.Id);

            HatchesCollection.SelectedItem = null;

            CloseAsync();
        }
    }
}