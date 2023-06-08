using Manchito.DataBaseContext;
using Manchito.Model;
using Manchito.Views;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace Manchito.ViewModel
{
    public class UpdateProjectViewModel : INotifyPropertyChangedAbst
    {

        #region Properties
        private Project _Project;
        private UpdateProject _UpdateProject;

        public ICommand UpdateProjectCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public Project Project
        {
            get { return _Project; }
            set { _Project = value; }
        }


        #endregion

        #region Method
        public UpdateProjectViewModel(UpdateProject view)
        {

            this._UpdateProject = view;
            // Commands
            UpdateProjectCommand = new Command(async (o) => await UpdateProject(o));
            CancelCommand = new Command(Cancel);
            // validation of the logic
            if (_UpdateProject.ProjectId >= 0)
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
                using (var dbLocal = new DBLocalContext())
                {
                    var Project = await dbLocal.Project.Where(P => P.ProjectId == id).FirstOrDefaultAsync();
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
            }
            catch (Exception ex)
            {
                await _UpdateProject.DisplayAlert("Error", $"Error interno {ex.Message}", "OK");
                return null;
            }
        }
        private async Task UpdateProject(object o)
        {
            int projectId = (int)o;
            try
            {
                using (var db = new DBLocalContext())
                {

                    db.Project.Update(Project);
                    db.SaveChanges();
                    await _UpdateProject.DisplayAlert("Información", "Información del proyecto actualizada", "Ok");
                    _UpdateProject.Navigation.RemovePage(_UpdateProject);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async void Cancel()
        {
            var lastPage = Application.Current.MainPage.Navigation.NavigationStack.ToList().LastOrDefault();
            if (lastPage is UpdateProject)
            {
                Application.Current.MainPage.Navigation.RemovePage(lastPage);
            }


        }

        #endregion

    }
}
