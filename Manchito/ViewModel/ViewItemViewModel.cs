using Android.Provider;
using Android.Webkit;
using AndroidX.Core.Content;
using CommunityToolkit.Mvvm.Input;
using Manchito.FilesStorageManager;
using Manchito.Views;
using Microsoft.EntityFrameworkCore.Update;
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
			TakePhotoCommand = new AsyncRelayCommand(TakePhotoAndroid);
        }

		private async Task OpenViewPhoto()
		{
			try
			{
				var page = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
				
				ViewPhoto vPhoto = new();
				if (page.GetType() == vPhoto.GetType())
				{
					Application.Current.MainPage.Navigation.RemovePage(page);

				}
					await Application.Current.MainPage.Navigation.PushAsync(vPhoto,true);
			}
			catch(Exception ex )
			{
				await Application.Current.MainPage.DisplayAlert("Error OpenViewPhoto ", $"Error: {ex.Message}", "ok");
			}
		}

		private async Task  TakePhotoAndroid()
		{
			try
			{
				var temporalDirectory = Path.Combine(PathDirectoryFilesAndroid, "PicturesTmp");
				if (MediaPicker.Default.IsCaptureSupported)
				{
					FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
					if (photo != null)
					{
						photo.FileName = $"Img-D{DateTime.Today.ToString("yyy-MM-dd")}-T{DateTime.Now.ToString("HH-mm-ss-fff")}.jpg";						
						if(!Directory.Exists(temporalDirectory))
						{
							Directory.CreateDirectory(temporalDirectory);
						}
							
						// save the file into local storage
						string localFilePath = Path.Combine(temporalDirectory , photo.FileName);
						using Stream sourceStream = await photo.OpenReadAsync();
						using FileStream localFileStream = File.OpenWrite(localFilePath);
						sourceStream.CopyToAsync(localFileStream);
						ViewPhoto vPhoto = new();
						Application.Current.MainPage.Navigation.PushAsync(vPhoto, true);
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
