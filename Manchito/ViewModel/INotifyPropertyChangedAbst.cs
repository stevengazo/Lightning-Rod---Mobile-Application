using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Manchito.ViewModel
{
    public abstract class INotifyPropertyChangedAbst : INotifyPropertyChanged
    {
        public readonly string PathDirectoryFilesAndroid = FileSystem.Current.AppDataDirectory;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
