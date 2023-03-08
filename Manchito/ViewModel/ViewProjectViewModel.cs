using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manchito.DataBaseContext;
using Manchito.Model;
using Manchito.Views;

namespace Manchito.ViewModel
{
	public class ViewProjectViewModel : INotifyPropertyChangedAbst
	{

		public int ProjectIdEXternal { get; set; }
		private INavigation Navigation { get; set; }
		private ViewProject ViewProject { get; set; }

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


		public ViewProjectViewModel(INavigation nav, ViewProject _viewProject)
		{
			Navigation = nav;
			ViewProject = _viewProject;

			// refresh the data and the project
			ViewProject.Appearing += (s, a) =>
			{
				// Logic
				Project = GetProject(ProjectIdEXternal);

			};
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
	}
}
