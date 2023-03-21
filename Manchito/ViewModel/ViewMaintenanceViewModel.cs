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
					});
				}
				}catch(Exception f)
			{
				Application.Current.MainPage.DisplayAlert("error", $"Error {f.Message}", "OK");
			}
		}

		[Obsolete]
		private Maintenance GetMaintenance()
		{
			try
			{
				Maintenance maintenance = null;
				var tmpId = 0;

				MessagingCenter.Subscribe<ViewProjectViewModel, int>(this, "MaintenanceId", async (sender, arg) =>
				{
					tmpId = int.Parse(arg.ToString());					
				});				
				if (maintenance !=null)
				{				
					return maintenance;
				}
				else
				{
					Application.Current.MainPage.DisplayAlert("Error interno", $"El mantenimiento no se encuentra definido", "Ok");
					return null;
				}
			}
			catch (Exception ex)
			{
				Application.Current.MainPage.DisplayAlert("Error interno", $"Error. Estos son los detalles del error {ex.Message}", "Ok");
				return null;
			}
		}
		#endregion
	}
}
