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

		public ICommand LoadPhotoCommand { get; private set; }

		private ImageSource _UrlPhoto;
		public ImageSource UrlPhoto
		{
			get { return _UrlPhoto; }
			set { _UrlPhoto = value;
				if (UrlPhoto != null) {
					OnPropertyChanged(nameof(UrlPhoto));
				}
			}
		}
		public ViewPhotoViewModel()
		{
			LoadPhotoCommand = new Command(() => { loadPhoto(); }) ;
		}
		private void loadPhoto()
		{
			try
			{
				var temporalDirectory = Path.Combine(FileSystem.AppDataDirectory, "Pictures");
				if (Directory.Exists(temporalDirectory))
				{
					var datas = Directory.GetFiles(temporalDirectory).FirstOrDefault();
					UrlPhoto = ImageSource.FromFile(datas);
				}
			}catch(Exception ex)
			{
				Application.Current.MainPage.DisplayAlert("Error LoadPhoto Function", ex.Message, "ok");
			}
			
		}
	}
}
