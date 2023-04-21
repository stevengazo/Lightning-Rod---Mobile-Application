using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Manchito.ViewModel
{
	public abstract class INotifyPropertyChangedAbst : INotifyPropertyChanged
	{
		public readonly string PathDirectoryFilesAndroid = "/storage/emulated/0/Pictures/Manchito";
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
