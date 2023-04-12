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
			try
			{
				TakePhotoCommand = new Command(() => { TakePhotoAndroid(); }) ;
			}catch(Exception ex)
			{
				Application.Current.MainPage.DisplayAlert("Error TakePhotoAndroid ", $"Error: {ex.Message}", "ok");
			}
            
        }

		private static async void TakePhotoAndroid()
		{
			try
			{
				bool SaveIt = false;
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
						SaveIt = FileManager.SaveFile(temporalDirectory, photo);						
					}
					if (SaveIt) {
						
					}
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error TakePhotoAndroid ", $"Error: {ex.Message}", "ok");
			}
		}
	}
}
