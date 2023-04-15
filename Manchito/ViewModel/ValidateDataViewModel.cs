using Manchito.Model;

namespace Manchito.ViewModel
{
	public class ValidateDataViewModel : INotifyPropertyChangedAbst
	{
		#region Properties
		private List<Category> _Categories;
		public List<Category> Categories
		{
			get { return _Categories; }
			set
			{
				_Categories = value;
				if (Categories != null)
				{
					OnPropertyChanged(nameof(Categories));
				}
			}
		}
		#endregion
		#region Methods
		public ValidateDataViewModel()
		{
		}
		#endregion
	}
}
