using Manchito.DataBaseContext;
using Manchito.Model;
using Manchito.ViewModel;
using Manchito.Views;


namespace Manchito;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel(this);
		
	}	
	



	
}

