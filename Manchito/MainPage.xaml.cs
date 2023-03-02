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
		//listProjects();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
	
	}
	private async void AddProject(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new AddProject());
	}


	private async void connecdb(object sender, EventArgs e)
	{
		try
		{
			using (var dbconnection = new DBLocalContext())
			{
				Random rnd = new Random();
				int num = rnd.Next(10,1000);
				Project project = new()
				{	
					ProjectId = num,
					Name = "sample 2",
					CustomerContactName = "ejs",
					CustomerName = "saj;lek"
				};
				dbconnection.Project.Add(project);
				dbconnection.SaveChanges();
				await DisplayAlert("Alert", "modificacion en base de datos", "OK");
				await listProjects();
			}
		}catch (Exception f)
		{
			await DisplayAlert("Alert", $"Error: {f.Message}", "OK");
		}
		
	}
	private async void ViewProject(object sender, EventArgs e)
	{
		try
		{
		
			await Navigation.PushAsync(new ViewProject());	
		}catch (Exception ex) { 
		}
	}

	private async Task listProjects()
	{
		try
		{
			var listta = new List<Project>();
			using (var dbconnection = new DBLocalContext())
			{
				listta = dbconnection.Project.ToList();
			}

			if (listta.Count > 0)
			{
				containerlist.Clear();
				foreach (var item in listta)
				{
					Frame tmp = new Frame()
					{
						BorderColor = Colors.Red,
						CornerRadius = 4,
						Padding = 5,
						Content = new StackLayout
						{

							Children =
							{
								new Label { Text = item.Name },
								new Label
								{ Text = item.ProjectId.ToString()
								}
							}

						}
					};
					containerlist.Add(tmp);
				}
			}
		}
		catch (Exception f)
		{
			await DisplayAlert("Alert", $"Error: {f.Message}", "OK");
		}
	}
}

