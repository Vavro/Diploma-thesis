using System.Collections.Generic;

namespace DragqnLD.Core.Abstraction.Indexes
{
    public class DragqnLDIndexRequirements
    {
        public DragqnLDIndexRequirements()
        {
            PropertyPaths = new List<PropertyToIndex>();
        }

        public List<PropertyToIndex> PropertyPaths { get; set; }
    }
}