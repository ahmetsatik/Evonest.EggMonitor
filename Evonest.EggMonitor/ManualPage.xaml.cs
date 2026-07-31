namespace Evonest.EggMonitor;

public partial class ManualPage : ContentPage
{
    public ManualPage()
    {
        InitializeComponent();
    }

    private void OpenValuesModal(object sender, EventArgs e)
    {
        OverlayBackground.IsVisible = true;
        ValuesModal.IsVisible = true;
    }

    private void OpenFaqModal(object sender, EventArgs e)
    {
        OverlayBackground.IsVisible = true;
        FaqModal.IsVisible = true;
    }

    private void CloseAllModals(object sender, EventArgs e)
    {
        OverlayBackground.IsVisible = false;
        ValuesModal.IsVisible = false;
        FaqModal.IsVisible = false;
    }

    private void AccordionClicked(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Grid contentGrid)
        {
            contentGrid.IsVisible = !contentGrid.IsVisible;
        }
    }
}