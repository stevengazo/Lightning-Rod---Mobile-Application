using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Manchito.DataBaseContext;
using Manchito.Messages;
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
            set { _LoadingAnimationVisible = value;
                if (_LoadingAnimationVisible!=null)
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
        public ICommand TakePhotoCommand { get; private set; }
        public ICommand AddPhotoCommand { get; private set; }
        public ICommand TakeVideoCommand { get; private set; }
        public ICommand AppearingCommand { get; private set; }
        public ICommand ShareItemCommand { get; private set; }
        public ICommand DeletePhotoCommand { get; private set; }
        public ICommand AddItemCommand { get; private set; }
        public ICommand RecordAudioItem { get; private set; }
        public ICommand DeleteAudioCommand { get; private set; }
        public ICommand PlayItemCommand { get; private set; }
        #endregion
        public ViewCategoryViewModel()
        {
            LoadingAnimationVisible = true;
            LoadingPhotosVisible = false;

            urlIconRecorder = "record.svg";
            ColorButtonRecorder = Colors.Green;
            Title = "";
            TakePhotoCommand = new AsyncRelayCommand(TakePhotoAndroid);
            TakeVideoCommand = new AsyncRelayCommand(TakeVideoAndroid);
            AddPhotoCommand = new AsyncRelayCommand(AddPhotoFromGalleryAsync);
            AppearingCommand = new AsyncRelayCommand(LoadCategory);
            ShareItemCommand = new Command((O) => SharePhoto(O));
            DeletePhotoCommand = new Command((O) => DeletePhoto(O));
            PlayItemCommand = new Command((O) => PlayAudio(O));
            AddItemCommand = new AsyncRelayCommand(async () => await AddPhotoFromGalleryAsync());
            RecordAudioItem = new AsyncRelayCommand(async () => await RecordAudioAsync());
            DeleteAudioCommand = new Command((O) => DeleteAudioAsync(O));
        }

        private async void PlayAudio(object o)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make("Reproduciendo audio", ToastDuration.Long, 14);
            await toast.Show(cancellationTokenSource.Token);
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
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        var toast = Toast.Make("Audio borrado", ToastDuration.Long, 14);
                        await toast.Show(cancellationTokenSource.Token);
                        await LoadAudio();
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
                if (StatusAudio == PermissionStatus.Granted )
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
                            await LoadAudio();
                            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                            var toast = Toast.Make("Audio Guardado", ToastDuration.Long, 14);
                            await toast.Show(cancellationTokenSource.Token);
                        }
                        else
                        {
                            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                            var toast = Toast.Make("Error al guardar el audio", ToastDuration.Long, 16);
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
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    var toast = Toast.Make("Error en permisos", ToastDuration.Long, 16);
                    await toast.Show(cancellationTokenSource.Token);
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
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var files = await FilePicker.PickMultipleAsync();
                foreach (var item in files)
                {
                    var extension =Path.GetExtension(item.FileName);
                    if(extension == ".jpg" || extension == ".png" || extension == ".jpeg")
                    {
                        item.FileName = $"IMG D{DateTime.Today.ToString("yyyy-MM-dd")}_H{DateTime.Now.ToString("HH-mm-ss-fff")} .jpg";
                        string tempPath = await FolderPathAndroid();
                        string localFilePath = Path.Combine(tempPath, item.FileName);
                        using Stream sourceStream = await item.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);
                        await sourceStream.CopyToAsync(localFileStream);
                        await RegisterPhoto(localFilePath);
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        var toast = Toast.Make("Elementos cargados", ToastDuration.Short, 16);
                        await toast.Show(cancellationTokenSource.Token);
                    }
                    else
                    {
                        CancellationTokenSource cancellationTokenSour = new CancellationTokenSource();
                        var ltoast = Toast.Make($"Elemento no valido: {item.FileName}", ToastDuration.Short, 12);
                        await ltoast.Show(cancellationTokenSour.Token);
                    }
                    await loadImages();
                }
                
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
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        var toast = Toast.Make("Foto borrada", ToastDuration.Long, 14);
                        await toast.Show(cancellationTokenSource.Token);
                        string path = o.ToString();
                        await loadImages();
                    }

                }
            }
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


            }
            catch (Exception f)
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
                WeakReferenceMessenger.Default.UnregisterAll(this);
                await Application.Current.MainPage.DisplayAlert("Error Load Category", f.Message, "OK");
            }
        }
        private async Task loadImages()
        {
            try
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
