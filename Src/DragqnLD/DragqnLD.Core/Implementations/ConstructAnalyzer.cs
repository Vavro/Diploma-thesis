using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using Raven.Json.Linq;
using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Patterns;

namespace DragqnLD.Core.Implementations
{
    public static class ConstructAnalyzerHelper
    {
        private class ParsedSparqlQuery : IParsedSparqlQuery
        {
            public ParsedSparqlQuery()
            {

            }

            public SparqlQuery Query { get; set; }
            public string StartingParameterName { get; set; }
        }

        public static IParsedSparqlQuery ReplaceParamAndParseConstructQuery(QueryDefinition queryDefinition)
        {
            var constructQuery = queryDefinition.ConstructQuery.Query;
            var constructParameterName = queryDefinition.ConstructQueryUriParameterName;
            //contsruct query contains @Variable that has to be substituted for ?Variable for the parser
            var paramNameToReplace = '@' + constructParameterName;
            var paramNameToReplaceBy = '?' + constructParameterName;
            var replacedParamQuery = constructQuery.Replace(paramNameToReplace, paramNameToReplaceBy);

            var parser = new SparqlQueryParser();
            var sparqlQuery = parser.ParseFromString(replacedParamQuery);

            var result = new ParsedSparqlQuery() { Query = sparqlQuery, StartingParameterName = paramNameToReplaceBy };
            return result;
        }
    }
    public class ConstructAnalyzer : IConstructAnalyzer
    {
        class Abbreviations
        {
            private class AbbreviationsStore
            {
                private readonly Dictionary<string, string> _prefixFormToAbbreviationDict = new Dictionary<string, string>();
                private const char ZeroChar = '0';
                private readonly char[] Numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                private readonly Dictionary<string, List<int>> _abbreviations = new Dictionary<string, List<int>>();
                private IReadOnlyDictionary<string, string> _readOnlyPrefixFormToAbbreviationDic;

                public AbbreviationsStore()
                {
                    _readOnlyPrefixFormToAbbreviationDic = new ReadOnlyDictionary<string, string>(_prefixFormToAbbreviationDict);
                }

                public IReadOnlyDictionary<string, string> PrefixFormToAbbreviation
                {
                    get
                    {
                        return _readOnlyPrefixFormToAbbreviationDic;
                    }
                }

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
                            _abbreviations.Add(baseName, new List<int>() { tailingNumber });
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

            private readonly IReadOnlyDictionary<string, string> _readonlyUriToPrefixes;
            public Abbreviations()
            {
                _readonlyUriToPrefixes = new ReadOnlyDictionary<string, string>(_uriToPrefixDict);
            }

            public IReadOnlyDictionary<string, string> UriToPrefixes
            {
                get
                {
                    return _readonlyUriToPrefixes;
                }
            }

            public IReadOnlyDictionary<string, string> PrefixFormToAbbreviation
            {
                get { return _abbreviationsStore.PrefixFormToAbbreviation; }
            }

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

