using System.Collections.Generic;

namespace DragqnLD.Core.Abstraction.Indexes
{
    public class DragqnLDIndexDefiniton
    {
        public string Name { get; set; }
        public DragqnLDIndexRequirements Requirements { get; set; }

        public string RavenMap { get; set; }
        public IDictionary<string, string> RavenAnalyzers { get; set; }
        public Dictionary<string, string> PropertyNameMap { get; set; }
    }
}