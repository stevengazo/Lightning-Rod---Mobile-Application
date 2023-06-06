using Android;
using Android.Content.PM;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Manchito.DataBaseContext;
using Manchito.Messages;
using Manchito.Model;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using Plugin.AudioRecorder;
using AndroidX.Core.Content;
using System.IO;
using static Java.Util.Jar.Attributes;
using Microsoft.Maui.Platform;
using static Android.Renderscripts.ScriptGroup;

namespace Manchito.ViewModel
{
	public class ViewCategoryViewModel : INotifyPropertyChangedAbst
	{
		#region properties

		private readonly AudioRecorderService _recorderService = new() { StopRecordingAfterTimeout= false, StopRecordingOnSilence= false};
		private AudioPlayer _audioPlayer = new();
		private Task recordTask;
		private string recording = "";
		private Category _Category;
		private string _Title;
		private List<Photography> _Photos;
		private Color _ColorButtonRecorder;
		public Color ColorButtonRecorder
		{
			get { return _ColorButtonRecorder; }
			set { _ColorButtonRecorder = value;
			if(ColorButtonRecorder != null)
				{
					OnPropertyChanged(nameof(ColorButtonRecorder));
				}
			}
		}
		private string _urlIconRecorder;
		public string urlIconRecorder
		{
			get { return _urlIconRecorder; }
			set
			{
				_urlIconRecorder = value;
				if (urlIconRecorder != null)
				{
					OnPropertyChanged(nameof(urlIconRecorder));
				}
			}
		}

		private Task<string> _recordTask;

