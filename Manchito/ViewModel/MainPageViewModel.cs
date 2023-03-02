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
        public ICommand AddProjectCommand { get; private set; }

        private List<Project> _Projects;

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


        public INavigation Navigation { get; set; }
        
        public MainPageViewModel(INavigation navigation)
        {
            Navigation= navigation;
            // binding the icommand property with the async method
            AddProjectCommand= new Command(async ()=> await AddProject());
            // load the projects in the db
            using(var db = new DBLocalContext())
            {
                Projects = db.Project.ToList() ;
            }
        }


        public async Task AddProject()
        {
            try
            {               
				await Navigation.PushAsync(new AddProject());
			}catch(Exception ex)
            {
                
            }
            
        }
    }
}
