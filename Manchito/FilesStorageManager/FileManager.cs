
namespace Manchito.FilesStorageManager
{
	/// <summary>
	/// Manage the access and load of the files in the app (Android system)
	/// </summary>
	public static class FileManager
	{
		/// <summary>
		/// Get the base path 
		/// </summary>
		/// <returns></returns>
		public static string GetBase() {
			return "/storage/emulated/0/Proyectos";
		}

		/// <summary>
		/// Check if the folder exists
		/// </summary>
		/// <param name="path">Path of the folder specify</param>
		/// <returns>True if the folder exist, False if present an error</returns>
		public static bool CheckFolder(string path)
		{
			try
			{

				return true;
			}
			catch (Exception f)
			{
				Console.WriteLine(f.Message);
				return false;
			}
		}

		/// <summary>
		/// Create a new folder in the path
		/// </summary>
		/// <param name="path">Path of the new folder</param>
		/// <returns>True if the folder was created, False if present an error</returns>
		public static bool CreateFolder(string path)
		{
			try
			{

				return true;
			}catch(Exception f)
			{
				Console.WriteLine(f.Message);
				return false;
			}
		}
		/// <summary>
		/// Save an image in the specify path
		/// </summary>
		/// <param name="path">Path to save the image</param>
		/// <param name="image">Image to save</param>
		/// <returns>True if the image was save, False if present an error </returns>
		public static bool SaveImageFile(string path, Image image)
		{
			try
			{

				return true;
			}
			catch (Exception f)
			{
				Console.WriteLine(f.Message);
				return false;
			}
		}

	}
}
