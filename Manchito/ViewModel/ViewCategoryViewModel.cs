using Android.Provider;
using Android.Webkit;
using AndroidX.Core.Content;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Manchito.DataBaseContext;
using Manchito.FilesStorageManager;
using Manchito.Messages;
using Manchito.Model;
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
	public class ViewCategoryViewModel : INotifyPropertyChangedAbst
	{
		private Category _Category;

		public Category CategoryItem
		{
			get { return _Category; }
			set
			{
				_Category = value;
				if (CategoryItem != null)
				{
					OnPropertyChanged(nameof(CategoryItem));
				}
			}
		}

		public ICommand TakePhotoCommand { get; private set; }
		public ICommand AppearingCommand { get; private set; }

		public ViewCategoryViewModel()
		{
			TakePhotoCommand = new AsyncRelayCommand(TakePhotoAndroid);
			TakePhotoCommand = new AsyncRelayCommand(LoadCategory);
		}


		public async Task LoadCategory()
		{
			try
			{
				if (CategoryItem != null)
				{
					WeakReferenceMessenger.Default.Register<NameItemViewMessage>(this, async (r, m) => {
						using (var db = new DBLocalContext())
						{
							CategoryItem = db.Category.Where(M => M.CategoryId == m.Value).FirstOrDefault();
						}
						if (CategoryItem != null)
						{
							// Load Category
						}
					});

				}
			}
			catch (Exception f)
			{
				Application.Current.MainPage.DisplayAlert("Error ViewCategory", f.Message, "OK");
			}
		}

		private async Task TakePhotoAndroid()
		{
			try
			{
				if (MediaPicker.Default.IsCaptureSupported)
				{
					FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
					if (photo != null)
					{
						// save the file into local storage
						string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
						using Stream sourceStream = await photo.OpenReadAsync();
						using FileStream localFileStream = File.OpenWrite(localFilePath);
						await sourceStream.CopyToAsync(localFileStream);
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
