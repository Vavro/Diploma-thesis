using System.ComponentModel.DataAnnotations;
using DragqnLD.WebApi.Annotations;

namespace DragqnLD.WebApi.Models
{
    public class QueryDefinitionMetadataDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        //todo: required? - for new query def it might not be
        [UsedImplicitly]
        public string Id { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        [UsedImplicitly]
        public string Name { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [UsedImplicitly]
        public string Description { get; set; }        
    }
}