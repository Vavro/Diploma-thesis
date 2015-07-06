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
            private class AbbreviationsStore
            {
                private readonly Dictionary<string, string> _prefixFormToAbbreviationDict = new Dictionary<string, string>();
                private const char ZeroChar = '0';
                private readonly char[] Numbers = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                private readonly Dictionary<string,List<int>> _abbreviations = new Dictionary<string, List<int>>();

                public bool ContainsAbbreviationFor(string prefixedForm)
                {
                    return _prefixFormToAbbreviationDict.ContainsKey(prefixedForm);
                }

                /// <summary>
                /// Checks the and add index if need.
                /// </summary>
                /// <param name="newName">The new name.</param>
                /// <param name="prefixedForm">The prefixed form. cant be in the abbreviations already</param>
                public void CheckAndAddIndexIfNeed(string newName, string prefixedForm)
                {
                    if (ContainsAbbreviationFor(prefixedForm))
                    {
                        throw new ArgumentException("prefixed form already abbreviated, it makes no sense to call this");
                    }

                    string finalName;
                    //check for tailing numbers
                    var lastChar = newName[newName.Length - 1];
                    var numberCheck = lastChar - ZeroChar;
                    //if tailing numbers -> get base without numbers -> add base to abbreviations and add the specified number
                    if (numberCheck >= 0 && numberCheck <= 9)
                    {
                        //tailing number
                        var lastIndexOfNumber = newName.LastIndexOfAny(Numbers);
                        var baseName = newName.Substring(0, lastIndexOfNumber - 1);
                        var tailingNumber = int.Parse(newName.Substring(lastIndexOfNumber));

                        List<int> indexes;
                        var succ = _abbreviations.TryGetValue(baseName, out indexes);
                        if (succ)
                        {
                            if (indexes.Contains(tailingNumber))
                            {
                                finalName = AddNewIndex(indexes, baseName);
                            }
                            else
                            {
                                indexes.Add(tailingNumber);
                                //ineffective, but keeps the index list in sorted order
                                indexes.Sort();
                                finalName = newName;
                            }
                        }
                        else
                        {
                            _abbreviations.Add(baseName, new List<int>() {tailingNumber});
                            finalName = newName;
                        }
                    }
                    //else check abbreviations for existing same string
                    //  not existing -> add abbreviation with empty list
                    //  exists -> check list and get a new number
                    else
                    {
                        List<int> indexes;
                        var succ = _abbreviations.TryGetValue(newName, out indexes);
                        if (succ)
                        {
                            finalName = AddNewIndex(indexes, newName);
                        }
                        else
                        {
                            _abbreviations.Add(newName, new List<int>());
                            finalName = newName;
                        }
                    }
                    
                    //final name is checked by compiler to be assigned :)
                    _prefixFormToAbbreviationDict.Add(prefixedForm, finalName);
                }

                private static string AddNewIndex(List<int> indexes, string baseName)
                {
                    var newIndex = indexes.LastOrDefault();
                    newIndex++;
                    indexes.Add(newIndex);
                    string finalName = baseName + newIndex;
                    return finalName;
                }
            }

            private class UniquePrefixCreater
            {
                private int index = 0;

                public string GetNewPrefix()
                {
                    var prefix = "n" + index;
                    index++;
                    return prefix;
                }
            }
            
            private readonly Dictionary<string, string> _uriToPrefixDict = new Dictionary<string, string>();
            private readonly char[] _uriPrefixSplitCharacters = { '#', '/' };
            private readonly char[] _renameReasonCharacters = { ':' };
            private readonly UniquePrefixCreater _prefixCreater = new UniquePrefixCreater();
            private readonly AbbreviationsStore _abbreviationsStore = new AbbreviationsStore();

            public void AddPrefix(string prefix, Uri namespaceUri)
            {
                _uriToPrefixDict.Add(namespaceUri.AbsoluteUri, prefix);
            }

            public bool TryGetPrefixedForm(Uri uri, out string prefixedForm)
            {
                //this will be slow, could be improved with aho-corasick and the like
                //but probably sufficient
                var searchedUri = uri.AbsoluteUri;

                foreach (var uriPrefixPair in _uriToPrefixDict)
                {
                    if (searchedUri.StartsWith(uriPrefixPair.Key))
                    {
                        prefixedForm = searchedUri.ReplaceNamespaceWithPrefix(uriPrefixPair.Key, uriPrefixPair.Value);
                        return true;
                    }
                }

                prefixedForm = null;
                return false;
            }

            public string CreatePrefixedFormFor(Uri uri)
            {
                var uriToPrefix = uri.AbsoluteUri;
                //take last "/" or "#" and make it a prefix
                var splitIndex = uriToPrefix.LastIndexOfAny(_uriPrefixSplitCharacters);

                if (splitIndex == -1)
                {
                    //shouldn't happen....
                    throw new ArgumentException("Couldn't abbreviate uri");
                }
                var namespaceToAbbreviate = uriToPrefix.Substring(0, splitIndex + 1);
                var newPrefix = _prefixCreater.GetNewPrefix();

                _uriToPrefixDict.Add(namespaceToAbbreviate, newPrefix);
                var prefixedForm = uriToPrefix.ReplaceNamespaceWithPrefix(namespaceToAbbreviate, newPrefix);

                return prefixedForm;
            }

            public static bool ShouldBeSkipped(Uri uri)
            {
                return false;
            }

            public void AddRenameIfNeeded(string prefixedForm)
            {
                var renameReasonIndex = prefixedForm.LastIndexOfAny(_renameReasonCharacters);
                if (renameReasonIndex == -1)
                {
                    return;
                }

                var succ = _abbreviationsStore.ContainsAbbreviationFor(prefixedForm);
                if (succ)
                {
                    //already abbreviated and will get into the created context
                    return;
                }

                var newName = prefixedForm.Substring(renameReasonIndex + 1);

                _abbreviationsStore.CheckAndAddIndexIfNeed(newName, prefixedForm);
            }
        }

        public Context CreateCompactionContextForQuery(QueryDefinition queryDefinition)
        {
            var constructQuery = queryDefinition.ConstructQuery.Query;
            var constructParameterName = queryDefinition.ConstructQueryUriParameterName;

            var parser = new SparqlQueryParser();
            var sparqlQuery = parser.ParseFromString(constructQuery);

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

    public static class UriStringExtensions
    {
        public static string ReplaceNamespaceWithPrefix(this string searchedUri, string namespaceToAbbreviate, string prefix)
        {
            return searchedUri.Replace(namespaceToAbbreviate, prefix + ":");
        }
    }
}
