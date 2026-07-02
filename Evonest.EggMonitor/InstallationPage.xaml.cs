
namespace Evonest.EggMonitor;

public partial class InstallationPage : ContentPage
{
	public InstallationPage()
	{
		InitializeComponent();

		StartingDate.MaximumDate = DateTime.Now;
		
		
	}

	private void OnInstallationClicked(Object sender, EventArgs e)
	{

	}
}