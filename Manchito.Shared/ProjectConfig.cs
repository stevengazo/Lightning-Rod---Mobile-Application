namespace Manchito.Shared
{
	public static class ProjectConfig
	{
		public static string DatabasePath => $"Filename={Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\maui-test-database.db";
	}
}