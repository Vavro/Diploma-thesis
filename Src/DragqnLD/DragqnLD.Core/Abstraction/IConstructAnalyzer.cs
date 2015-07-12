using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Query;
using Raven.Json.Linq;

namespace DragqnLD.Core.Abstraction
{
    public class CompactionContext
    {
        private Dictionary<string, string> _uriToAbbreviation;

        public CompactionContext(RavenJObject buildContext, Dictionary<string, string> uriToAbbreviation)
        {
            BuildContext = buildContext;
            _uriToAbbreviation = uriToAbbreviation;
            UriToAbbreviation = new ReadOnlyDictionary<string, string>(_uriToAbbreviation);
        }

        public RavenJObject BuildContext { get; private set; }
        public IReadOnlyDictionary<string, string> UriToAbbreviation { get; private set; }
    }

    public interface IConstructAnalyzer
    {
        CompactionContext CreateCompactionContextForQuery(IParsedSparqlQuery queryDefinition);
        ConstructQueryAccessibleProperties CreatePropertyPathsForQuery(IParsedSparqlQuery parsedSparqlQuery, CompactionContext compactionContext);
    }
}
