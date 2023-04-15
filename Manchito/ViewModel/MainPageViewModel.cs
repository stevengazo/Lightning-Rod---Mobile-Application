using CommunityToolkit.Mvvm.Messaging;
using Manchito.DataBaseContext;
using Manchito.Messages;
using Manchito.Model;
using Manchito.Views;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace Manchito.ViewModel
{
	public class MainPageViewModel : INotifyPropertyChangedAbst
	{
		#region Properties

		public ICommand LoadProjectsCommand { get; private set; }
		public ICommand AddProjectCommand { get; private set; }
		public ICommand ViewProjectCommand { get; private set; }
		private List<Project> _Projects;
		private string _ErrorMessage;
		public string ErrorMessage
		{
			get { return _ErrorMessage; }
			set
			{
				_ErrorMessage = value;
				if (_ErrorMessage != null)
				{
					OnPropertyChanged(nameof(ErrorMessage));
				}
			}
		}
		public List<Project> Projects
		{
			get { return _Projects; }
			set
			{
				_Projects = value;
				if (_Projects != null)
				{
					OnPropertyChanged(nameof(Projects));
				}
			}
		}

		#endregion

		#region Methods
		public MainPageViewModel()
		{
			// binding the icommand property with the async method
			ViewProjectCommand = new Command(async (t) => ViewProject(t));
			// binding the icommand property with the async method
			AddProjectCommand = new Command(async () => await AddProject());
			LoadProjectsCommand = new Command(async () => await LoadProjects());
		}
		public async Task AddProject()
		{
			try
			{
				await Application.Current.MainPage.Navigation.PushAsync(new AddProject());

			}
			catch (Exception ex)
			{
				ErrorMessage = $"Error {ex.Message}";
			}

		}
		public async Task LoadProjects()
		{
			try
			{
				await CheckForStoragePermission();
				// load the projects in the db
				using DBLocalContext db = new DBLocalContext();
				Projects = db.Project.Include(P => P.Maintenances).ToList();
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error interno", $"Error: {ex.Message}", "ok");
			}
		}

		private static async void ViewProject(object t)
		{
			try
			{
				int id = int.Parse(t.ToString());
				await Application.Current.MainPage.Navigation.PushAsync(new ViewProject());
				WeakReferenceMessenger.Default.Send(new ProjectViewMessage(id));
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error interno", $"Error: {ex.Message}", "ok");
			}
		}
		public static async Task CheckForStoragePermission()
		{

			var statusStorageRead = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
			var statusStorageWrite = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
			var statusCamera = await Permissions.CheckStatusAsync<Permissions.Camera>();
			if (statusStorageRead != PermissionStatus.Granted)
			{
				await Permissions.RequestAsync<Permissions.StorageRead>();
			}
			if (statusStorageWrite != PermissionStatus.Granted)
			{
				await Permissions.RequestAsync<Permissions.StorageWrite>();
			}
			if (statusCamera != PermissionStatus.Granted)
			{
				await Permissions.RequestAsync<Permissions.Camera>();
			}

		}
		#endregion
	}
}
