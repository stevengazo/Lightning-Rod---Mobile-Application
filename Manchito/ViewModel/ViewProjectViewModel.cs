using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manchito.DataBaseContext;
using Manchito.Model;

namespace Manchito.ViewModel
{
	public class ViewProjectViewModel : INotifyPropertyChangedAbst
	{

		private INavigation Navigation { get; set; }

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


		public ViewProjectViewModel(INavigation nav)
		{
			Navigation = nav;

		}


		private async Task<Project> GetProject(int idProject)
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
