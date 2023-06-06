namespace Manchito.Model
{
    /// <summary>
    /// Object of the type of category part of the maintenance
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Internal Id of the category for their identification
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Alias to recognize the category
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// Parent of the category
        /// </summary>
        public Maintenance Maintenance { get; set; }
        /// <summary>
        /// id internal of the Parent of the category
        /// </summary>
        public int MaintenanceId { get; set; }
        /// <summary>
        /// Item type to group the categories
        /// </summary>
        public ItemType ItemType { get; set; }
        /// <summary>
        /// Internal id of the item type
        /// </summary>
        public int ItemTypeId { get; set; }
        /// <summary>
        /// List of item associate with the category
        /// </summary>
        public ICollection<Photography> Photographies { get; set; }
        public ICollection<Note> Notes { get; set; }
    }
}
