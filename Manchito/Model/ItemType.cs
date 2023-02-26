using Manchito.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Model
{
    /// <summary>
    /// Type of the category
    /// </summary>
    public class ItemType
    {
        /// <summary>
        /// Internal Id
        /// </summary>
        public int ItemTypeId { get; set; }
        /// <summary>
        /// Basic Name of the category
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// List of Categories
        /// </summary>
        public ICollection<Category> Categories { get; set; }
    }
}
