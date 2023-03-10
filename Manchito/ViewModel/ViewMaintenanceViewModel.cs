using Manchito.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.ViewModel
{    
    public class ViewMaintenanceViewModel : INotifyPropertyChangedAbst
    {
        public ViewMaintenance  _ViewMaintenance { get; set; }
        public ViewMaintenanceViewModel(ViewMaintenance VM)
        {
            _ViewMaintenance= VM;
        }
        public ViewMaintenanceViewModel()
        {

        }
    }
}
