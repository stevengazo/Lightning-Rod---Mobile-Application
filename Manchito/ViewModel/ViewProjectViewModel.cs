using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Manchito.DataBaseContext;
using Manchito.Model;
using Manchito.Views;

namespace Manchito.ViewModel
{
	public class ViewProjectViewModel : INotifyPropertyChangedAbst
	{
		#region Properties
		public ICommand AddMaintenanceCommand { get; private set; }
		public ICommand DeleteProjectCommand { get; private set; }
		public int ProjectIdEXternal { get; set; }
		private INavigation Navigation { get; set; }
		private List<Maintenance> _Maintenances;

		public List<Maintenance> Maintenances
		{
			get { return _Maintenances; }
			set { _Maintenances = value;
				if(Maintenances!= null) {
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
		public ViewProjectViewModel(INavigation nav, ViewProject _viewProject)
		{
			Navigation = nav;
			_ViewProject = _viewProject;
			// refresh the data and the project
			_ViewProject.Appearing += (s, a) =>
			{
				// Logic
				Project = GetProject(ProjectIdEXternal);
				LoadMaintenances();
			};
			// binding command 
			DeleteProjectCommand = new Command(() =>
			{
				DeleteProject();
			});
		}
		private async Task AddMaintenance()
		{
			try
			{

			}catch (Exception ex)
			{
				await _ViewProject.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
				Navigation.RemovePage(_ViewProject);
			}
		}

		private async Task DeleteProject()
		{
			try
			{
				var result = await _ViewProject.DisplayAlert("Alerta", "Deseas borrar este dato", "Yes", "No");
				if (result.Equals(true)){
					using(var dbLocal = new DBLocalContext())
					{
						var query = (from maint in dbLocal.Maintenance where maint.ProjectId == Project.ProjectId select maint).ToList();
						dbLocal.RemoveRange(query);
						dbLocal.SaveChanges();
						dbLocal.Project.Remove(Project);
						dbLocal.SaveChanges();
					}
					await _ViewProject.DisplayAlert("Información", $"Proyecto eliminado", "Ok");
					Navigation.RemovePage(_ViewProject);
				}
			}
			catch (Exception ex)
			{
				await _ViewProject.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
				Navigation.RemovePage(_ViewProject);
			}
		}

		private  Project GetProject(int idProject)
		{
			try
			{
				using(var dblocal = new DBLocalContext())
				{
					var queryResult = (from proj in dblocal.Project
									   where proj.ProjectId == idProject
									   select proj).FirstOrDefault();
					return queryResult;
				}

			}catch (Exception ex)
			{
				return null;
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

		#endregion
	}
}