            private static readonly Uri TypeUri = new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
            public static bool ShouldBeSkipped(Uri uri)
            {
                return uri == TypeUri;
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

        public RavenJObject CreateCompactionContextForQuery(IParsedSparqlQuery parsedSparqlQuery)
        {
            var sparqlQuery = parsedSparqlQuery.Query;

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

            return CreateContextForAbbreviations(abbreviations);

        }

        public class ConstructQueryAccessibleProperties
        {
            public IIndexableProperty RootProperty;
        }

        public enum ValuePropertyType
        {
            ObjectId,
            Value,
            LanguageString,
            ArrayOfValue,
            ArrayOfLanguageString
        }

        public class IndexableValueProperty : IIndexableProperty
        {
            //can be filled only by inspecting data (the predicate doesn't contain array info)
            ValuePropertyType? Type = null;
        }

        public interface IIndexableProperty { }

        public class IndexableObjectProperty : IIndexableProperty
        {
            private Dictionary<string, IIndexableProperty> _childProperties = new Dictionary<string, IIndexableProperty>();

            public void AddProperty(string propertyName, IIndexableProperty property)
            {
                _childProperties.Add(propertyName, property);
            }
        }

        private class HierarchyBuilder
        {
            private readonly Dictionary<string, List<IMatchTriplePattern>> _triplePatternsBySubjectParameter;

            public HierarchyBuilder(Dictionary<string, List<IMatchTriplePattern>> triplePatternsBySubjectParameter)
            {
                _triplePatternsBySubjectParameter = triplePatternsBySubjectParameter;
            }

            public ConstructQueryAccessibleProperties BuildHierarchyFrom(string startingParameter)
            {
                var rootProperty = ConstructPropertyFrom(startingParameter, true);
                var accessibleProps = new ConstructQueryAccessibleProperties() { RootProperty = rootProperty };

                return accessibleProps;
            }

            private IIndexableProperty ConstructPropertyFrom(string propertyName, bool first)
            {
                List<IMatchTriplePattern> properties;
                var succ = _triplePatternsBySubjectParameter.TryGetValue(propertyName, out properties);
                if (!succ)
                {
                    if (first) //the first property has to be there
                    {
                        throw new ArgumentException(String.Format("Starting parameter {0} is not present in the ConstructTemplate", propertyName));
                    }
                    
                    return new IndexableValueProperty();
                }

                var thisObject = new IndexableObjectProperty();

                foreach (var matchTriplePattern in properties)
                {
                    var patternObject = matchTriplePattern.Object;
                    var variableName = patternObject.VariableName;
                    if (variableName == null) // null if it isn't a variable
                    {
                        //will be always the same value in the object, there shouldn't be a reason to index it - or is there?
                        //i.e. should be the type triples in the Construct Template
                        continue;
                    }

                    //object is a variable - bound or unbound, so could be an answer that will require indexing (unbound), or it's an object (bound)
                    //if that variable is in the triplePatternBySubjectParameter dictionary, than its bound
                    var property = ConstructPropertyFrom(variableName, false);

                    var patternPredicate = matchTriplePattern.Predicate;

                    thisObject.AddProperty(patternPredicate.ToString(), property);
                }

                return thisObject;
            }
        }

        public void CreatePropertyPathsForQuery(IParsedSparqlQuery parsedSparqlQuery)
        {
            var sparqlQuery = parsedSparqlQuery.Query;

            var startingParameter = parsedSparqlQuery.StartingParameterName.Substring(1); //String the ? as variable names provided by the parser don't contain it

            var constructVariables = sparqlQuery.ConstructTemplate.Variables;

            //todo: for hierarchi model ?
            // - hierarchical class that will handle this
            // --- might be used by the flatGraphNester
            // --- Detect usage of same variable in multiple branches
            // - enumarate ConstructTemplate.TripplePattern collection according to starting parameter and continue by scheduling further probes

            var constructTemplate = sparqlQuery.ConstructTemplate;

            var tripplePatternsBySubjectParameter = new Dictionary<string, List<IMatchTriplePattern>>();
            foreach (var triplePattern in constructTemplate.TriplePatterns)
            {
                switch (triplePattern.PatternType)
                {
                    case TriplePatternType.Match:
                        {
                            var typed = (IMatchTriplePattern)triplePattern;
                            var subject = typed.Subject;

                            var subjectVariableName = subject.VariableName;
                            if (subjectVariableName != null)
                            {
                                List<IMatchTriplePattern> list;
                                if (!tripplePatternsBySubjectParameter.TryGetValue(subjectVariableName, out list))
                                {
                                    list = new List<IMatchTriplePattern>();
                                    tripplePatternsBySubjectParameter.Add(subjectVariableName, list);
                                }
                                list.Add(typed);
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException("parsedSparqlQuery", "triple patterns in the Construct template should always start with a variable so that they form a hierarchy");
                            }

                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException("parsedSparqlQuery", String.Format("triple pattern type {0} is not supported in a ConstructTemplate", triplePattern.PatternType));
                }
            }

            var hierarchyBuilder = new HierarchyBuilder(tripplePatternsBySubjectParameter);
            var hierarchy = hierarchyBuilder.BuildHierarchyFrom(startingParameter);


        }

        private RavenJObject CreateContextForAbbreviations(Abbreviations abbreviations)
        {
            var context = new RavenJObject();

            var contextContent = new RavenJObject();
            context.Add("@context", contextContent);

            foreach (var uriToPrefixPair in abbreviations.UriToPrefixes)
            {
                contextContent.Add(uriToPrefixPair.Value, new RavenJValue(uriToPrefixPair.Key));
            }

            foreach (var prefixedFormToAbbreviationPair in abbreviations.PrefixFormToAbbreviation)
            {
                contextContent.Add(prefixedFormToAbbreviationPair.Value, new RavenJValue(prefixedFormToAbbreviationPair.Key));
            }


            return context;
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
