using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Model
{
    /// <summary>
    /// Item class
    /// </summary>
    public class Item
    {
        /// <summary>
        /// internal id
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// list of childrens
        /// </summary>
        public ICollection<Photography> Photographies { get; set; }
		/// <summary>
		/// list of childrens
		/// </summary>
		public ICollection<Note> Notes { get; set; }        
    }
}
