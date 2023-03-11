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
		public ViewMaintenanceViewModel(ViewMaintenance VM)
		{
			_ViewMaintenance = VM;
			Maintenance = GetMaintenance();
		}
		public ViewMaintenanceViewModel()
		{

		}

		private Maintenance GetMaintenance()
		{
			try
			{
				Maintenance maintenance = null;
				if (_ViewMaintenance.MaintenanceId > 0)
				{
					using (var dbLocal = new DBLocalContext())
					{
						maintenance = dbLocal.Maintenance.Where(M => M.MaintenanceId == _ViewMaintenance.MaintenanceId).FirstOrDefault();

					}
					return maintenance;
				}
				else
				{
					_ViewMaintenance.DisplayAlert("Error interno", $"El mantenimiento no se encuentra definido", "Ok");
					return maintenance;
				}
			}
			catch (Exception ex)
			{
				_ViewMaintenance.DisplayAlert("Error interno", $"Error. Estos son los detalles del error {ex.Message}", "Ok");
				return null;
			}
		}
	}
}
