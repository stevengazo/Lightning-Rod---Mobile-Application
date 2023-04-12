using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Manchito.ViewModel
{
	public class ViewPhotoViewModel : INotifyPropertyChangedAbst
	{

		private string _UrlPhoto;
		public string UrlPhoto
		{
			get { return _UrlPhoto; }
			set { _UrlPhoto = value;
				if (UrlPhoto != null) {
					OnPropertyChanged(nameof(UrlPhoto));
				}
			}
		}
		public ICommand LoadPhotoCommand { get; private set; }

		public ViewPhotoViewModel()
		{
			LoadPhotoCommand = new Command(()=>loadPhoto());
		}

		private void loadPhoto()
		{
			try
			{
				var temporalDirectory = Path.Combine(FileSystem.AppDataDirectory, "Pictures");
				if (Directory.Exists(temporalDirectory))
				{
					var datas = Directory.GetFiles(temporalDirectory).FirstOrDefault();
					UrlPhoto = datas;
				}
			}catch(Exception ex)
			{
				Application.Current.MainPage.DisplayAlert("Error loadPhoto", ex.Message, "ok");
			}
			
		}
	}
}
