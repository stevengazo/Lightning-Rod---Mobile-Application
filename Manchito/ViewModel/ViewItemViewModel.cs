using Android.Provider;
using Android.Webkit;
using AndroidX.Core.Content;
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

		private async void TakePhoto()
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
						//string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
						string localFilePath = Path.Combine("/storage/emulated/0/download", photo.FileName);

						using Stream sourceStream = await photo.OpenReadAsync();
						using FileStream localFileStream = File.OpenWrite(localFilePath);

						await sourceStream.CopyToAsync(localFileStream);

						await Application.Current.MainPage.DisplayAlert("Info", $"Foto Guardada en {localFilePath}", "ok");
					}
				}
			}catch(Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error Interno", $"Error: {ex.Message}", "ok");
			}
			
		}
	}
}
