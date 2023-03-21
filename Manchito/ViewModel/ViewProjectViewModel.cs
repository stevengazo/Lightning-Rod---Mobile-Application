using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Manchito.DataBaseContext;
using Manchito.Messages;
using Manchito.Model;
using Manchito.Views;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Manchito.ViewModel
{
	public class ViewProjectViewModel : INotifyPropertyChangedAbst
	{
		#region Properties
		public ICommand AddMaintenanceCommand { get; private set; }
		public ICommand ViewMaintenanceCommand { get; private set; }
		public ICommand DeleteProjectCommand { get; private set; }
		public ICommand UpdateProjectCommand { get; private set; }

		public ICommand AppearingCommand { get; private set; }
		public int ProjectIdEXternal { get; set; }
		private List<Maintenance> _Maintenances;

		public List<Maintenance> Maintenances
		{
			get { return _Maintenances; }
			set
			{
				_Maintenances = value;
				if (Maintenances != null)
				{
					OnPropertyChanged(nameof(Maintenances));
				}
			}
		}		
		private Project _Project;
		public Project Project
		{
			get { return _Project; }
			set
			{
				_Project = value;
				if (Project != null)
				{
					OnPropertyChanged(nameof(Project));
				}
			}
		}
		#endregion

		#region Methods

		/// <summary>
		/// Constructor of the class ViewProjectViewModel
		/// </summary>
		/// <param name="nav">Navigation</param>
		/// <param name="_viewProject">View how implement the view</param>
		public ViewProjectViewModel()
		{
			// binding commands to the Property
			AddMaintenanceCommand = new Command(() => { AddMaintenance(); });		
			DeleteProjectCommand = new Command(() => { DeleteProject(); });			
			ViewMaintenanceCommand = new Command((t) => { ViewMaintenance(t); });			
			UpdateProjectCommand = new Command(() => UpdateProject() );
			AppearingCommand = new Command(() => { LoadProjectCommand(); } );
		}		
		private async Task AddMaintenance()
		{
			try
			{
				Model.Maintenance tmp = new Maintenance()
				{
					ProjectId = Project.ProjectId,
					MaintenanceId = GetLastMaintenanceId()+1,
					DateOfMaintenance= DateTime.Now
				};						
				tmp.Alias = await Application.Current.MainPage.DisplayPromptAsync("Ingreso mantenimiento", "¿Deseas añadir un mantenimiento?", "Agregar", "Cancelar", null, 50);
				tmp.Status = await Application.Current.MainPage.DisplayActionSheet("Estatus del mantenimiento", "Cancelar", null, "En ejecución", "Concluido", "Pendiente ejecución");
				if (!string.IsNullOrEmpty(tmp.Alias) && !string.IsNullOrEmpty(tmp.Status) && !tmp.Status.Equals("Cancelar"))
				{
					using (var dbLocal = new DBLocalContext())
					{
						dbLocal.Add(tmp);
						dbLocal.SaveChanges();
						await LoadMaintenances();
						await Application.Current.MainPage.DisplayAlert("Informacion", "Mantenimiento Agregado", "OK");
						// base items of the maintenance
						List<Category> itemsCategories = new List<Category>();
						using (var local = new DBLocalContext())
						{
							var itemstypes = local.ItemTypes.ToList();
							foreach (var item in itemstypes)
							{
								Category categoryTmp = new Category()
								{
									CategoryId = await lastCategoryId() + 1,
									Alias = "No Asignado",
									ItemTypeId = item.ItemTypeId,
									MaintenanceId = tmp.MaintenanceId
								};														
								dbLocal.Add(categoryTmp);
								dbLocal.SaveChanges();								
							}
							//await Application.Current.MainPage.DisplayAlert("Informacion", "Items por defecto agregados al mantenimiento", "OK");
						}
					}
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
				await ClosePage();
			}
		}		
		private async Task ClosePage()
		{
			var page = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
			Application.Current.MainPage.Navigation.RemovePage(page);
		}
		private async Task DeleteProject()
		{
			try
			{
				var result = await Application.Current.MainPage.DisplayAlert("Alerta", "Deseas borrar este dato", "Yes", "No");
				if (result.Equals(true))
				{
					using (var dbLocal = new DBLocalContext())
					{	
						var query = (from maint in dbLocal.Maintenance where maint.ProjectId == Project.ProjectId select maint).ToList();
						dbLocal.RemoveRange(query);
						dbLocal.SaveChanges();
						dbLocal.Project.Remove(Project);
						dbLocal.SaveChanges();
					}
					await Application.Current.MainPage.DisplayAlert("Información", $"Proyecto eliminado", "Ok");
					await ClosePage();
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
				await ClosePage();
			}
		}
		private int GetLastMaintenanceId()
		{
			using (var dbLocal = new DBLocalContext())
			{
				int id = (from main in dbLocal.Maintenance
						  orderby main.MaintenanceId descending
						  select main.MaintenanceId).FirstOrDefault();
				return id;
			}
		}
		private async Task<int> lastCategoryId()
		{
			using (var db = new DBLocalContext())
			{
				int id = (from iT in db.Category
						  orderby iT.CategoryId descending
						  select iT.CategoryId).FirstOrDefault();
				return id;
			}
		}
		private async Task LoadProjectCommand()
		{
			try
			{
				if(Project ==null)
				{
					WeakReferenceMessenger.Default.Register<ProjectViewMessage>(this, async (r, m) => {
						if (m.Value != 0)
						{
							using (var db = new DBLocalContext())
							{
								Project = db.Project.Where(P => P.ProjectId == m.Value).FirstOrDefault();
							}
							if (Project != null)
							{
								LoadMaintenances();
							}
						}
					});
				}
			}
			catch (Exception ex)
			{
				Application.Current.MainPage.DisplayAlert("error", $"Eror {ex.Message}", "OK");
			}			
		}
		private async Task LoadMaintenances()
		{
			try
			{
				
				using (var dbLocal = new DBLocalContext())
				{
					 var tmp= dbLocal.Maintenance.Where(M => M.ProjectId == Project.ProjectId).ToList();
					if ((tmp.Count > 0))
					{
						Maintenances = tmp;
					}
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
			}
		}
		private async Task UpdateProject()
		{
			try
			{
				UpdateProject UpdateView = new UpdateProject(Project.ProjectId);
				await Application.Current.MainPage.Navigation.PushAsync(UpdateView);
			}
			catch (Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error interno", $"Error interno, intentelo mas tarde. {f.Message}", "OK");
			}

		}
		private async void ViewMaintenance(object idNumber)
		{
			try
			{				
				int number = int.Parse(idNumber.ToString());				
				ViewMaintenance vMaintPage = new ViewMaintenance();				
				await Application.Current.MainPage.Navigation.PushAsync(vMaintPage);
				WeakReferenceMessenger.Default.Send(new MaintenanceViewMessage(number));
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
				await ClosePage();
			}
		}
		#endregion
	}
}
