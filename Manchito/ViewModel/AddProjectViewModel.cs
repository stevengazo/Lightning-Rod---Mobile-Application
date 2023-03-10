using Manchito.DataBaseContext;
using Manchito.Model;
using Manchito.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Manchito.ViewModel
{
   public class AddProjectViewModel : INotifyPropertyChangedAbst
    {
		#region Properties
		private AddProject _AddProjectView { get; set; }

		private string _Alias;
		public string Alias
		{
			get { return _Alias; }
			set { _Alias = value;
				if(_Alias != null)
				{
					OnPropertyChanged(nameof(Alias));
				}
			}
		}
		private DateTime _DateProject;
		public DateTime DateProject
		{
			get { return _DateProject; }
			set { _DateProject = value;
				if(_DateProject != null)
				{
					OnPropertyChanged(nameof(DateProject));	
				}
			}
		}
		private string _Status;
		public string Status
		{
			get { return _Status; }
			set { _Status = value;
				if(_Status != null)
				{
					OnPropertyChanged(nameof(Status));
				}
			}
		}
		private string _customerName;
		public string CustomerName
		{
			get { return _customerName; }
			set { _customerName = value;
				if(_customerName!= null)
				{
					OnPropertyChanged(nameof(CustomerName));
				}
			}
		}
		private string _CustomerContactName;
		public string CustomerContactName
		{
			get { return _CustomerContactName; }
			set { _CustomerContactName = value; 
				if(CustomerContactName != null)
				{
					OnPropertyChanged(nameof(CustomerContactName));
				}
			}
		}
		public ICommand AddProjectCommand { get; private set; }
		private string _ErrorMessage;
		public string ErrorMessage
		{
			get { return _ErrorMessage; }
			set { _ErrorMessage = value;
				if(_ErrorMessage != null ) {
					OnPropertyChanged(nameof(ErrorMessage));
				}			
			}
		}
		#endregion

		#region Methods
		public AddProjectViewModel(AddProject addProject)
		{
			/// Allow close the view from this class
			_AddProjectView = addProject;

			// Actual date
			DateProject = DateTime.Today;

			// Command to add new projects
			AddProjectCommand = new Command(async () => await AddProject());

		}


		public async Task<int> GetLastIdProject()
		{
			try
			{
				using(var dblocal = new DBLocalContext())
				{
					int queryResult = await (from proj in dblocal.Project
									   orderby proj.ProjectId descending
									   select proj.ProjectId
									).FirstOrDefaultAsync();
					return queryResult;
				}
			}catch(Exception f)
			{
				return -1;
			}
		}

		public async Task AddProject()
		{
			try
			{
				if(Alias== null || Status  == null) {
					ErrorMessage = "No hay datos cargados";
					await _AddProjectView.DisplayAlert("Error", $"{ErrorMessage}", "Ok");
				}
				else
				{
					int projectid = await GetLastIdProject();
					projectid++;
					using(var dbLocal = new DBLocalContext())
					{
						// add the project to the db
						Model.Project tmpProject = new Model.Project() { 
							ProjectId = projectid,
							Name = Alias,
							CustomerName= this._customerName,
							CustomerContactName = this._CustomerContactName,
							Status = this._Status
						};
						dbLocal.Project.Add(tmpProject);
						dbLocal.SaveChanges();
						// create the maintanance
						var lastIdMaintenance = (from maint in dbLocal.Maintenance orderby maint.MaintenanceId descending select maint.MaintenanceId).FirstOrDefault();
						Maintenance maintenancetmp = new Maintenance()
						{
							MaintenanceId = lastIdMaintenance + 1,
							ProjectId = tmpProject.ProjectId,
							Alias = $"Mantenimiento defecto",
							DateOfMaintenance = DateTime.Today,
							Status = "En progreso"
						};
						dbLocal.Maintenance.Add(maintenancetmp);
						dbLocal.SaveChanges();
						await _AddProjectView.DisplayAlert("Información", $"Proyecto Agregado con éxito\nProjecto {tmpProject.ProjectId}\nCliente {tmpProject.Name}", "Ok");
						_AddProjectView.Navigation.RemovePage(_AddProjectView);
					}
				}
			}catch(Exception f)
			{
				ErrorMessage =  $"Error interno: {f.Message}";
				await _AddProjectView.DisplayAlert("Error", $"Error Interno {f.Message}","Ok");
				_AddProjectView.Navigation.RemovePage(_AddProjectView);
			}
		}
		#endregion
	}
}
