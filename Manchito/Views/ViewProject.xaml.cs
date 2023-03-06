using System.Diagnostics;
using Manchito.ViewModel;

namespace Manchito.Views;

public partial class ViewProject : ContentPage
{
	public int ProjectId { get; set; }
	public ViewProject()
	{
		if(ProjectId >0)
		{
			InitializeComponent();
			BindingContext = new ViewProjectViewModel(Navigation);
		}
		else
		{
			DisplayAlert("Error", "No se indico un valor", "ok");
			Navigation.RemovePage(this);
		}
	}
}