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


	private async void AddItem(object sender, EventArgs e)
	{
		try
		{
			await Navigation.PushAsync(new AddItem());
		}catch (Exception ex)
		{
			await DisplayAlert("Error", $"Error {ex.Message}", null);
		}
	}
}