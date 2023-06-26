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


        public ConfigurationViewModel()
        {

        }
        private async void BackupFilesLocal()
        {
            try
            {

                string BackupPath = "/storage/emulated/0/Download/Manchito/Backups";
                if (!Directory.Exists(BackupPath) || !Directory.Exists("/storage/emulated/0/Download/Manchito"))
                {
                    if (!Directory.Exists("/storage/emulated/0/Download/Manchito"))
                    {
                        Directory.CreateDirectory("/storage/emulated/0/Download/Manchito");
                    }
                    Directory.CreateDirectory(BackupPath);
                }
                var ProjectList = new List<Project>();
                using var db = new DBLocalContext();
                ProjectList = db.Project.ToList();
                foreach (var item in ProjectList)
                {
                    string RootProjectPath = Path.Combine(PathDirectoryFilesAndroid, $"P-{item.ProjectId}_{item.Name}");
                    string FilePathZip = Path.Combine(BackupPath, $"P-{item.ProjectId}-{item.Name}.zip");
                    await GenerateBackup(RootProjectPath, FilePathZip);
                    await MessageToastAsync($"Generando copia de {item.Name}", true);


                    var JsonFile = Path.Combine(BackupPath, $"Information P-{item.ProjectId}-{item.Name}.json");
                    var JsonDAta = JsonConvert.SerializeObject(item, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    using (StreamWriter sw = new StreamWriter(JsonFile))
                    {
                        await sw.WriteAsync(JsonDAta);
                    }

                }

            }
            catch (UnauthorizedAccessException e)
            {
                await MessageToastAsync("No posee permisos", false);
                await CheckForStoragePermission();
            }
            catch (Exception rfd)
            {
                await MessageToastAsync("Error General" + rfd.Message, false);
            }
        }

        public async Task CheckForStoragePermission()
        {

            var statusStorageRead = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (statusStorageRead != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.StorageRead>();
            }
            var statusStorageWrite = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (statusStorageWrite != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.StorageWrite>();
            }
            var statusStoragePhotos = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (statusStoragePhotos != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.Photos>();
            }

            var statusMedia = await Permissions.CheckStatusAsync<Permissions.Media>();
            if (statusMedia != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.Media>();
            }
        }

        private async Task MessageToastAsync(string Message, bool IsLong)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var duration = (IsLong) ? ToastDuration.Long : ToastDuration.Short;
            var toast = Toast.Make(Message, duration, 14);
            await toast.Show(cancellationTokenSource.Token);
        }
        private async Task GenerateBackup(string basePath, string FilePath)
        {
            if (File.Exists(FilePath))
            {
               await MessageToastAsync("Se sobreescribirá la ultima copia", false);
                File.Delete(FilePath);
            }
            ZipFile.CreateFromDirectory(basePath, FilePath, CompressionLevel.SmallestSize, true);
        }
    }
}
