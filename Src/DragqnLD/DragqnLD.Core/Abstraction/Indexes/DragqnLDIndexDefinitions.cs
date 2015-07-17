using System.Collections.Generic;

namespace DragqnLD.Core.Abstraction.Indexes
{
    public class DragqnLDIndexDefinitions
    {
        public DragqnLDIndexDefinitions()
        {
            Indexes = new Dictionary<string, DragqnLDIndexDefiniton>();
        }

        public string DefinitionId { get; set; }
        public Dictionary<string, DragqnLDIndexDefiniton> Indexes { get; private set; }
    }
}