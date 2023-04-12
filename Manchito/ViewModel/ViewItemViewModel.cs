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
