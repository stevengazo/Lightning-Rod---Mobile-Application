using Android.Provider;
using Android.Webkit;
using AndroidX.Core.Content;
using Manchito.FilesStorageManager;
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
						var dcimFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures));
						await Application.Current.MainPage.DisplayAlert("Info", $"DCMI FOlder {dcimFolder}", "ok");
						// save the file into local storage												
						var data = FileManager.SaveFile("/storage/emulated/0/Pictures", photo);
						if (data)
						{
							await Application.Current.MainPage.DisplayAlert("Info", $"Foto Guardada", "ok");
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
