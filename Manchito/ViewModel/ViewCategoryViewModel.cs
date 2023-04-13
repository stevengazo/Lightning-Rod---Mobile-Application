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
using Microsoft.EntityFrameworkCore;
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

		private string _Title;

		public string Title
		{
			get { return _Title; }
			set { _Title = value;
				if (Title != null) { OnPropertyChanged(nameof(Title)); }
			}
		}


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
			Title = "";
			TakePhotoCommand = new AsyncRelayCommand(TakePhotoAndroid);
			AppearingCommand = new Command(()=>LoadCategory());
		}

		/// <summary>
		/// Catch the value and load the category in memory
		/// </summary>
		/// <returns></returns>
		public async Task LoadCategory()
		{
			try
			{
				if (CategoryItem == null)
				{
					WeakReferenceMessenger.Default.Register<NameItemViewMessage>(this, async (r, m) => {
						using (var db = new DBLocalContext())
						{
							CategoryItem = db.Category.Where(M => M.CategoryId == m.Value).Include(T=>T.ItemType).Include(C=>C.Maintenance).Include(C => C.Maintenance.Project).FirstOrDefault();
						}
						if (CategoryItem != null)
						{
							Title = $"{CategoryItem.ItemType.Name} - {CategoryItem.Alias}";
						}
					});
				}
				else
				{
					WeakReferenceMessenger.Default.UnregisterAll(this);
				}
			}
			catch (Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error ViewCategory", f.Message, "OK");
			}
		}
		private async Task<bool> CheckAndroidDirectory()
		{
			try
			{
				if(CategoryItem!=null)
				{
					var Pj = CategoryItem.Maintenance.Project;
					var Man = CategoryItem.Maintenance;
					var Cat = CategoryItem;
					//string categoryPath = Path.Combine(PathDirectoryFilesAndroid, $"{Pj.ProjectId}-{Pj.Name}", $"{Man.MaintenanceId}-{Man.Alias}", );
					return false;
					throw new NotImplementedException();
					
				}
				else
				{
					return false;
				}

			}
			catch(Exception e)
			{
				await Application.Current.MainPage.DisplayAlert("Error CheckAndroidDirectory", $"Error: {e.Message}", "ok");
				return false;
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
