namespace Manchito.Views;

public partial class ViewProjectList : ContentPage
{
	public ViewProjectList()
	{
		InitializeComponent();
	}

    private void btnAgregar_Clicked(object sender, EventArgs e)
    {

		Navigation.PushAsync(new AddProject());
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new ViewProject());

    }
}