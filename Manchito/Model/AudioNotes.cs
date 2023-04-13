using Manchito.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Model
{
	/// <summary>
	/// Class to define the Audio Notes Recorded
	/// </summary>
	public class AudioNote
	{
		/// <summary>
		/// internal id of the audio note
		/// </summary>
		public int AudioNoteId { get; set; }
		/// <summary>
		/// path of the audio file
		/// </summary>
		public string PathFile { get; set; }
		/// <summary>
		/// Internal id of the parent item
		/// </summary>
		public Category Category { get; set; }
		/// <summary>
		/// Parent Item
		/// </summary>
		public int CategoryId { get; set; }	
		
	}
}
