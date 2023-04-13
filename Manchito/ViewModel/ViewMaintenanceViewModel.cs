using Android.Widget;
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
		public ViewMaintenance _ViewMaintenance { get; set; }
		#endregion

		#region Methods
		public ViewMaintenanceViewModel()
		{
			AppearingCommand = new Command(() => LoadManteinance());
			ValidateDataCommand = new Command(() => ValidateDataPage());
			AddCategoryCommand = new Command(() => AddCategory());
			ViewCategoryCommand = new Command((O) => ViewCategory(O));
		}

		private async void ViewCategory(Object id)
		{
			try
			{
				int idNumber = int.Parse(id.ToString());
				await Application.Current.MainPage.Navigation.PushAsync(new ViewItem(), true);
				WeakReferenceMessenger.Default.Send(new NameItemViewMessage(idNumber));
			}
			catch(Exception f)
			{
				await Application.Current.MainPage.DisplayAlert("Error ViewCategory", f.Message, "OK");

			}
		}

		private void ValidateDataPage()
		{
			Application.Current.MainPage.Navigation.PushAsync(new ValidateData());
		}
		private Project GetProject(int id)
		{
			try
			{
				using(var db = new DBLocalContext())
				{
					var data = (from i in db.Project
								where i.ProjectId == id
								select i).FirstOrDefault();
					return data;
				}
			}catch(Exception f)
			{
				return null;
			}
		}

		private void LoadManteinance()
		{
			try
			{
				if(Maintenance == null)
				{
					WeakReferenceMessenger.Default.Register<NameItemViewMessage>(this, async (r, m) => {
						using (var db = new DBLocalContext())
						{
							Maintenance = db.Maintenance.Where(M => M.MaintenanceId == m.Value).FirstOrDefault();
						}
						if(Maintenance != null)
						{
							loadCategories();
						}
					});
				}
				else
				{
					WeakReferenceMessenger.Default.Unregister<NameItemViewMessage>(this);
				}
				}catch(Exception f)
			{
				Application.Current.MainPage.DisplayAlert("error", $"Error {f.Message}", "OK");
			}
		}

		private void loadCategories()
		{
			using(var db = new DBLocalContext())
			{
				Categories = db.Category.Where(C => C.MaintenanceId == Maintenance.MaintenanceId).Include(C=>C.ItemType).ToList();
			}
		}
		private async Task<int> GetLastCategoryId()
		{
			try
			{
				using (var db = new DBLocalContext())
				{
					return (from i in db.Category
							orderby i.CategoryId descending
							select i.CategoryId).FirstOrDefault();
				}
			}
			catch (Exception e)
			{
				await Application.Current.MainPage.DisplayAlert("Error GetItemTypeName", e.Message, "OK");
				return -1;
			}
		}
		private async Task<ItemType> GetItemType(int id)
		{
			try
			{
				using (var db = new DBLocalContext())
				{
					return (from item in db.ItemTypes
							where item.ItemTypeId == id
							select item).FirstOrDefault();
				}
			}
			catch (Exception e)
			{
				await Application.Current.MainPage.DisplayAlert("Error GetItemTypeName", e.Message, "OK");
				return null;
			}
		}
		private async Task<ItemType> GetItemType(string name)
		{
			try
			{
				using (var db = new DBLocalContext())
				{
					return (from item in db.ItemTypes
							where item.Name.Equals(name)
							select item).FirstOrDefault();
				}
			}
			catch (Exception e)
			{
				await Application.Current.MainPage.DisplayAlert("Error GetItemTypeName", e.Message, "OK");
				return null;
			}
		}
		private async Task<string[]> GetItemTypeName()
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
								using( var db = new DBLocalContext())
								{
									Category categoryTmp = new Category() {
										CategoryId = (await GetLastCategoryId() + 1),
										Alias = result,
										ItemTypeId = ItemType.ItemTypeId,
										MaintenanceId = Maintenance.MaintenanceId										
									};
									// cargar datos
									db.Category.Add(categoryTmp);
									db.SaveChanges();
									// cargar datos de nuevo
									loadCategories();
									var project = GetProject(Maintenance.ProjectId);
									var DirectoryPath = Path.Combine(PathDirectoryFilesAndroid, $"{project.ProjectId.ToString()}-{project.Name}", $"{categoryTmp.MaintenanceId.ToString()}-{categoryTmp.Alias}");
								}
							}
						}
					}
				}				
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Error {ex.Message}", null);
			}
		}


		#endregion
	}
}
