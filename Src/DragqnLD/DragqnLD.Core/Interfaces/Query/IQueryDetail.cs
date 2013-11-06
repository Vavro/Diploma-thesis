namespace DragqnLD.Core.Interfaces.Query
{
    public interface IQueryDetail
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        IQueryKey Key { get; }

        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <value>
        /// The definition.
        /// </value>
        IQueryDefinition Definition { get; }
    }
}