namespace Manchito.Model
{
	/// <summary>
	/// Define objets type maintenance
	/// </summary>
	public class Maintenance
	{
		/// <summary>
		/// Internal Number to identify the project
		/// </summary>
		public int MaintenanceId { get; set; }
		/// <summary>
		/// Date of the begin of the maintenance
		/// </summary>
		public DateTime DateOfMaintenance { get; set; }
		/// <summary>
		/// Status of the maintenance
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// Alias to identify the maintenance (site, etc)
		/// </summary>
		public string Alias { get; set; }
		/// <summary>
		/// Project Owner of maintenance
		/// </summary>
		public Project Project { get; set; }
		/// <summary>
		/// Id of the project owner
		/// </summary>
		public int ProjectId { get; set; }
	}
}
