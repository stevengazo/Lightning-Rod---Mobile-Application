using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Manchito.DataBaseContext;
using Manchito.Messages;
using System.Threading;
using Microsoft.Maui.Graphics.Platform;
using Manchito.Model;
using Microsoft.EntityFrameworkCore;
using Plugin.AudioRecorder;
using System.Windows.Input;


namespace Manchito.ViewModel
{
    public class ViewCategoryViewModel : INotifyPropertyChangedAbst
    {
        #region properties

        private readonly AudioRecorderService _recorderService = new() { StopRecordingAfterTimeout = false, StopRecordingOnSilence = false };
        private AudioPlayer _audioPlayer = new();
        private Task recordTask;
        private string recording = "";
        private Category _Category;
        private string _Title;
        private List<Photography> _Photos;
        private bool _LoadingPhotosVisible;
        public bool LoadingPhotosVisible
        {
            get { return _LoadingPhotosVisible; }
            set
            {
                _LoadingPhotosVisible = value;
                if (_LoadingPhotosVisible != null)
                {
                    OnPropertyChanged(nameof(LoadingPhotosVisible));
                }
            }
        }


        private bool _LoadingAnimationVisible;

        public bool LoadingAnimationVisible
        {
            get { return _LoadingAnimationVisible; }
            set
            {
                _LoadingAnimationVisible = value;
                if (_LoadingAnimationVisible != null)
                {
                    OnPropertyChanged(nameof(LoadingAnimationVisible));
                }
            }
        }



        private List<AudioNote> _AudioNotes;

