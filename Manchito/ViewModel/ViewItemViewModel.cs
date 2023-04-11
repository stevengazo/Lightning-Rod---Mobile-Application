using Android.Provider;
using Android.Webkit;
using AndroidX.Core.Content;
using Manchito.FilesStorageManager;
using Manchito.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Manchito.ViewModel
{
   public class ViewItemViewModel : INotifyPropertyChangedAbst 
    {
        public ICommand TakePhotoCommand { get; private set; }

        public ViewItemViewModel()
        {
            TakePhotoCommand = new Command(() => TakePhoto());
        }

		private async void TakePhotoAndroid()
		{
			try
			{
				if (MediaPicker.Default.IsCaptureSupported)
				{
					FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
					if (photo != null)
					{
						var temporalDirectory= Path.Combine(FileSystem.AppDataDirectory, "Pictures");
						if(Directory.Exists(temporalDirectory))
						{
							Directory.Delete(temporalDirectory, true);						
						}
						Directory.CreateDirectory(temporalDirectory);
						var data = FileManager.SaveFile(temporalDirectory, photo);
						if (data)
						{
							await Application.Current.MainPage.Navigation.PushModalAsync(new ViewPhoto());
						}
					}
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error Interno", $"Error: {ex.Message}", "ok");
			}
		}

		private async void TakePhoto()
		{
			TakePhotoAndroid();
		}
	}
}
