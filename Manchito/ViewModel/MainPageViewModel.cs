﻿using Manchito.DataBaseContext;
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

        public ICommand LoadProjectsCommand { get; private set; }
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
     
		#endregion

		#region Methods
		public MainPageViewModel()
        {               
            // binding the icommand property with the async method
            ViewProjectCommand = new Command(async (t) =>  ViewProject(t));
			// binding the icommand property with the async method
			AddProjectCommand = new Command(async ()=> await AddProject());    
            LoadProjectsCommand = new Command(async ()=> await LoadProjects());
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
				// load the projects in the db
				using (var db = new DBLocalContext())
				{
					Projects = db.Project.ToList();
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error interno", $"Error: {ex.Message}", "ok");
			}
		}
		[Obsolete]
        private async void ViewProject(object t)
		{
            try
            {
                int id = int.Parse(t.ToString());				
                await Application.Current.MainPage.Navigation.PushAsync(new ViewProject());
				MessagingCenter.Send<MainPageViewModel, int>(this, "Hi", id);
			}
			catch (Exception ex)
            {
				await Application.Current.MainPage.DisplayAlert("Error interno", $"Error: {ex.Message}", "ok");
			}
        }
		#endregion
	}
}
