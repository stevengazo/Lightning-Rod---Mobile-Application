namespace Manchito.Views;

public partial class AddProject : ContentPage
{
	public AddProject()
	{
		InitializeComponent();
	}

	private async void OnClickClosed(object sender, EventArgs e)
	{
		Navigation.RemovePage(this);
	}
}