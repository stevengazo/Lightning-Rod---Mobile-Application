namespace Manchito.Views;

public partial class AddProject : ContentPage
{
    public AddProject()
    {
        InitializeComponent();
        //BindingContext = new AddProjectViewModel(this);
    }

    private async void OnClickClosed(object sender, EventArgs e)
    {
        Navigation.RemovePage(this);
    }
}