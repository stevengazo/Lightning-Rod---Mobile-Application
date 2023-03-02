using Manchito.DataBaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manchito.Views;
using System.Windows.Input;

namespace Manchito.ViewModel
{
   public class MainPageViewModel : INotifyPropertyChangedAbst
    {
        public ICommand AddProjectCommand { get; private set; }        


        public INavigation Navigation { get; set; }
        
        public MainPageViewModel(INavigation navigation)
        {
            Navigation= navigation;
            // permite enlazar la propiedad con el meotodo asincrono
            AddProjectCommand= new Command(async ()=> await AddProject());
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
