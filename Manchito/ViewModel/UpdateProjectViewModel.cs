using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manchito.DataBaseContext;
using Manchito.Model;
using Manchito.Views;
using Microsoft.EntityFrameworkCore;

namespace Manchito.ViewModel
{
    public class UpdateProjectViewModel : INotifyPropertyChangedAbst
    {

		#region Properties
		private Project _Project;

        public Project Project
        {
            get { return _Project; }
            set { _Project = value; }
        }

        private UpdateProject _UpdateProject;
		#endregion

		#region Method
		public UpdateProjectViewModel(  UpdateProject view)
        {
            
            this._UpdateProject = view;
            if(_UpdateProject.ProjectId >= 0)
            {
				Project = GetProject(view.ProjectId).Result;
			}
            else
            {
				view.DisplayAlert("Error", $"El id no es valido, valir {view.ProjectId}", "Ok");
				view.Navigation.RemovePage(view);
			}
        }

        private async Task<Project> GetProject(int id)
        {
            try
            {
                using (var dbLocal= new DBLocalContext())
                {
                    var Project = await dbLocal.Project.Where(P=>P.ProjectId== id).FirstOrDefaultAsync();
                    if (Project == null)
                    {
                        await _UpdateProject.DisplayAlert("Error", "El proyecto no fue encontrado", "OK");
                        return null;
                    }
                    else
                    {
                        return Project;
                    }
                }
            }catch (Exception ex)
            {
                await _UpdateProject.DisplayAlert("Error", $"Error interno {ex.Message}", "OK");
                return null;
            }
        }
        private async Task<bool> UpdateProject()
        {
            try
            {
                return true;
            }catch (Exception ex)
            {
                
                return false;
            }
        }

		#endregion

	}
}