		public List<Photography> Photos
		{
			get { return _Photos; }
			set
			{
				_Photos = value;
				if (Photos != null)
				{
					OnPropertyChanged(nameof(Photos));
				}
			}
		}
		public string Title
		{
			get { return _Title; }
			set
			{
				_Title = value;
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
		public ICommand AddItemCommand { get; private set; }
		public ICommand RecordAudioItem { get; private set; }
		public ICommand DeleteAudioCommand { get; private set; }
		public ICommand PlayAudioCommand { get; private set; }
		#endregion
		public ViewCategoryViewModel()
		{
			
			urlIconRecorder = "record.svg";
			ColorButtonRecorder = Colors.Green;
			Title = "";
			TakePhotoCommand = new AsyncRelayCommand(TakePhotoAndroid);
			TakeVideoCommand = new AsyncRelayCommand(TakeVideoAndroid);
			AppearingCommand = new AsyncRelayCommand(LoadCategory);
			ShareItemCommand = new Command((O) => SharePhoto(O));
			DeleteItemCommand = new Command((O) => DeletePhoto(O));
			AddItemCommand = new AsyncRelayCommand( async()=> await AddPhotoFromGalleryAsync());
			RecordAudioItem = new AsyncRelayCommand(async ()=> await RecordAudioAsync());
			DeleteAudioCommand = new AsyncRelayCommand(async (o)=> await DeleteAudioAsync(o));			
		}
		private async Task DeleteAudioAsync(object Object)
		{		
			throw new NotImplementedException();
		}
		private async Task RecordAudioAsync()
		{
			try
			{
				var StatusAudio = await Permissions.CheckStatusAsync<Permissions.Microphone>();
				if (StatusAudio == PermissionStatus.Granted)
				{
					if (_recorderService.IsRecording)
					{
                        ColorButtonRecorder = Colors.Green;
						urlIconRecorder = "record.svg";
						await _recorderService.StopRecording();
						var storagePermission = await Permissions.CheckStatusAsync< Permissions.StorageWrite>();
						if(storagePermission == PermissionStatus.Granted)
						{
							string pathtext = Path.Combine(PathDirectoryFilesAndroid, "Información.txt");
							File.Create(pathtext);
                            string pathAudio = Path.Combine(PathDirectoryFilesAndroid, "sample.wav");
							File.Copy(_recorderService.GetAudioFilePath(),pathAudio);

                        }
						else
						{
						var wirtes=	await Permissions.RequestAsync<Permissions.StorageWrite>();
						var re =	await Permissions.RequestAsync<Permissions.StorageRead>();
						}
					}
					else
					{
						ColorButtonRecorder = Colors.Red;
						urlIconRecorder = "stop.svg";
						_recordTask = await _recorderService.StartRecording();
					}
				}
				else
				{
                    await Permissions.RequestAsync<Permissions.Microphone> ();
                }
			}
			catch (Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error RecordAudioAsync", f.Message, "OK");
			}
		}
		private async Task AddPhotoFromGalleryAsync()
		{
			throw new NotImplementedException();
		}
		private async Task DeletePhoto(object o)
		{
			var Response = Application.Current.MainPage.DisplayAlert("Alerta", "Deseas borrar este dato", "Yes", "No");
		}

	

		private async Task<int> GetLastIdAudio()
		{
			try
			{
				using (DBLocalContext db = new())
				{
					return await (from a in db.AudioNote
							orderby a.AudioNoteId descending
							select a.AudioNoteId).FirstOrDefaultAsync();
				}


			}catch(Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error SharePhoto ", $"Error: {f.Message}", "ok");
				return -1;
			}
		}

		private async Task SharePhoto(object o)
		{
			try
			{
				int number = int.Parse(o.ToString());
				Photography photographytmp = new();
				using (var db = new DBLocalContext())
				{
					photographytmp = db.Photography.Where(P => P.PhotographyId == number).FirstOrDefault();
				}
				if (photographytmp != null)
				{
					await Share.Default.RequestAsync(new ShareFileRequest { Title = "Compartir Imagen", File = new ShareFile(photographytmp.FilePath) });
				}
			}
			catch (Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error SharePhoto ", $"Error: {f.Message}", "ok");
			}

		}
		private async Task TakeVideoAndroid()
		{
			try
			{
				if (MediaPicker.Default.IsCaptureSupported)
				{
					FileResult photo = await MediaPicker.Default.CaptureVideoAsync();
					if (photo != null)
					{
						photo.FileName = $"VID D{DateTime.Today.ToString("yyyy-MM-dd")}_H{DateTime.Now.ToString("HH-mm-ss-fff")}.mp4";
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

					WeakReferenceMessenger.Default.Register<NameItemViewMessage>(this, async (r, m) =>
					{
						using (var db = new DBLocalContext())
						{
							CategoryItem = await db.Category.Where(M => M.CategoryId == m.Value)
														.Include(T => T.ItemType)
														.Include(C => C.Maintenance)
														.Include(C => C.Maintenance.Project)
														.FirstOrDefaultAsync();
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
				List<Photography> photos = new();
				using (DBLocalContext db = new())
				{
					photos = db.Photography.Where(P => P.CategoryId == CategoryItem.CategoryId).OrderByDescending(I => I.CategoryId).ToList();
				}
				Photos = photos;
			}
			catch (Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error LoadImages", f.Message, "OK");
			}
		}

		/// <summary>
		/// return the path of the category in android
		/// </summary>
		/// <returns></returns>
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
			catch (Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error FolderPathAndroid", $"Error: {f.Message}", "ok");
				return string.Empty;
			}
		}
		private async Task<int> GetLastPhotographyId()
		{
			try
			{
				using (var db = new DBLocalContext())
				{
					var num = await (from i in db.Photography
									 orderby i.PhotographyId descending
									 select i.PhotographyId).FirstOrDefaultAsync();
					return num;
				}
			}
			catch (Exception f)
			{
				return -20;
			}

		}
		private async Task<bool> CheckAndroidDirectory()
		{
			try
			{
				if (CategoryItem != null)
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
				}
				else
				{
					return false;
				}

			}
			catch (Exception e)
			{
				await Application.Current.MainPage.DisplayAlert("Error CheckAndroidDirectory", $"Error: {e.Message}", "ok");
				return false;
			}
		}
		private async Task RegisterPhoto(string pathFile)
		{
			try
			{
				Photography photography = new()
				{
					DateTaked = DateTime.Now,
					CategoryId = CategoryItem.CategoryId,
					FilePath = pathFile,
					PhotographyId = (await GetLastPhotographyId() + 1)
				};
				using (DBLocalContext db = new())
				{
					db.Add(photography);
					db.SaveChanges();
				}
			}
			catch (Exception f)
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
