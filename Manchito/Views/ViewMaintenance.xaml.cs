namespace Manchito.Views;

public partial class ViewMaintenance : ContentPage
{
	public ViewMaintenance()
	{
		InitializeComponent();
	}

	private void Button_Clicked(object sender, EventArgs e)
	{

    }

	private async void ViewItem(object sender, EventArgs r)
	{
		try
		{
			await Navigation.PushAsync(new ViewItem());
		}catch(Exception ex) {
			await DisplayAlert("Error", $"Error {ex.Message}", null);
		}
	}


	private async void AddItem(object sender, EventArgs e)
	{
		try
		{
			string action = await DisplayActionSheet("¿Qué tipo de item deseas añadir?", "Cancelar", null, "Pararrayos", "Montaje", "Bajante", "Sistema Puesta a Tierra", "Unificaciones", "Supresores");

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