using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Manchito.DataBaseContext;
using Manchito.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Windows.Input;

namespace Manchito.ViewModel
{
    public class ValidateDataViewModel : INotifyPropertyChangedAbst
    {
        #region Properties
        private string _Title;

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                if (Title != null)
                {
                    OnPropertyChanged("Title");
                }
            }
        }

        private Maintenance _Maintenance;
        public Maintenance Maintenance
        {
            get { return _Maintenance; }
            set
            {
                _Maintenance = value;
                if (value != null)
                {
                    OnPropertyChanged(nameof(Maintenance));
                }
            }
        }
        private List<Category> _Categories;
        public List<Category> Categories
        {
            get { return _Categories; }
            set
            {
                _Categories = value;
                if (Categories != null)
                {
                    OnPropertyChanged(nameof(Categories));
                }
            }
        }

        #endregion

        #region Icommands 
        public ICommand ShareMaintenanceCommand
        {
            get
            {
                return new Command(() => ShareMaintenance());
            }
            private set { }
        }
        public ICommand AppearingCommand
        {
            get
            {
                return new Command(async () => await LoadManteinance());
            }
            private set { }
        }
        #endregion

        #region Methods
        public ValidateDataViewModel()
        {
            Title = "";
        }
        private async Task LoadCategories()
        {
            try
            {
                using var db = new DBLocalContext();

                Categories = db.Category.Where(C => C.MaintenanceId == Maintenance.MaintenanceId)
                                        .Include(C => C.ItemType)
                                        .Include(M => M.Photographies.Take(3)).ToList();

            }
            catch (Exception f)
            {
                await Application.Current.MainPage.DisplayAlert("Error LoadCategories ", $"Error: {f.Message}", "ok");
            }
        }
        private async Task LoadManteinance()
        {
            try
            {
                var idtmp = TempData.IdMaintenance;

                using (var db = new DBLocalContext())
                {
                    Maintenance = await db.Maintenance.Where(M => M.MaintenanceId == idtmp).SingleOrDefaultAsync();
                    if (Maintenance != null)
                    {
                        Maintenance.Project = await db.Project.FirstOrDefaultAsync(P => P.ProjectId == Maintenance.ProjectId);
                        Maintenance.Project.Maintenances = null;
                        Maintenance.Categories = db.Category.Where(Category => Category.MaintenanceId == idtmp).ToList();
                        foreach (var item in Maintenance.Categories)
                        {
                            item.ItemType = await db.ItemTypes.FirstOrDefaultAsync(I => I.ItemTypeId == item.ItemTypeId);
                            item.ItemType.Categories = null;
                            item.Photographies = await db.Photography.Where(P => P.CategoryId == item.CategoryId).ToListAsync();

                            item.AudioNotes = await db.AudioNote.Where(A => A.CategoryId == item.CategoryId).ToListAsync();
                        }
                        Title = $"Validación {Maintenance.Alias}";
                    }
                }
                if (Maintenance != null)
                {
                    Title = $"Validación {Maintenance.Alias}";
                    await LoadCategories();
                }
            }
            catch (Exception f)
            {
                await Application.Current.MainPage.DisplayAlert("Error LoadMaintenance", $"Error {f.Message}", "OK");
            }
        }

        private async Task MessageToastAsync(string Message, bool IsLong)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var duration = (IsLong) ? ToastDuration.Long : ToastDuration.Short;
            var toast = Toast.Make(Message, duration, 14);
            await toast.Show(cancellationTokenSource.Token);
        }
        private async Task ShareMaintenance()
        {
            try
            {
                string startPath = Path.Combine(
                                    PathDirectoryFilesAndroid,
                                    $"P-{Maintenance.Project.ProjectId}_{Maintenance.Project.Name}",
                                    $"M-{Maintenance.MaintenanceId}_{Maintenance.Alias}");
                if (!Directory.Exists("/storage/emulated/0/Download/Manchito"))
                {
                    Directory.CreateDirectory("/storage/emulated/0/Download/Manchito");
                }
                string zipPathFile = Path.Combine(
                                         "/storage/emulated/0/Download/Manchito",
                                        $"P-{Maintenance.Project.Name}-{Maintenance.Alias}.zip");
                if (System.IO.File.Exists(zipPathFile))
                {
                    // create new zip file
                    await MessageToastAsync("Se sobreescribirá la ultima copia", false);
                    System.IO.File.Delete(zipPathFile);
                }
                var JsonFile = Path.Combine(startPath, "Information About.json");
                var JsonDAta = JsonConvert.SerializeObject(Maintenance, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                using (StreamWriter sw = new StreamWriter(JsonFile))
                {
                    await sw.WriteAsync(JsonDAta);
                }
                await MessageToastAsync("Generando Archivo", false);
                ZipFile.CreateFromDirectory(startPath, zipPathFile, CompressionLevel.SmallestSize, true);

                if (System.IO.File.Exists(zipPathFile))
                {
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = "Compartir Archivo",
                        File = new ShareFile(zipPathFile)
                    });
                }
            }
            catch (Exception f)
            {
                await Application.Current.MainPage.DisplayAlert("Error ShareMaintenance", $"Error interno {f.Message}", "Ok");
            }
        }
        #endregion
    }
}
