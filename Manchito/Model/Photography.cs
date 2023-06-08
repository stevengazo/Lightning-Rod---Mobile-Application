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
        /// 
        /// </summary>
        public string Name { get; set; }
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
        public Category Category { get; set; }
        /// <summary>
        /// internal id of the parent
        /// </summary>
        public int CategoryId { get; set; }

    }
}
