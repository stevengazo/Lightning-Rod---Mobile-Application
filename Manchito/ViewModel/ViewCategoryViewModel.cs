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
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Threading;

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

		private List<AudioNote> _AudioNotes;

		public List<AudioNote> AudioNotes
		{
			get { return _AudioNotes; }
			set { _AudioNotes = value;
				if (AudioNotes != null)
				{
					OnPropertyChanged(nameof(AudioNotes));	
				}
			}
		}


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
		public ICommand PlayItemCommand { get; private set; }
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
            PlayItemCommand = new Command((O) => PlayAudio(O));
            AddItemCommand = new AsyncRelayCommand( async()=> await AddPhotoFromGalleryAsync());
			RecordAudioItem = new AsyncRelayCommand(async ()=> await RecordAudioAsync());
			DeleteAudioCommand = new AsyncRelayCommand(async (o)=> await DeleteAudioAsync(o));			
		}

        private async void PlayAudio(object o)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make("Reproduciendo audio", ToastDuration.Long, 14);
            await toast.Show(cancellationTokenSource.Token);
            string path = o.ToString();
			_audioPlayer.Play(path);
            
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
                var storagePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (StatusAudio == PermissionStatus.Granted && storagePermission == PermissionStatus.Granted)
				{
					if (_recorderService.IsRecording)
					{
                        ColorButtonRecorder = Colors.Green;
						urlIconRecorder = "record.svg";
                        string pathAudio = Path.Combine(PathDirectoryFilesAndroid,$"P-{CategoryItem.Maintenance.Project.ProjectId.ToString()}_{CategoryItem.Maintenance.Project.Name}",$"M-{CategoryItem.Maintenance.MaintenanceId}_{CategoryItem.Maintenance.Alias}",$"C-{CategoryItem.CategoryId}_{CategoryItem.ItemType.Name}_{CategoryItem.Alias}",	$"Audio-{DateTime.Now.ToString("HH_mm_ss")}.wav");                        
                        await _recorderService.StopRecording();
                        string file = _recorderService.GetAudioFilePath();
                        if ( !String.IsNullOrEmpty(file) && _recorderService.TotalAudioTimeout.Seconds >  1 )
						{
							
							File.Copy(file,pathAudio,true);						
							using (var db = new DBLocalContext())
							{
								AudioNote audioNote = new AudioNote();
								audioNote.AudioNoteId = db.AudioNote.OrderByDescending(A=>A.AudioNoteId).Select(A => A.AudioNoteId).FirstOrDefault() + 1;
								audioNote.PathFile = pathAudio;
								audioNote.CategoryId = CategoryItem.CategoryId;
								db.AudioNote.Add(audioNote);
								db.SaveChanges();
							}
							await LoadAudio();
                            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                            var toast = Toast.Make("Audio Guardado",ToastDuration.Long,14);
                            await toast.Show(cancellationTokenSource.Token);
                        }
						else
						{                                                      
                               CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                            var toast = Toast.Make("Error al guardar el audio",ToastDuration.Long,16);
                            await toast.Show(cancellationTokenSource.Token);
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
						await LoadAudio();
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


		private async Task LoadAudio()
		{
			try
			{
                using (DBLocalContext db = new())
                {
                    AudioNotes = (from a in db.AudioNote
                                  where a.CategoryId == CategoryItem.CategoryId
                                  select a).ToList();
                }
            }
			catch(Exception f)
			{
                await Application.Current.MainPage.DisplayAlert("Error LoadAudio", f.Message, "OK");
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

						 CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        var toast = Toast.Make("Foto Guardada", ToastDuration.Long, 14);
                        await toast.Show(cancellationTokenSource.Token);

                    }
				}
			}
			catch (Exception ex)
			{
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var toast = Toast.Make("La foto no fue tomada", ToastDuration.Long, 14);
                await toast.Show(cancellationTokenSource.Token);
            }
		}
		
	}
}
