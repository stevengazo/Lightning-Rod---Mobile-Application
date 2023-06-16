using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using Manchito.DataBaseContext;
using Manchito.Messages;
using Manchito.Model;
using Manchito.Views;
using System.Windows.Input;

namespace Manchito.ViewModel
{
    public class ViewProjectViewModel : INotifyPropertyChangedAbst
    {
        #region Properties
        public ICommand AddMaintenanceCommand { get; private set; }
        public ICommand ViewMaintenanceCommand { get; private set; }
        public ICommand DeleteMaintenanceCommand { get; private set; }
        public ICommand DeleteProjectCommand { get; private set; }
        public ICommand UpdateProjectCommand { get; private set; }

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


        public ICommand AppearingCommand { get; private set; }
        public int ProjectIdEXternal { get; set; }
        private List<Maintenance> _Maintenances;

        public List<Maintenance> Maintenances
        {
            get { return _Maintenances; }
            set
            {
                _Maintenances = value;
                if (Maintenances != null)
                {
                    OnPropertyChanged(nameof(Maintenances));
                }
            }
        }
        private Project _Project;
        public Project Project
        {
            get { return _Project; }
            set
            {
                _Project = value;
                if (Project != null)
                {
                    OnPropertyChanged(nameof(Project));
                }
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Constructor of the class ViewProjectViewModel
        /// </summary>
        /// <param name="nav">Navigation</param>
        /// <param name="_viewProject">View how implement the view</param>
        public ViewProjectViewModel()
        {
            LoadingAnimationVisible = true;
            // binding commands to the Property
            AddMaintenanceCommand = new Command(async () => { await AddMaintenanceAsync(); });
            DeleteProjectCommand = new Command(async () => { await DeleteProjectAsync(); });
            ViewMaintenanceCommand = new Command(async (t) => { await ViewMaintenanceAsync(t); });
            UpdateProjectCommand = new Command(async () => await UpdateProjectAsync());
            AppearingCommand = new Command(async () => { await LoadProjectCommandAsync(); });
            DeleteMaintenanceCommand = new Command(async (t) => { await DeleteMaintenanceAsync(t); });
        }


        private async Task MessageToastAsync(string Message, bool IsLong)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var duration = (IsLong) ? ToastDuration.Long : ToastDuration.Short;
            var toast = Toast.Make(Message, duration, 14);
            await toast.Show(cancellationTokenSource.Token);
        }


        private async Task DeleteMaintenanceAsync(object t)
        {
            try
            {
                int MaintenanceId = int.Parse(t.ToString());
                var Response = await Application.Current.MainPage.DisplayAlert("Pregunta", "¿Deseas borrar este mantenimiento?", "Sí", "No");
                if (Response.Equals(true))
                {
                    using (var db = new DBLocalContext())
                    {
                        var maintenance = db.Maintenance.FirstOrDefault(M => M.MaintenanceId == MaintenanceId);
                        if (maintenance != null)
                        {
                            db.Remove(maintenance);
                            db.SaveChanges();
                            await MessageToastAsync("Mantenimiento Eliminado", false);
                        }
                    }
                }
            }catch  (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
            }
            finally
            {
                await LoadMaintenancesAsync();
            }

        }

        private async Task AddMaintenanceAsync()
        {
            try
            {
                Model.Maintenance tmp = new Maintenance()
                {
                    ProjectId = Project.ProjectId,
                    MaintenanceId = GetLastMaintenanceId() + 1,
                    DateOfMaintenance = DateTime.Now
                };
                tmp.Alias = await Application.Current.MainPage.DisplayPromptAsync("Ingreso mantenimiento", "¿Deseas añadir un mantenimiento?", "Agregar", "Cancelar", null, 50);
                tmp.Status = await Application.Current.MainPage.DisplayActionSheet("Estatus del mantenimiento", "Cancelar", null, "En ejecución", "Concluido", "Pendiente ejecución");
                if (!string.IsNullOrEmpty(tmp.Alias) && !string.IsNullOrEmpty(tmp.Status) && !tmp.Status.Equals("Cancelar"))
                {
                    using (var dbLocal = new DBLocalContext())
                    {
                        dbLocal.Add(tmp);
                        dbLocal.SaveChanges();
                        await LoadMaintenancesAsync();
                        await MessageToastAsync("Mantenimiento Agregado Al Proyecto", false);
                        //Create Directory // Project/Mantenance
                        var DirectoryPath = Path.Combine(PathDirectoryFilesAndroid, $"P-{Project.ProjectId}_{Project.Name}", $"M-{tmp.MaintenanceId}_{tmp.Alias}");
                        Directory.CreateDirectory(DirectoryPath);
                        // base items of the maintenance
                        List<Category> itemsCategories = new List<Category>();
                        using (var local = new DBLocalContext())
                        {
                            var itemstypes = local.ItemTypes.ToList();
                            foreach (var item in itemstypes)
                            {
                                Category Cat = new Category()
                                {
                                    CategoryId = await lastCategoryIdAsync() + 1,
                                    Alias = "No Asignado",
                                    ItemTypeId = item.ItemTypeId,
                                    MaintenanceId = tmp.MaintenanceId
                                };
                                dbLocal.Add(Cat);
                                dbLocal.SaveChanges();
                                //Create Directory // Project/Mantenance/Category
                                var DirectoryPathtmp = Path.Combine(
                                    PathDirectoryFilesAndroid,
                                    $"P-{Project.ProjectId}_{Project.Name}",
                                    $"M-{tmp.MaintenanceId}_{tmp.Alias}",
                                    $"C-{Cat.CategoryId}_{item.Name}_{Cat.Alias}");
                                Directory.CreateDirectory(DirectoryPathtmp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
                await ClosePageAsync();
            }
        }
        private async Task ClosePageAsync()
        {
            var page = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
            Application.Current.MainPage.Navigation.RemovePage(page);
        }
        private async Task DeleteProjectAsync()
        {
            try
            {
                var result = await Application.Current.MainPage.DisplayAlert("Alerta", "Deseas borrar este dato", "Yes", "No");
                if (result.Equals(true))
                {
                    using (var dbLocal = new DBLocalContext())
                    {
                        var query = (from maint in dbLocal.Maintenance where maint.ProjectId == Project.ProjectId select maint).ToList();
                        dbLocal.RemoveRange(query);
                        dbLocal.SaveChanges();
                        dbLocal.Project.Remove(Project);
                        dbLocal.SaveChanges();
                    }
                    await Application.Current.MainPage.DisplayAlert("Información", $"Proyecto eliminado", "Ok");
                    await ClosePageAsync();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
                await ClosePageAsync();
            }
        }
        private int GetLastMaintenanceId()
        {
            using (var dbLocal = new DBLocalContext())
            {
                int id = (from main in dbLocal.Maintenance
                          orderby main.MaintenanceId descending
                          select main.MaintenanceId).FirstOrDefault();
                return id;
            }
        }
        private async Task<int> lastCategoryIdAsync()
        {
            using (var db = new DBLocalContext())
            {
                int id = (from iT in db.Category
                          orderby iT.CategoryId descending
                          select iT.CategoryId).FirstOrDefault();
                return id;
            }
        }
        private async Task LoadProjectCommandAsync()
        {
            try
            {
                if (Project == null)
                {
                    int idProject = TempData.IdProject;
                    using var db = new DBLocalContext();
                    Project = db.Project.Where(P => P.ProjectId == idProject).FirstOrDefault();
                    if (Project != null)
                    {
                        await Task.Run(LoadMaintenancesAsync);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("error", $"Eror {ex.Message}", "OK");
            }
        }
        private async Task LoadMaintenancesAsync()
        {
            try
            {
                using (var dbLocal = new DBLocalContext())
                {
                    Maintenances = dbLocal.Maintenance.Where(M => M.ProjectId == Project.ProjectId).ToList();
                    LoadingAnimationVisible = false;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
            }
        }
        private async Task UpdateProjectAsync()
        {
            try
            {
                UpdateProject UpdateView = new UpdateProject(Project.ProjectId);
                await Application.Current.MainPage.Navigation.PushAsync(UpdateView, true);
            }
            catch (Exception f)
            {
                await Application.Current.MainPage.DisplayAlert("Error interno", $"Error interno, intentelo mas tarde. {f.Message}", "OK");
            }

        }
        private async Task ViewMaintenanceAsync(object idNumber)
        {
            try
            {
                TempData.IdMaintenance = int.Parse(idNumber.ToString());
                ViewMaintenance vMaintPage = new ViewMaintenance();
                await Application.Current.MainPage.Navigation.PushAsync(vMaintPage,false);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error interno {ex.Message}", "Ok");
                await ClosePageAsync();
            }
        }
        #endregion
    }
}
