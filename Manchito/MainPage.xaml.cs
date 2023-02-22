using Manchito.ViewModel;
using Manchito.Views;

namespace Manchito;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();		
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
	
	}
	private async void AddProject(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new AddProject());
		
	}

	private async void ViewProject(object sender, EventArgs e)
	{
		try
		{
			await Navigation.PushAsync(new ViewProject());	
		}catch (Exception ex) { 
		}
	}
}

