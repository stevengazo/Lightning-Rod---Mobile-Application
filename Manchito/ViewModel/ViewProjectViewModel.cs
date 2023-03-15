using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Manchito.DataBaseContext;
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

		private ViewProject _ViewProject { get; set; }
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
			LoadProject();
			// binding command add Project
			AddMaintenanceCommand = new Command(() => { AddMaintenance(); });
			// binding command Delete Project
			DeleteProjectCommand = new Command(() =>
			 {
				 DeleteProject();
			 });
			// binding command view maintenance
			ViewMaintenanceCommand = new Command((t) => { ViewMaintenance(t); });
			// binding command update project
			UpdateProjectCommand = new Command(() => { UpdateProject(); });
			
		}

		private async void ViewMaintenance(object idNumber)
		{
			try
			{
				int number = int.Parse(idNumber.ToString());
				ViewMaintenance viewMaintenance = new ViewMaintenance(number);
				await _ViewProject.Navigation.PushAsync(viewMaintenance);

			}
			catch (Exception ex)
			{
				await _ViewProject.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
			}
		}
		private async Task AddMaintenance()
		{
			try
			{
				Model.Maintenance tmp = new Maintenance();
				tmp.ProjectId = Project.ProjectId;
				tmp.MaintenanceId = GetLastMaintenanceId() + 1;
				tmp.DateOfMaintenance = DateTime.Now;
				tmp.Alias = await _ViewProject.DisplayPromptAsync("Ingreso mantenimiento", "¿Deseas añadir un mantenimiento?", "Agregar", "Cancelar", null, 50);
				tmp.Status = await _ViewProject.DisplayActionSheet("Estatus del mantenimiento", "Cancelar", null, "En ejecución", "Concluido", "Pendiente ejecución");
				if (!string.IsNullOrEmpty(tmp.Alias) && !string.IsNullOrEmpty(tmp.Status) && !tmp.Status.Equals("Cancelar"))
				{
					using (var dbLocal = new DBLocalContext())
					{
						dbLocal.Add(tmp);
						dbLocal.SaveChanges();
						await LoadMaintenances();
						await _ViewProject.DisplayAlert("Informacion", "Mantenimiento Agregado", "OK");
						// base items of the maintenance
						List<Category> itemsCategories = new List<Category>();
						using (var local = new DBLocalContext())
						{
							var itemstypes = local.ItemTypes.ToList();
							foreach (var item in itemstypes)
							{
								Category categoryTmp = new Category();
								categoryTmp.CategoryId = await lastCategoryId() + 1;
								categoryTmp.Alias = "No Asignado";
								categoryTmp.ItemTypeId = item.ItemTypeId;
								categoryTmp.MaintenanceId = tmp.MaintenanceId;
								dbLocal.Add(categoryTmp);
								dbLocal.SaveChanges();								
							}
							await _ViewProject.DisplayAlert("Informacion", "Items por defecto agregados al mantenimiento", "OK");
						}
					}
				}
			}
			catch (Exception ex)
			{
				await _ViewProject.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
				_ViewProject.Navigation.RemovePage(_ViewProject);
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


		private async Task DeleteProject()
		{
			try
			{
				var result = await _ViewProject.DisplayAlert("Alerta", "Deseas borrar este dato", "Yes", "No");
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
					await _ViewProject.DisplayAlert("Información", $"Proyecto eliminado", "Ok");
					_ViewProject.Navigation.RemovePage(_ViewProject);
				}
			}
			catch (Exception ex)
			{
				await _ViewProject.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
				_ViewProject.Navigation.RemovePage(_ViewProject);
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
		private async Task< Project> GetProject(int idProject)
		{
			try
			{
				using (var dblocal = new DBLocalContext())
				{
					var queryResult = (from proj in dblocal.Project
									   where proj.ProjectId == idProject
									   select proj).FirstOrDefault();
					return queryResult;
				}
			}
			catch (Exception ex)
			{
				Application.Current.MainPage.DisplayAlert("Error", $"Error en function GetProjet, {ex.Message}", "ok");
				return null;
			}

		}

		private void LoadProject()
		{
			try
			{
				var tmp = 0;
				MessagingCenter.Subscribe<MainPageViewModel,int>(this, "Hi", async (sender, arg) =>
				{
					tmp = int.Parse(arg.ToString());
					Project = await GetProject(tmp);
				});
				//MessagingCenter.Unsubscribe<MainPageViewModel, int>(this, "Hi");
			}catch (Exception ex)
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
					Maintenances = dbLocal.Maintenance.Where(M => M.ProjectId == Project.ProjectId).ToList();
				}
			}
			catch (Exception ex)
			{
				await _ViewProject.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
			}
		}

		private async Task UpdateProject()
		{
			try
			{
				UpdateProject UpdateView = new UpdateProject(Project.ProjectId);
				await _ViewProject.Navigation.PushAsync(UpdateView);
			}
			catch (Exception f)
			{
				await _ViewProject.DisplayAlert("Error interno", $"Error interno, intentelo mas tarde. {f.Message}", "OK");
			}

		}
		#endregion
	}
}
