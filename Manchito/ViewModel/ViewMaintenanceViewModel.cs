using CommunityToolkit.Mvvm.Messaging;
using Kotlin.Contracts;
using Manchito.DataBaseContext;
using Manchito.Messages;
using Manchito.Model;
using Manchito.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Manchito.ViewModel
{
	public class ViewMaintenanceViewModel : INotifyPropertyChangedAbst
	{
		#region Properties
		private Maintenance _Maintenance;
		private List<Category> _Categories;
		public List<Category> Categories
		{
			get { return _Categories; }
			set { _Categories = value; 
			if(Categories != null)
				{
					OnPropertyChanged(nameof(Categories));
				}
			}
		}
		public Maintenance Maintenance
		{
			get { return _Maintenance; }
			set { _Maintenance = value;
				if (value != null)
				{
					OnPropertyChanged(nameof(Maintenance));
				}
			}
		}
		public ICommand AppearingCommand { get; private set; }
		public ViewMaintenance _ViewMaintenance { get; set; }
		#endregion


		#region Methods
		public ViewMaintenanceViewModel()
		{
			AppearingCommand = new Command(() => LoadManteinance());
		}
		private void LoadManteinance()
		{
			try
			{
				if(Maintenance == null)
				{
					WeakReferenceMessenger.Default.Register<MaintenanceViewMessage>(this, async (r, m) => {
						using (var db = new DBLocalContext())
						{
							Maintenance = db.Maintenance.Where(M => M.MaintenanceId == m.Value).FirstOrDefault();
						}
						if(Maintenance != null)
						{
							loadCategories();
						}
					});
				}
				else
				{
					WeakReferenceMessenger.Default.Unregister<MaintenanceViewMessage>(this);
				}
				}catch(Exception f)
			{
				Application.Current.MainPage.DisplayAlert("error", $"Error {f.Message}", "OK");
			}
		}

		private void loadCategories()
		{
			using(var db = new DBLocalContext())
			{
				Categories = db.Category.Where(C => C.MaintenanceId == Maintenance.MaintenanceId).Include(C=>C.ItemType).ToList();
			}
		}



		#endregion
	}
}
