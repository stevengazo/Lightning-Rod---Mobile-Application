using CommunityToolkit.Mvvm.Messaging;
using Manchito.DataBaseContext;
using Manchito.Messages;
using Manchito.Model;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Windows.Input;

namespace Manchito.ViewModel
{
	public class ValidateDataViewModel : INotifyPropertyChangedAbst
	{
		#region Properties
		private string _Title;

		public string Title
		{
			get { return _Title; }
			set { _Title = value;
			if(Title!= null)
				{
					OnPropertyChanged("Title");
				}
			}
		}

		private Maintenance _Maintenance;
		public Maintenance Maintenance
		{
			get { return _Maintenance; }
			set
			{
				_Maintenance = value;
				if (value != null)
				{
					OnPropertyChanged(nameof(Maintenance));
				}
			}
		}
		private List<Category> _Categories;
		public List<Category> Categories
		{
			get { return _Categories; }
			set
			{
				_Categories = value;
				if (Categories != null)
				{
					OnPropertyChanged(nameof(Categories));
				}
			}
		}
		public ICommand ShareMaintenanceCommand { get; private set; }
		public ICommand AppearingCommand { get; private set; }
		#endregion
		#region Methods
		public ValidateDataViewModel()
		{
			Title = "";
			AppearingCommand = new Command(async () => await LoadManteinance());
			ShareMaintenanceCommand = new Command( () => ShareMaintenance());
		}
		private async Task LoadCategories()
		{
			try
			{
				using var db = new DBLocalContext();

				Categories = db.Category.Where(C => C.MaintenanceId == Maintenance.MaintenanceId)
										.Include(C => C.ItemType)
										.Include(M => M.Photographies.Take(3)).ToList();

			}
			catch (Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error LoadCategories ", $"Error: {f.Message}", "ok");
			}
		}
		private async Task LoadManteinance()
		{
			try
			{
				var d = WeakReferenceMessenger.Default.IsRegistered<NameItemViewMessage>(this);
				if (!d)
				{
					if (Maintenance == null)
					{
						WeakReferenceMessenger.Default.Register<NameItemViewMessage>(this, async (r, m) =>
						{
							using (var db = new DBLocalContext())
							{
								Maintenance = await db.Maintenance.Where(M => M.MaintenanceId == m.Value).Include(M => M.Project).SingleOrDefaultAsync();
								Title = $"Validación {Maintenance.Alias}";
							}
							if (Maintenance != null)
							{
								Title = $"Validación {Maintenance.Alias}";
								await LoadCategories();
							}
						});
					}
					else
					{
						LoadCategories();
					}
				}
				else
				{
					if (Maintenance != null)
					{
						LoadCategories();
					}
					WeakReferenceMessenger.Default.Unregister<NameItemViewMessage>(this);
				}
			}
			catch (Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error LoadMaintenance", $"Error {f.Message}", "OK");
			}
		}
		private void ShareMaintenance()
		{
			try
			{
				string startPath = Path.Combine(
									PathDirectoryFilesAndroid,
									$"P-{Maintenance.Project.ProjectId}_{Maintenance.Project.Name}",
									$"M-{Maintenance.MaintenanceId}_{Maintenance.Alias}");
				string zipPath = Path.Combine(
										FileSystem.CacheDirectory,
										$"P-{Maintenance.Project.Name}-{Maintenance.Alias}.zip");
				if (File.Exists(zipPath))
				{
					// create new zip file
					File.Delete(zipPath);
				}
				ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Optimal, true);
				if (File.Exists(zipPath))
				{
					Share.Default.RequestAsync(new ShareFileRequest
					{
						Title = "Compartir Archivo",
						File = new ShareFile(zipPath)
					});
				}
			}
			catch (Exception f)
			{
				Application.Current.MainPage.DisplayAlert("Error ShareMaintenance", $"Error interno {f.Message}", "Ok");
			}
		}
		#endregion
	}
}
