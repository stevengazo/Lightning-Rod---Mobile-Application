using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Model
{
    /// <summary>
    /// Base class of the projects
    /// </summary>
   public class Project
    {
        /// <summary>
        /// Internal number to identify the project
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// Name of the project
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Name of the Customer
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Name of the contact for the project
        /// </summary>
        public string CustomerContactName { get;set; } 
        /// <summary>
        /// List of maintenances part of the project
        /// </summary>
        public ICollection<Maintenance> Maintenances { get; set; }

    }
}
