using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Manchito.DataBaseContext;
using Manchito.Model;
using Manchito.Views;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace Manchito.ViewModel
{
    public class ViewMaintenanceViewModel : INotifyPropertyChangedAbst
    {
        #region Properties
        private Maintenance _Maintenance;
        private List<Category> _Categories;
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
        public ViewMaintenance ViewMaintenance { get; set; }
        #endregion

        #region Icommands
        public ICommand AppearingCommand { get { return new Command(async () => await LoadManteinance()); } private set { } }
        public ICommand ViewCategoryCommand { get { return new Command(async (O) => await ViewCategory(O)); } private set { } }
        public ICommand ValidateDataCommand { get { return new Command(async (o) => await ValidateDataPage(o)); } private set { } }
        public ICommand AddCategoryCommand { get { return new Command(async () => await AddCategory()); } private set { } }
        public ICommand UpdateOnSwapCommand { get; private set; }
        public ICommand UpdateItemOnSwapCommand { get { return new Command(async (o) => await UpdateCategory(o)); ; } private set { } }
        public ICommand DeleteItemOnSwapCommand { get { return new Command(async (o) => await DeleteCategory(o)); ; } private set { } }
        #endregion

        #region Methods
        public ViewMaintenanceViewModel()
        {
            LoadingAnimationVisible = true;
        }
        private async Task MessageToastAsync(string Message, bool IsLong)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var duration = (IsLong) ? ToastDuration.Long : ToastDuration.Short;
            var toast = Toast.Make(Message, duration, 14);
            await toast.Show(cancellationTokenSource.Token);
        }


        private async Task UpdateCategory(object o)
        {
            try
            {
                int CategoryId = int.Parse(o.ToString());
                var ArrayName = await GetItemTypeName();
                string action = await Application.Current.MainPage.DisplayActionSheet("Selecciona el nuevo tipo", "Cancelar", null, ArrayName);
                if (action != null)
                {
                    if (!action.Equals("Cancelar"))
                    {
                        string result = await Application.Current.MainPage.DisplayPromptAsync("Alias", "Digite el nuevo nombre");
                        var ItemType = await GetItemType(action);
                        if (result != null && ItemType != null)
                        {
                            using (var db = new DBLocalContext())
                            {
                                Category category = await db.Category.Where(C => C.CategoryId == CategoryId).Include(C => C.ItemType).FirstOrDefaultAsync();
                                ItemType item = await db.ItemTypes.Where(I => I.Name == action).FirstOrDefaultAsync();
                                if (category != null)
                                {
                                    var OldPath = Path.Combine(PathDirectoryFilesAndroid,
                                    $"P-{Maintenance.Project.ProjectId}_{Maintenance.Project.Name}",
                                    $"M-{Maintenance.MaintenanceId}_{Maintenance.Alias}",
                                    $"C-{category.CategoryId}_{category.ItemType.Name}_{category.Alias}");
                                    if (Directory.Exists(OldPath))
                                    {
                                        category.Alias = result;
                                        category.ItemTypeId = item.ItemTypeId;
                                        category.ItemType = item;
                                        string NewPath = Path.Combine(PathDirectoryFilesAndroid, $"P-{Maintenance.Project.ProjectId}_{Maintenance.Project.Name}", $"M-{Maintenance.MaintenanceId}_{Maintenance.Alias}", $"C-{category.CategoryId}_{action}_{category.Alias}");

                                        Directory.CreateDirectory(NewPath);
                                        var files = Directory.GetFiles(OldPath);
                                        foreach (var item1 in files)
                                        {
                                            await Task.Run(() => File.Move(Path.Combine(OldPath, item1), Path.Combine(NewPath, item1)));
                                        }
                                        db.Category.Update(category);
                                        db.SaveChanges();
                                        if (Directory.GetFiles(OldPath).Length == 0)
                                        {
                                            Directory.Delete(OldPath);
                                        }
                                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                                        var toast = Toast.Make("Categoria Actualizada", ToastDuration.Long, 16);
                                        await toast.Show(cancellationTokenSource.Token);
                                    }
                                }
                            }
                            LoadCategories();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error Update Category", $"Error interno {ex.Message}", "Ok");
                LoadCategories();
            }
        }


        private async Task DeleteCategory(object o)
        {

            try
            {
                int CategoryId = int.Parse(o.ToString());
                var Response = await Application.Current.MainPage.DisplayAlert("Pregunta", "¿Deseas borrar esta categoria", "Sí", "No");
                if (Response.Equals(true))
                {
                    using (var db = new DBLocalContext())
                    {
                        var Category = db.Category.FirstOrDefault(M => M.CategoryId == CategoryId);
                        var itemtype = db.ItemTypes.FirstOrDefaultAsync(M => M.ItemTypeId == Category.ItemTypeId);
                        if (Category != null)
                        {
                            db.Category.Remove(Category);
                            db.SaveChanges();
                            MessageToastAsync("Categoria Eliminada", true);
                            string PathToDelete = Path.Combine(
                                    PathDirectoryFilesAndroid,
                                    $"P-{Maintenance.Project.ProjectId}_{Maintenance.Project.Name}",
                                    $"M-{Maintenance.MaintenanceId}_{Maintenance.Alias}",
                                    $"C-{Category.CategoryId}_{Category.ItemType.Name}_{Category.Alias}");
                            Thread DeleteFiles = new Thread(new ThreadStart(() => { Directory.Delete(PathToDelete, true); }));
                            DeleteFiles.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error DeleteCategory", $"Error interno {ex.Message}", "Ok");
            }
            finally
            {
                LoadCategories();
            }
        }

        /// <summary>
        /// View Category Page
        /// </summary>
        /// <param name="id">id of the item to see</param>
        /// <returns></returns>
        private async Task ViewCategory(Object id)
        {
            try
            {
                TempData.IdCategory = int.Parse(id.ToString());
                await Application.Current.MainPage.Navigation.PushAsync(new ViewCategory(), true);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error ViewCategory", $"Error interno {ex.Message}", "Ok");
            }
        }
        /// <summary>
        /// Open 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private async Task ValidateDataPage(object o)
        {
            //await Application.Current.MainPage.Navigation.PushAsync(new ValidateData());
            try
            {
                TempData.IdMaintenance = int.Parse(o.ToString());
                await Application.Current.MainPage.Navigation.PushAsync(new ValidateData(), true);

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error ValidateDataPage", $"Error interno {ex.Message}", "Ok");
            }
        }
        private static Project GetProject(int id)
        {
            try
            {
                using var db = new DBLocalContext();
                var data = (from i in db.Project
                            where i.ProjectId == id
                            select i).FirstOrDefault();
                return data;
            }
            catch (Exception f)
            {
                Application.Current.MainPage.DisplayAlert("Error ValidateDataPage", $"Error {f.Message}", "OK");
                return null;
            }
        }
        private async Task LoadManteinance()
        {
            try
            {
                if (Maintenance == null)
                {
                    var tempIdMaintenance = TempData.IdMaintenance;
                    using var db = new DBLocalContext();
                    Maintenance = db.Maintenance.Where(M => M.MaintenanceId == tempIdMaintenance).Include(M => M.Project).FirstOrDefault();
                    if (Maintenance != null)
                    {
                        LoadCategories();
                    }
                }
            }
            catch (Exception f)
            {
                await Application.Current.MainPage.DisplayAlert("Error LoadMaintenance", $"Error {f.Message}", "OK");
            }
            finally
            {
                if (Maintenance != null)
                {
                    LoadCategories();
                }
            }
        }
        private async void LoadCategories()
        {
            try
            {
                Thread threadLoadCategories = new Thread(new ThreadStart(() =>
                {

                    if (Maintenance != null)
                    {
                        lock (this)
                        {
                            using var db = new DBLocalContext();
                            Categories = db.Category.Where(C => C.MaintenanceId == Maintenance.MaintenanceId)
                                                    .Include(C => C.ItemType)
                                                    .Include(M => M.Photographies.Take(3)).ToList();
                            LoadingAnimationVisible = false;
                        }
                    }
                }));
                threadLoadCategories.Start();
                threadLoadCategories.Join();
            }
            catch (Exception f)
            {
                await Application.Current.MainPage.DisplayAlert("Error LoadCategories ", $"Error: {f.Message}", "ok");
            }
        }
        private static async Task<int> GetLastCategoryId()
        {
            try
            {
                using var db = new DBLocalContext();
                return (from i in db.Category orderby i.CategoryId descending select i.CategoryId).FirstOrDefault();
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error GetItemTypeName", e.Message, "OK");
                return -1;
            }
        }
        private static async Task<ItemType> GetItemType(int id)
        {
            try
            {
                using var db = new DBLocalContext();
                return (from item in db.ItemTypes
                        where item.ItemTypeId == id
                        select item).FirstOrDefault();
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error GetItemTypeName", e.Message, "OK");
                return null;
            }
        }
        private static async Task<ItemType> GetItemType(string name)
        {
            try
            {
                using var db = new DBLocalContext();
                return (from item in db.ItemTypes
                        where item.Name.Equals(name)
                        select item).FirstOrDefault();
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error GetItemTypeName", e.Message, "OK");
                return null;
            }
        }
        private static async Task<string[]> GetItemTypeName()
        {
            try
            {
                string[] namesOfTypes;
                using (var db = new DBLocalContext())
                {
                    namesOfTypes = (from item in db.ItemTypes select item.Name).ToArray();
                }
                return namesOfTypes;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error GetItemTypeName", e.Message, "OK");
                return null;
            }
        }
        private async Task AddCategory()
        {
            try
            {
                var ArrayName = await GetItemTypeName();
                string action = await Application.Current.MainPage.DisplayActionSheet("¿Qué tipo de item deseas añadir?", "Cancelar", null, ArrayName);
                if (action != null)
                {
                    if (!action.Equals("Cancelar"))
                    {
                        string result = await Application.Current.MainPage.DisplayPromptAsync("Alias", "Digite el nombre del item a agregar");
                        if (result != null)
                        {
                            var ItemType = await GetItemType(action);
                            if (ItemType != null)
                            {
                                using DBLocalContext db = new();
                                Category categoryTmp = new()
                                {
                                    CategoryId = (await GetLastCategoryId() + 1),
                                    Alias = result,
                                    ItemTypeId = ItemType.ItemTypeId,
                                    MaintenanceId = Maintenance.MaintenanceId
                                };
                                // cargar datos
                                db.Category.Add(categoryTmp);
                                db.SaveChanges();

                                // cargar datos de nuevo
                                LoadCategories();
                                var project = GetProject(Maintenance.ProjectId);

                                //Create Directory // Project/Mantenance/Category
                                var DirectoryPathtmp = Path.Combine(
                                    PathDirectoryFilesAndroid,
                                    $"P-{Maintenance.Project.ProjectId}_{Maintenance.Project.Name}",
                                    $"M-{Maintenance.MaintenanceId}_{Maintenance.Alias}",
                                    $"C-{categoryTmp.CategoryId}_{action}_{categoryTmp.Alias}");
                                Directory.CreateDirectory(DirectoryPathtmp);

                                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                                var toast = Toast.Make("Categoria Agregada", ToastDuration.Long, 16);
                                await toast.Show(cancellationTokenSource.Token);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error AddCategory", $"Error {ex.Message}", null);
            }
        }
        #endregion
    }
}
