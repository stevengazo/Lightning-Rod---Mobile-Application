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
        public ICommand AddProjectCommand { get; }        


        public INavigation Navigation { get; set; }
        
        public MainPageViewModel(INavigation navigation)
        {
            Navigation= navigation;
        }


        public async Task AddProject()
        {
            await Navigation.PushAsync(new AddProject());
        }
    }
}
