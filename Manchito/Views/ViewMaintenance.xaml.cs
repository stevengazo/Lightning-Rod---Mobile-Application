using Manchito.ViewModel;
namespace Manchito.Views;

public partial class ViewMaintenance : ContentPage
{
	public int MaintenanceId { get; set; }
	public ViewMaintenance()
	{
		InitializeComponent();
		//BindingContext = new  ViewMaintenanceViewModel();
	}
	private void Button_Clicked(object sender, EventArgs e)
	{
		throw new NotImplementedException();
	}
	private async Task ViewItem(object sender, EventArgs r)
	{
		throw new NotImplementedException();
	}
	private async Task AddItem(object sender, EventArgs e)
	{
		throw new NotImplementedException();
	}
}