namespace Manchito;

public partial class AgregarProyecto : ContentPage
{
	public AgregarProyecto()
	{
		InitializeComponent();
	}

	private async void OnClickClosed(object sender, EventArgs e)
	{
	Navigation.RemovePage(this);
	}
}