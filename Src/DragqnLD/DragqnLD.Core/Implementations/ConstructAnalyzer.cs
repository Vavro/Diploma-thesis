using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Patterns;

namespace DragqnLD.Core.Implementations
{
    public class ConstructAnalyzer : IConstructAnalyzer
    {
        class Abbreviations
        {
            public void AddPrefix(string prefix, Uri getNamespaceUri)
            {
                
            }

            public bool TryGetPrefixedForm(Uri uri, out string prefix)
            {
                prefix = "n";
                return false;
            }

            public string CreatePrefixedFormFor(Uri uri)
            {
                return "n";
            }

            public static bool ShouldBeSkipped(Uri uri)
            {
                return false;
            }

            public void AddRenameIfNeeded(string prefixedForm)
            {
                
            }
        }

        public Context CreateCompactionContextForQuery(QueryDefinition queryDefinition)
        {
            var constructQuery = queryDefinition.ConstructQuery.Query;
            var constructParameterName = queryDefinition.ConstructQueryUriParameterName;
            
            var parser = new SparqlQueryParser();
            var sparqlQuery =parser.ParseFromString(constructQuery);

            var abbreviations = new Abbreviations();
            
            //todo: for context abbreviations - enumerate ConstructTemplate.TripplePattern - Predicate
            var namespaceMap = sparqlQuery.NamespaceMap;
            foreach (var prefix in namespaceMap.Prefixes)
            {
                abbreviations.AddPrefix(prefix, namespaceMap.GetNamespaceUri(prefix));
            }

            var constructTriples = sparqlQuery.ConstructTemplate.TriplePatterns;
            //construct template can only contain TriplePatterns - match type triples
            var predicates = constructTriples.Cast<TriplePattern>().Select(triple => triple.Predicate);
            var predicatesAsUris =
                predicates.Cast<NodeMatchPattern>()
                    .Select(match => match.Node)
                    .Cast<UriNode>()
                    .Select(uriNode => uriNode.Uri);

            foreach (var uri in predicatesAsUris)
            {
                //detect whether it already has a prefix
                // - yes, replace and add abreviation so that there is no ":"
                // - no, create prefix and abreviate

                //skip special cases - like "a" == "type"
                if (Abbreviations.ShouldBeSkipped(uri))
                {
                    continue;
                }

                string prefixedForm;
                var containsPrefix = abbreviations.TryGetPrefixedForm(uri, out prefixedForm);

                if (!containsPrefix)
                {
                    prefixedForm = abbreviations.CreatePrefixedFormFor(uri);
                }

                //now there should be max a ":" character that will get abbreviated
                abbreviations.AddRenameIfNeeded(prefixedForm);
            }
            

            var constructVariables = sparqlQuery.ConstructTemplate.Variables;
            


            //todo: for hierarchi model 
            // - hierarchical class that will handle this
            // --- might be used by the flatGraphNester
            // --- Detect usage of same variable in multiple branches
            // - enumarate ConstructTemplate.TripplePattern collection according to starting parameter and continue by scheduling further probes
            return new Context();
        }
    }
}
