using Manchito.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Model
{
	/// <summary>
	/// Class to register the photos
	/// </summary>
	public class Photography
	{
		/// <summary>
		/// internal id
		/// </summary>
		public int PhotographyId { get; set; }
		/// <summary>
		/// path of the file
		/// </summary>
		public string FilePath { get; set; }
		/// <summary>
		/// Date of was take the fotography
		/// </summary>
		public DateTime DateTaked { get; set; }
		/// <summary>
		/// Parent of this object
		/// </summary>
		public Item Item { get; set; }
		/// <summary>
		/// internal id of the parent
		/// </summary>
		public int ItemId { get; set; }

	}
}
