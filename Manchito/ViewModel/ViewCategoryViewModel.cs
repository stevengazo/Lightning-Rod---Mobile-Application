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
using static Android.Content.ClipData;

namespace Manchito.ViewModel
{
	public class ViewCategoryViewModel : INotifyPropertyChangedAbst
	{
		private Category _Category;

		private string _Title;

		private List<Photography> _Photos;

		public List<Photography> Photos
		{
			get { return _Photos; }
			set { _Photos = value; 
			if(Photos != null)
				{
					OnPropertyChanged(nameof(Photos));
				}
			}
		}

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
		public ICommand TakeVideoCommand { get; private set; }
		public ICommand AppearingCommand { get; private set; }
		public ICommand ShareItemCommand { get; private set; }
		public ICommand DeleteItemCommand { get; private set; }


		public ViewCategoryViewModel()
		{
			Title = "";
			TakePhotoCommand = new AsyncRelayCommand(TakePhotoAndroid);
			TakeVideoCommand = new AsyncRelayCommand(TakeVideoAndroid);
			AppearingCommand = new Command(()=>LoadCategory());
			ShareItemCommand = new Command((O) => SharePhoto(O));
			DeleteItemCommand = new Command((O) => DeletePhoto(O));
		}

		private void DeletePhoto(object o)
		{
			var Response = Application.Current.MainPage.DisplayAlert("Alerta", "Deseas borrar este dato", "Yes", "No");
		}

		private void SharePhoto(object o)
		{
			
		}

		private async Task TakeVideoAndroid()
		{
			try
			{
				if (MediaPicker.Default.IsCaptureSupported)
				{
					FileResult photo = await MediaPicker.Default.CaptureVideoAsync();
					photo.FileName = $"VID D{DateTime.Today.ToString("yyyy-MM-dd")}_H{DateTime.Now.ToString("HH-mm-ss-fff")}.mp4";
					if (photo != null)
					{
						// save the file into local storage
						string tempPath = await FolderPathAndroid();
						string localFilePath = Path.Combine(tempPath, photo.FileName);
						using Stream sourceStream = await photo.OpenReadAsync();
						using FileStream localFileStream = File.OpenWrite(localFilePath);
						await sourceStream.CopyToAsync(localFileStream);
						await loadImages();
					}
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error TakePhotoAndroid ", $"Error: {ex.Message}", "ok");
			}
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
							CategoryItem = db.Category.Where(M => M.CategoryId == m.Value)
														.Include(T=>T.ItemType)
														.Include(C=>C.Maintenance)
														.Include(C => C.Maintenance.Project)
														.FirstOrDefault();
						}
						if (CategoryItem != null)
						{
							Title = $"{CategoryItem.ItemType.Name} - {CategoryItem.Alias}";
						}
						await loadImages();
					});
				}
				else
				{
					loadImages();
					WeakReferenceMessenger.Default.UnregisterAll(this);
				}
			}
			catch (Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error ViewCategory", f.Message, "OK");
			}
		}


		private async Task loadImages()
		{
			try
			{
				List<Photography> photos=new();
				using (DBLocalContext db = new())
				{
					photos = db.Photography.Where(P => P.CategoryId == CategoryItem.CategoryId).OrderByDescending(I=>I.CategoryId).ToList();
				}
				Photos = photos;
			}catch(Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error LoadImages", f.Message, "OK");
			}
		}

		private async Task<string> FolderPathAndroid()
		{
			try
			{
				if (CategoryItem != null)
				{
					var Pj = CategoryItem.Maintenance.Project;
					var Man = CategoryItem.Maintenance;
					var Cat = CategoryItem;
					string categoryPath = Path.Combine(
									PathDirectoryFilesAndroid,
								$"P-{Pj.ProjectId}_{Pj.Name}",
									$"M-{Man.MaintenanceId}_{Man.Alias}",
									$"C-{Cat.CategoryId}_{Cat.ItemType.Name}_{Cat.Alias}");
					return categoryPath;
				}
				else
				{
					return string.Empty;
				}
			}
			catch(Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error FolderPathAndroid", $"Error: {f.Message}", "ok");
				return string.Empty;
			}
		}

		private int GetLastPhotographyId()
		{
			try
			{
				using (var db = new DBLocalContext())
				{
					var num = (from i in db.Photography
							   orderby i.PhotographyId descending
							   select i.PhotographyId).FirstOrDefault();
					return num;
				}
			}catch(Exception f)
			{
				return -20;
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
					string categoryPath = await FolderPathAndroid();
					if (Directory.Exists(categoryPath))
					{
						return true;
					}
					else
					{
						Directory.CreateDirectory(categoryPath);
						return true;
					}
					return false;
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
		

		private async Task RegisterPhoto(string pathFile)
		{
			try
			{
				Photography photography = new() { 
					DateTaked= DateTime.Now,
					CategoryId = CategoryItem.CategoryId,
					FilePath = pathFile,
					PhotographyId = (GetLastPhotographyId()+1)
				};
				using (DBLocalContext db = new())
				{
					db.Add(photography);
					db.SaveChanges();
				}
			}catch(Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error RegisterPhoto ", $"Error: {f.Message}", "ok");
			}
		}

		
		private async Task TakePhotoAndroid()
		{
			try
			{
				if (MediaPicker.Default.IsCaptureSupported)
				{
					FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
					photo.FileName = $"IMG D{DateTime.Today.ToString("yyyy-MM-dd")}_H{DateTime.Now.ToString("HH-mm-ss-fff")} .jpg";
					if (photo != null)
					{
						// save the file into local storage
						string tempPath = await FolderPathAndroid();
						string localFilePath = Path.Combine(tempPath, photo.FileName);
						using Stream sourceStream = await photo.OpenReadAsync();
						using FileStream localFileStream = File.OpenWrite(localFilePath);
						await sourceStream.CopyToAsync(localFileStream);
						await RegisterPhoto(localFilePath);
						await loadImages();
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
