using Android.Widget;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Kotlin.Contracts;
using Manchito.DataBaseContext;
using Manchito.Messages;
using Manchito.Model;
using Manchito.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Android.Content.ClipData;
using static Android.Graphics.Paint;

namespace Manchito.ViewModel
{
	public class ViewMaintenanceViewModel : INotifyPropertyChangedAbst
	{
		#region Properties
		private Maintenance _Maintenance;
		private List<Category> _Categories;
		public List<Category> Categories
		{
			get { return _Categories; }
			set { _Categories = value; 
			if(Categories != null)
				{
					OnPropertyChanged(nameof(Categories));
				}
			}
		}
		public Maintenance Maintenance
		{
			get { return _Maintenance; }
			set { _Maintenance = value;
				if (value != null)
				{
					OnPropertyChanged(nameof(Maintenance));
				}
			}
		}
		public ICommand AppearingCommand { get; private set; }
		public ICommand ViewCategoryCommand { get; private set; }
		public ICommand ValidateDataCommand { get; private set; }
		public ICommand AddCategoryCommand { get; private set; }
		public ViewMaintenance ViewMaintenance { get; set; }
		#endregion

		#region Methods
		public ViewMaintenanceViewModel()
		{
			AppearingCommand = new Command(() => LoadManteinance());
			ValidateDataCommand = new Command(() => ValidateDataPage());
			AddCategoryCommand = new Command(() => AddCategory());
			ViewCategoryCommand = new Command( (O) =>  ViewCategory(O));
		}

		private async void ViewCategory(Object id)
		{
			try
			{
				int number = int.Parse(id.ToString());				
				await Application.Current.MainPage.Navigation.PushAsync(new ViewCategory(), true);
				WeakReferenceMessenger.Default.Cleanup();
				WeakReferenceMessenger.Default.Send(new NameItemViewMessage(number));
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error ViewCategory", $"Error interno {ex.Message}", "Ok");				
			}
		}

		private static void ValidateDataPage()
		{
			Application.Current.MainPage.Navigation.PushAsync(new ValidateData());
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
			catch(Exception f)
			{
				Application.Current.MainPage.DisplayAlert("Error ValidateDataPage", $"Error {f.Message}", "OK");
				return null;
			}
		}

		private void LoadManteinance()
		{
			try
			{
				var d = WeakReferenceMessenger.Default.IsRegistered<NameItemViewMessage>(this);
				if (!d)
				{
					if (Maintenance == null)
					{
						WeakReferenceMessenger.Default.Register<NameItemViewMessage>(this, async (r, m) => {
							using (var db = new DBLocalContext())
							{
								Maintenance = db.Maintenance.Where(M => M.MaintenanceId == m.Value).Include(M=>M.Project).FirstOrDefault();
							}
							if (Maintenance != null)
							{
								LoadCategories();
							}
						});
					}
				}
				else
				{
					WeakReferenceMessenger.Default.Unregister<NameItemViewMessage>(this);
				}
				}catch(Exception f)
			{
				Application.Current.MainPage.DisplayAlert("Error LoadMaintenance", $"Error {f.Message}", "OK");
			}
		}

		private void LoadCategories()
		{
			try
			{
				using var db = new DBLocalContext();
				Categories = db.Category.Where(C => C.MaintenanceId == Maintenance.MaintenanceId).Include(C => C.ItemType).ToList();
			}catch(Exception f)
			{
				Application.Current.MainPage.DisplayAlert("Error LoadCategories ", $"Error: {f.Message}", "ok");
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
				using (var db= new DBLocalContext())
				{
					namesOfTypes = (from item in db.ItemTypes select item.Name).ToArray();
				}
				return namesOfTypes;
			}catch (Exception e)
			{
				await Application.Current.MainPage.DisplayAlert("Error GetItemTypeName", e.Message, "OK");
				return null;
			}
		}
		private async void AddCategory()
		{
			try
			{
				var ArrayName = await GetItemTypeName();
				string action = await Application.Current.MainPage.DisplayActionSheet("¿Qué tipo de item deseas añadir?", "Cancelar", null,ArrayName);
				if (action != null)
				{
					if (!action.Equals("Cancelar"))
					{
						string result = await Application.Current.MainPage.DisplayPromptAsync("Alias", "Digite el nombre del item a agregar");
						if (result != null) {
							var ItemType =await GetItemType(action);
							if(ItemType!=null)
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
