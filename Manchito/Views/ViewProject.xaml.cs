using System.Diagnostics;

namespace Manchito.Views;

public partial class ViewProject : ContentPage
{
	public ViewProject()
	{
		InitializeComponent();
	}
	private async void AddItem(object sender, EventArgs r)
	{
		try
		{
			await Navigation.PushAsync(new AddItem());
		}
		catch (Exception ex)
		{
		}
	}

	private async void updateProject(object sender, EventArgs r)
	{
		try
		{
			await Navigation.PushAsync(new UpdateProject());
		}catch(Exception ex) { 
		}
	}
	private async void AddMaintenance(object sender, EventArgs a)
	{
		try
		{
			await Navigation.PushAsync(new AddMaintenance());
		}
		catch (Exception ex)
		{

		}
	}
	private async void ViewMaintenance(object sender, EventArgs a)
	{
		try
		{
			await Navigation.PushAsync(new ViewMaintenance());
		}catch(Exception f)
		{

		}
	}

	private async void DeleteProyect(object sender, EventArgs e)
	{
		bool answer = await DisplayAlert("Advertencia!", "¿Estas Seguro que deseas borrar este proyecto?", "Sí", "No");		
	}
}