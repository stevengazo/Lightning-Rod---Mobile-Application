using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Model
{
    /// <summary>
    /// Class to save the records
    /// </summary>
    public class Note
    {
        /// <summary>
        /// internal id
        /// </summary>
        public int NoteId { get; set; }
        /// <summary>
        /// file path
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// parent item
        /// </summary>
        public Category Category { get; set; }
        /// <summary>
        /// parent item id
        /// </summary>
        public int CategoryId { get; set; }
    }
}
