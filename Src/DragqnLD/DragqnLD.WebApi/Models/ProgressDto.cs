namespace DragqnLD.WebApi.Models
{
    /// <summary>
    /// Data transfer object for Progress
    /// </summary>
    public class ProgressDto
    {
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        /// <value>
        /// The current item.
        /// </value>
        public int CurrentItem { get; set; }
    }
}