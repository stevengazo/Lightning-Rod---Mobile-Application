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

    }

	private async void ViewItem(object sender, EventArgs r)
	{		
	}
	private async void AddItem(object sender, EventArgs e)
	{
		try
		{
			string action = await DisplayActionSheet("�Qu� tipo de item deseas a�adir?", "Cancelar", null, "Pararrayos", "Montaje", "Bajante", "Sistema Puesta a Tierra", "Unificaciones", "Supresores");
			if (action != null)
			{
				if (!action.Equals("Cancelar"))
				{
					string result = await DisplayPromptAsync("Alias", "Digite el nombre del item a agregar");
				}
			}
			//await Navigation.PushAsync(new AddItem());
		}catch (Exception ex)
		{
			await DisplayAlert("Error", $"Error {ex.Message}", null);
		}
	}
}