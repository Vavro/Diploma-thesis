namespace DragqnLD.Core.Abstraction.Query
{
    /// <summary>
    /// Defines an escaped property
    /// </summary>
    public class PropertyEscape
    {
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string OldName { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        public string NewName { get; set; }
    }
}