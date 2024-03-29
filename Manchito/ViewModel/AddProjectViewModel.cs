﻿using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Manchito.DataBaseContext;
using Manchito.Model;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace Manchito.ViewModel
{
    public class AddProjectViewModel : INotifyPropertyChangedAbst
    {
        #region Properties		

        private Project _Project = new();

        private string _Alias;
        public string Alias
        {
            get { return _Alias; }
            set
            {
                _Alias = value;
                if (_Alias != null)
                {
                    OnPropertyChanged(nameof(Alias));
                }
            }
        }
        private DateTime _DateProject;
        public DateTime DateProject
        {
            get { return _DateProject; }
            set
            {
                _DateProject = value;
                if (_DateProject != null)
                {
                    OnPropertyChanged(nameof(DateProject));
                }
            }
        }
        private string _Status;
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                if (_Status != null)
                {
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                _customerName = value;
                if (_customerName != null)
                {
                    OnPropertyChanged(nameof(CustomerName));
                }
            }
        }
        private string _CustomerContactName;
        public string CustomerContactName
        {
            get { return _CustomerContactName; }
            set
            {
                _CustomerContactName = value;
                if (CustomerContactName != null)
                {
                    OnPropertyChanged(nameof(CustomerContactName));
                }
            }
        }
        public ICommand AddProjectCommand { get { return new Command(async () => await AddProject()); } private set { } }
        private string _ErrorMessage;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                if (_ErrorMessage != null)
                {
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }
        #endregion

        #region Methods
        public AddProjectViewModel()
        {
            // Actual date
            DateProject = DateTime.Today;



        }

        public async Task<int> GetLastIdProject()
        {
            try
            {
                using (var dblocal = new DBLocalContext())
                {
                    int queryResult = await (from proj in dblocal.Project
                                             orderby proj.ProjectId descending
                                             select proj.ProjectId
                                    ).FirstOrDefaultAsync();
                    return queryResult;
                }
            }
            catch (Exception f)
            {
                return -1;
            }
        }
        private async void ClosedPage()
        {
            var page = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
            Application.Current.MainPage.Navigation.RemovePage(page);
        }


        public bool AddDirectoryAndroid(string ProjectName, int ProjectId)
        {
            try
            {
                var DirectoryPath = Path.Combine(PathDirectoryFilesAndroid, $"P-{ProjectId.ToString()}_{ProjectName}");
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception f)
            {
                Application.Current.MainPage.DisplayAlert("AddDirectoryAndroid Error", $"Error: {f.Message}", "Ok");
                return false;
            }
        }
        public async Task AddProject()
        {
            try
            {
                if (Alias == null || Status == null)
                {
                    ErrorMessage = "No hay datos cargados";
                    await Application.Current.MainPage.DisplayAlert("Error", $"{ErrorMessage}", "Ok");
                }
                else
                {
                    int projectid = await GetLastIdProject();
                    projectid++;
                    using (var dbLocal = new DBLocalContext())
                    {
                        // add the project to the db
                        Model.Project tmpProject = new Model.Project()
                        {
                            ProjectId = projectid,
                            Name = Alias,
                            CustomerName = this._customerName,
                            CustomerContactName = this._CustomerContactName,
                            Status = this._Status
                        };
                        dbLocal.Project.Add(tmpProject);
                        dbLocal.SaveChanges();
                        AddDirectoryAndroid(tmpProject.Name, tmpProject.ProjectId);

                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        var toast = Toast.Make($"Proyecto Agregado con éxito\nProjecto {tmpProject.ProjectId}\nCliente {tmpProject.Name}", ToastDuration.Long, 14);
                        await toast.Show(cancellationTokenSource.Token);
                        ClosedPage();
                    }
                }
            }
            catch (Exception f)
            {
                ErrorMessage = $"Error interno: {f.Message}";
                await Application.Current.MainPage.DisplayAlert("Error", $"Error Interno {f.Message}", "Ok");
                ClosedPage();
            }
        }

        public async Task<Project> GetProjectAsync(int id)
        {
            try
            {
                using (var db = new DBLocalContext())
                {
                    var data = await (from i in db.Project
                                      where i.ProjectId == id
                                      select i).FirstOrDefaultAsync();
                    return data;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion
    }
}
