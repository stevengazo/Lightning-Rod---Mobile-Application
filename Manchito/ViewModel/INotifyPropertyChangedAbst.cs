using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.ViewModel
{
	public abstract class INotifyPropertyChangedAbst: INotifyPropertyChanged
	{
		public readonly string PathDirectoryFilesAndroid = "/storage/emulated/0/Pictures";
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
