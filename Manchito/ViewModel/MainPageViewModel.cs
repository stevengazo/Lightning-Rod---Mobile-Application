using Manchito.DataBaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manchito.Views;
using System.Windows.Input;
using Manchito.Model;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Manchito.ViewModel
{
   public class MainPageViewModel : INotifyPropertyChangedAbst
    {
		#region Properties
		public ICommand AddProjectCommand { get; private set; }
        public ICommand ViewProjectCommand { get; private set; }
        private List<Project> _Projects;
        private string _ErrorMessage;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value;
                if(_ErrorMessage != null)
                {
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }
        public List<Project> Projects
        {
            get { return _Projects; }
            set { _Projects = value;
                if (_Projects != null)
                {
                    OnPropertyChanged(nameof(Projects));
                }
            }
        }
        private MainPage _MainPage { get; set; }
		#endregion

		#region Methods

		public MainPageViewModel(MainPage mainPage)
        {
            _MainPage= mainPage;
            /// Add event listener and call the function loadproject when the view is loaded
            _MainPage.Appearing += (s, a) => {
                LoadProjects();
            };
            // binding the icommand property with the async method
            ViewProjectCommand = new Command(async (t) =>  ViewProject(t));
			// binding the icommand property with the async method
			AddProjectCommand = new Command(async ()=> await AddProject());            
		}

        private  void  ViewProject(object t)
		{
            try
            {
                ViewProject viewProjecttmp = new ViewProject( t);
                _MainPage.Navigation.PushAsync(viewProjecttmp);
			}
			catch (Exception ex)
            {
				_MainPage.DisplayAlert("Error interno", $"Error: {ex.Message}", "ok");
			}
        }

        public async Task LoadProjects()
        {
            try
            {
				// load the projects in the db
				using (var db = new DBLocalContext())
				{
					Projects = db.Project.ToList();
				}
			}
			catch(Exception ex) {
                await _MainPage.DisplayAlert("Error interno", $"Error: {ex.Message}", "ok");   
            }
        }

        public async Task AddProject()
        {
            try
            {
                await _MainPage.Navigation.PushAsync(new AddProject());
                
			}catch(Exception ex)
            {
                ErrorMessage = $"Error {ex.Message}";
            }
            
        }
		#endregion
	}
}
