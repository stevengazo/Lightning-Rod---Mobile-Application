using Kotlin.Contracts;
using Manchito.DataBaseContext;
using Manchito.Model;
using Manchito.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public ViewMaintenance _ViewMaintenance { get; set; }
		#endregion
		#region Methods
		public ViewMaintenanceViewModel()
		{			
			Maintenance = GetMaintenance();
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