        public List<AudioNote> AudioNotes
        {
            get { return _AudioNotes; }
            set
            {
                _AudioNotes = value;
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
            set
            {
                _ColorButtonRecorder = value;
                if (ColorButtonRecorder != null)
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

        #endregion

        #region Icommands
        public ICommand TakePhotoCommand
        {
            get
            {
                return new AsyncRelayCommand(TakePhotoAndroid);
            }
            private set { }
        }
        public ICommand AddPhotoCommand { get { return new AsyncRelayCommand(AddPhotoFromGalleryAsync); } private set { } }
        public ICommand TakeVideoCommand
        {
            get { return new AsyncRelayCommand(TakeVideoAndroid); }
            private set { }
        }
        public ICommand AppearingCommand
        {
            get
            {
                return new AsyncRelayCommand(LoadCategory);
            }
            private set { }
        }
        public ICommand ShareItemCommand
        {
            get
            {
                return
                    new Command((O) => SharePhoto(O));
            }
            private set { }
        }
        public ICommand DeletePhotoCommand
        {
            get
            {
                return
                    new Command((O) => DeletePhoto(O));
            }
            private set { }
        }
        public ICommand AddItemCommand
        {
            get
            {
                return new
                    AsyncRelayCommand(async () => await AddPhotoFromGalleryAsync());
            }
            private set { }
        }
        public ICommand RecordAudioItem
        {
            get
            {
                return new
                    AsyncRelayCommand(async () => await RecordAudioAsync());
            }
            private set { }
        }
        public ICommand DeleteAudioCommand
        {
            get
            {
                return
                    new Command((O) => DeleteAudioAsync(O));
            }
            private set { }
        }
        public ICommand PlayItemCommand
        {
            get
            {
                return
                    new Command((O) => PlayAudio(O));
            }
            private set { }
        }
        #endregion

        #region Methods
        public ViewCategoryViewModel()
        {
            LoadingAnimationVisible = true;
            LoadingPhotosVisible = false;
            urlIconRecorder = "record.svg";
            Title = "";
            ColorButtonRecorder = Colors.Green;
        }
        private async void PlayAudio(object o)
        {
            await MessageToastAsync("Reproduciendo audio", false);
            _audioPlayer.Play(o.ToString());
        }
        private async Task DeleteAudioAsync(object Object)
        {
            int id = int.Parse(Object.ToString());
            var Response = await Application.Current.MainPage.DisplayAlert("Pregunta", "Deseas borrar este Audio?", "Sí", "No");
            if (Response.Equals(true))
            {
                using (var db = new DBLocalContext())
                {
                    var audio = db.AudioNote.Where(P => P.AudioNoteId == id).FirstOrDefault();
                    if (audio != null)
                    {
                        db.AudioNote.Remove(audio);
                        db.SaveChanges();
                        File.Delete(audio.PathFile);
                        await MessageToastAsync("Audio Borrado", true);
                        LoadAudio();
                    }
                }
            }
        }
        private async Task RecordAudioAsync()
        {
            try
            {
                var StatusAudio = await Permissions.CheckStatusAsync<Permissions.Microphone>();
                if (OperatingSystem.IsAndroidVersionAtLeast(30))
                {
                    var storagePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                }
                if (StatusAudio == PermissionStatus.Granted)
                {
                    if (_recorderService.IsRecording)
                    {
                        ColorButtonRecorder = Colors.Green;
                        urlIconRecorder = "record.svg";
                        string pathAudio = Path.Combine(PathDirectoryFilesAndroid, $"P-{CategoryItem.Maintenance.Project.ProjectId.ToString()}_{CategoryItem.Maintenance.Project.Name}", $"M-{CategoryItem.Maintenance.MaintenanceId}_{CategoryItem.Maintenance.Alias}", $"C-{CategoryItem.CategoryId}_{CategoryItem.ItemType.Name}_{CategoryItem.Alias}", $"Audio-{DateTime.Now.ToString("HH_mm_ss")}.wav");
                        await _recorderService.StopRecording();
                        string file = _recorderService.GetAudioFilePath();
                        if (!String.IsNullOrEmpty(file) && _recorderService.TotalAudioTimeout.Seconds > 1)
                        {
                            File.Copy(file, pathAudio, true);
                            using (var db = new DBLocalContext())
                            {
                                AudioNote audioNote = new AudioNote();
                                audioNote.AudioNoteId = db.AudioNote.OrderByDescending(A => A.AudioNoteId).Select(A => A.AudioNoteId).FirstOrDefault() + 1;
                                audioNote.PathFile = pathAudio;
                                audioNote.CategoryId = CategoryItem.CategoryId;
                                db.AudioNote.Add(audioNote);
                                db.SaveChanges();
                            }
                            LoadAudio();
                            await MessageToastAsync("Audio Guardado", true);
                        }
                        else
                        {
                            await MessageToastAsync("Error al guardar el audio", true);
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
                    await MessageToastAsync("Error en Permisos", false);
                    await Permissions.RequestAsync<Permissions.Microphone>();
                }
            }
            catch (Exception f)
            {
                await Application.Current.MainPage.DisplayAlert("Error RecordAudioAsync", f.Message, "OK");
            }
        }
        private async Task AddPhotoFromGalleryAsync()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var files = await FilePicker.PickMultipleAsync();
                    foreach (var item in files)
                    {
                        var extension = Path.GetExtension(item.FileName);
                        if (extension == ".jpg" || extension == ".png" || extension == ".jpeg")
                        {
                            item.FileName = $"IMG D{DateTime.Today.ToString("yyyy-MM-dd")}_H{DateTime.Now.ToString("HH-mm-ss-fff")}.jpeg";
                            string localFilePath = Path.Combine(await FolderPathAndroid(), item.FileName);
                            File.Copy(item.FullPath, localFilePath);
                            await RegisterPhoto(localFilePath);
                            await MessageToastAsync("Elementos Cargados", false);
                        }
                        else
                        {
                            await MessageToastAsync($"Elemento no valido: {item.FileName}", false);
                        }
                    }
                }
            }
            catch (Exception f)
            {
                await MessageToastAsync($"No seleccionó un elemento", true);
            }
            finally
            {
                loadImages();
            }
        }
        private async Task DeletePhoto(object o)
        {
            int id = int.Parse(o.ToString());
            var Response = await Application.Current.MainPage.DisplayAlert("Alerta", "Deseas borrar este dato", "Yes", "No");
            if (Response.Equals(true))
            {
                using (var db = new DBLocalContext())
                {
                    var photo = db.Photography.Where(P => P.PhotographyId == id).FirstOrDefault();
                    if (photo != null)
                    {
                        db.Photography.Remove(photo);
                        db.SaveChanges();
                        File.Delete(photo.FilePath);
                        await MessageToastAsync("Foto Borrada", false);
                        string path = o.ToString();
                        loadImages();
                    }
                }
            }
        }
        private async Task ShareAudio(object o)
        {
            try
            {
                int number = int.Parse(o.ToString());
                AudioNote audio = new();
                using (var db = new DBLocalContext())
                {
                    audio = db.AudioNote.Where(P => P.AudioNoteId == number).FirstOrDefault();
                }
                if (audio != null)
                {
                    await Share.Default.RequestAsync(new ShareFileRequest { Title = "Compartir Nota Voz", File = new ShareFile(audio.PathFile) });
                }
            }
            catch (Exception f)
            {
                await MessageToastAsync($"Error: {f.Message}", true);
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
                    FileResult video = await MediaPicker.Default.CaptureVideoAsync();
                    if (video != null)
                    {
                        video.FileName = $"VID D{DateTime.Today.ToString("yyyy-MM-dd")}_H{DateTime.Now.ToString("HH-mm-ss-fff")}.mp4";
                        // save the file into local storage                       
                        string localFilePath = Path.Combine(await FolderPathAndroid(), video.FileName);
                        File.Move(video.FullPath, localFilePath);
                        loadImages();
                        await MessageToastAsync("Video Guardado", true);
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
                int IdTemp = TempData.IdCategory;
                if (IdTemp != 0)
                {
                    using var db = new DBLocalContext();

                    CategoryItem = await db.Category.Where(M => M.CategoryId == IdTemp)
                                                .Include(T => T.ItemType)
                                                .Include(C => C.Maintenance)
                                                .Include(C => C.Maintenance.Project)
                                                .FirstOrDefaultAsync();
                    if (CategoryItem != null)
                    {
                        Title = $"{CategoryItem.ItemType.Name} - {CategoryItem.Alias}";
                        Thread threadImages = new Thread(new ThreadStart(loadImages));
                        threadImages.Start();
                        Thread threadAudio = new Thread(new ThreadStart(LoadAudio));
                        threadAudio.Start();
                    }
                }
            }
            catch (Exception f)
            {
                await Application.Current.MainPage.DisplayAlert("Error Load Category", f.Message, "OK");
            }
        }
        private void loadImages()
        {
            try
            {
                Thread threadLoadImages = new Thread(new ThreadStart(() =>
                {
                    if (CategoryItem != null)
                    {
                        List<Photography> photos = new();
                        using (DBLocalContext db = new())
                        {
                            photos = db.Photography.Where(P => P.CategoryId == CategoryItem.CategoryId).OrderByDescending(I => I.CategoryId).ToList();
                        }
                        Photos = photos;
                        LoadingAnimationVisible = false;
                        LoadingPhotosVisible = true;
                    }
                }));
                threadLoadImages.Start();
            }
            catch (Exception f)
            {
                Application.Current.MainPage.DisplayAlert("Error LoadImages", f.Message, "OK");
            }
        }
        private async void LoadAudio()
        {
            try
            {
                if (CategoryItem != null)
                {
                    using (DBLocalContext db = new())
                    {
                        AudioNotes = (from a in db.AudioNote
                                      where a.CategoryId == CategoryItem.CategoryId
                                      select a).ToList();
                    }
                }
            }
            catch (Exception f)
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
        private async Task RegisterPhoto(string pathFile)
        {
            try
            {
                Photography photography = new()
                {
                    DateTaked = DateTime.Now,
                    CategoryId = CategoryItem.CategoryId,
                    Name = Path.GetFileName(pathFile),
                    FilePath = pathFile,
                    PhotographyId = (await GetLastPhotographyId() + 1)
                };
                photography.DateTaked = DateTime.Now;
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
                        string tempPath = await FolderPathAndroid();
                        string localFilePath = Path.Combine(tempPath, photo.FileName);
                        File.Move(photo.FullPath, localFilePath);
                        await RegisterPhoto(localFilePath);
                        loadImages();
                        await MessageToastAsync("Foto Guardada", true);
                    }
                }
            }
            catch (Exception ex)
            {
                await MessageToastAsync("La Foto no fue tomada", true);
            }
        }
        private async Task MessageToastAsync(string Message, bool IsLong)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var duration = (IsLong) ? ToastDuration.Long : ToastDuration.Short;
            var toast = Toast.Make(Message, duration, 14);
            await toast.Show(cancellationTokenSource.Token);
        }

        #endregion
    }
}
