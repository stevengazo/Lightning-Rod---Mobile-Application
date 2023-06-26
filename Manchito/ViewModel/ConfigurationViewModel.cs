using Android.App.Backup;
using Android.Systems;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Manchito.DataBaseContext;
using Manchito.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Manchito.ViewModel
{
    public class ConfigurationViewModel : INotifyPropertyChangedAbst
    {


        public ICommand BackupLocalCommand { get { return new Command(async () => BackupFilesLocal()); } set { } }
        public List<Project> Projects { get; set; }

        string BasePath = "/storage/emulated/0/Download/Manchito";
        string BackupPath = string.Empty;
        public ConfigurationViewModel()
        {
            BackupPath = Path.Combine(BasePath,"Copia Seguridad");
        }
        private async void BackupFilesLocal()
        {
            try
            {
                GenerateDirectories();
                using var db = new DBLocalContext();
                Projects = db.Project.ToList();
                foreach (var Project in Projects)
                {
                    string ZipFilePath = Path.Combine(BackupPath, $"P-{Project.ProjectId}_{Project.Name}.zip");
                    string ProjectBasePath = Path.Combine(PathDirectoryFilesAndroid, $"P-{Project.ProjectId}_{Project.Name}");
                    GenerateZipFile(ZipFilePath, ProjectBasePath);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                await MessageToastAsync("No posee permisos", false);
            }
            catch (Exception rfd)
            {
                await MessageToastAsync("Error General" + rfd.Message, false);
            }
        }

      

        private async Task MessageToastAsync(string Message, bool IsLong)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var duration = (IsLong) ? ToastDuration.Long : ToastDuration.Short;
            var toast = Toast.Make(Message, duration, 14);
            await toast.Show(cancellationTokenSource.Token);
        }


        private void GenerateDirectories()
        {
            try
            {
                if (!Directory.Exists(BasePath))
                {
                    Directory.CreateDirectory(BasePath);
                }
                if (!Directory.Exists(BackupPath))
                {
                    Directory.CreateDirectory(BackupPath);
                }
            }
            catch (Exception f)
            {
                MessageToastAsync("Error " + f.Message, false);
            }

        }
        private async Task GenerateZipFile(string ZipFilePath, string BasePath)
        {
            try
            {
                if (File.Exists(ZipFilePath))
                {
                    await MessageToastAsync("Se sobreescribirá la ultima copia", true);
                    File.Delete(ZipFilePath);
                }

                ZipFile.CreateFromDirectory(BasePath, ZipFilePath, CompressionLevel.SmallestSize, true);
                string filename = Path.GetFileName(ZipFilePath);
                MessageToastAsync($"Archivo {filename} generado", false);
            }
            catch (Exception f)
            {
                MessageToastAsync("Error " + f.Message, true);
            }
    
        }
    }
}
