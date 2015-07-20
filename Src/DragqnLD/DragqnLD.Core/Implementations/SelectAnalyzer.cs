using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Indexes;
using Raven.Abstractions.Data;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Algebra;
using VDS.RDF.Query.Expressions.Functions.Sparql.Boolean;
using VDS.RDF.Query.Patterns;
using VDS.RDF.Update.Commands;

namespace DragqnLD.Core.Implementations
{
    public class SelectAnalyzer : ISelectAnalyzer
    {
        public string ConvertSparqlToLuceneNoIndex(string sparql, ConstructQueryAccessibleProperties hierarchy)
        {
            var parsedSparql = ParseAndCheck(sparql);

            var rootVariable = parsedSparql.Variables.Single(var => var.IsResultVariable);

            //find all property paths that are getting queried
            //  enumerate triples start from variable and traverse down all possible paths
            //  end of a path is designated by a literal, or by finding the variable in the FILTER clause

            var accessedPropertyPaths = CreateAccessedPropertyPaths(hierarchy, parsedSparql, rootVariable);

            CheckAccessedPropertyPaths(accessedPropertyPaths);

            //consult hierarchy for abbrevieted names and array wrapping 
            //  to construct correct lucene title name for each identified path, dont forget possible "_value" at the end
            //if path points to a objectProp -> add _id
            var luceneQuery = CreateLuceneQueryNoIndexFor(accessedPropertyPaths,hierarchy);

            return luceneQuery;
        }

        private static void CheckAccessedPropertyPaths(List<SelectPropertyPathsBuilder.PathWithValue> accessedPropertyPaths)
        {
//for now filters arent supported
            if (!accessedPropertyPaths.TrueForAll(path => path.VariableName == null))
            {
                throw new NotSupportedException("Filter expressions arent supported");
            }
        }

        private static List<SelectPropertyPathsBuilder.PathWithValue> CreateAccessedPropertyPaths(ConstructQueryAccessibleProperties hierarchy, SparqlQuery parsedSparql,
            SparqlVariable rootVariable)
        {
            var tripplePatternsBySubjectParameter = SortPatternsBySubjectParamName(parsedSparql);

            var propertyPathsBuidler = new SelectPropertyPathsBuilder(tripplePatternsBySubjectParameter, hierarchy);
            var accessedPropertyPaths = propertyPathsBuidler.CreateAccessedPropertyPaths(rootVariable.Name);
            return accessedPropertyPaths;
        }

        private static SparqlQuery ParseAndCheck(string sparql)
        {
            var parser = new SparqlQueryParser();
            var parsedSparql = parser.ParseFromString(sparql);

            if (parsedSparql.Variables.Where(var => var.IsResultVariable).Count() > 1)
            {
                throw new NotSupportedException("only one variable, that will be bound to the resulting ids is supported");
            }

            if (parsedSparql.RootGraphPattern.HasChildGraphPatterns)
                throw new NotSupportedException("Child graph patterns are not supported");

            return parsedSparql;
        }

        public DragqnLDIndexRequirements CreateIndexRequirementsFromSparql(string sparql, ConstructQueryAccessibleProperties hierarchy)
        {
            var parsedSparql = ParseAndCheck(sparql);

            var rootVariable = parsedSparql.Variables.Single(var => var.IsResultVariable);

            var accessedPropertyPaths = CreateAccessedPropertyPaths(hierarchy, parsedSparql, rootVariable);

            CheckAccessedPropertyPaths(accessedPropertyPaths);

            var requirements = new DragqnLDIndexRequirements()
            {
                PropertyPaths =
                    accessedPropertyPaths.Select(
                        path => new PropertyToIndex() {FulltextSearchable = false, PropertyPath = path.Path}).ToList()
            };
            return requirements;
        }

        public string ConvertSparqlToLuceneWithIndex(string sparql, ConstructQueryAccessibleProperties hierarchy,
            DragqnLDIndexDefiniton indexDefinition)
        {
            var parsedSparql = ParseAndCheck(sparql);

            var rootVariable = parsedSparql.Variables.Single(var => var.IsResultVariable);

            var accessedPropertyPaths = CreateAccessedPropertyPaths(hierarchy, parsedSparql, rootVariable);

            CheckAccessedPropertyPaths(accessedPropertyPaths);

            //consult the index definition for the required paths
            var luceneQuery = CreateLuceneQueryWithIndexFor(accessedPropertyPaths, indexDefinition);

            return luceneQuery;
        }

        private string CreateLuceneQueryWithIndexFor(List<SelectPropertyPathsBuilder.PathWithValue> accessedPropertyPaths, DragqnLDIndexDefiniton indexDefinition)
        {
            foreach (var accessedPropertyPath in accessedPropertyPaths)
            {
                if (!indexDefinition.PropertyNameMap.ContainsKey(accessedPropertyPath.Path))
                {
                    throw new ArgumentOutOfRangeException(
                        String.Format("property path {0} is not contained in the index {1}", 
                        accessedPropertyPath.Path, 
                        indexDefinition.Name));
                }
            }

            var convertedPaths = accessedPropertyPaths.Select(
                path => new {Value = path.ExpectedValue, luceneField = indexDefinition.PropertyNameMap[path.Path]});

            var luceneFieldQueries = convertedPaths.Select(path => "+" + path.luceneField + ": (" + path.Value + ")");
            var luceneQuery = luceneFieldQueries.Aggregate((str1, str2) => str1 + " " + str2);

            return luceneQuery;
        }

        public string CreateLuceneQueryNoIndexFor(List<SelectPropertyPathsBuilder.PathWithValue> accessedPropertyPaths, ConstructQueryAccessibleProperties hierarchy)
        {
            var luceneQuery = new StringBuilder();
            if (!accessedPropertyPaths.Any())
            {
                throw new ArgumentException("We have to query at least one property", "accessedPropertyPaths");
            }

            foreach (var accessedPropertyPath in accessedPropertyPaths)
            {
                if (luceneQuery.Length != 0)
                {
                    luceneQuery.Append(" ");
                }
                var luceneFieldQuery = CreateLuceneFieldQueryNoIndexForPath(accessedPropertyPath, hierarchy);
                luceneQuery.Append("+").Append(luceneFieldQuery); //for now all selects equal to a big AND lucene query
            }

            return luceneQuery.ToString();
        }

        private string CreateLuceneFieldQueryNoIndexForPath(SelectPropertyPathsBuilder.PathWithValue accessedPropertyPath, ConstructQueryAccessibleProperties hierarchy)
        {
            var fieldName = new StringBuilder();
            var currentProp = hierarchy.RootProperty;
            NamedIndexableProperty lastFoundNamedProp = null;
            //this is an abbrevieted path
            var pathNames = accessedPropertyPath.Path.Split('.');
            for (int i = 0; i < pathNames.Length; i++)
            {
                var s = pathNames[i];
                if (s == "@id")
                {
                    fieldName.Append("_id");
                    break;  
                }

                NamedIndexableProperty namedProp;
                try
                {
                    namedProp = currentProp.GetPropertyByAbbreviatedName(s);
                }
                catch (KeyNotFoundException exception)
                {
                    throw new ArgumentException(String.Format("Field: {0} in path {1} is not part of the hierarchy", s, accessedPropertyPath.Path), exception);
                }
                fieldName.Append(s);
                if (i < pathNames.Length - 1)
                {
                    if (namedProp.WrappedInArray.HasValue && (bool)namedProp.WrappedInArray)
                    {
                        fieldName.Append(",");
                    }
                    else
                    {
                        fieldName.Append(".");
                    }
                }
                lastFoundNamedProp = namedProp;
                var asObject = namedProp.Property as IndexableObjectProperty;
                if (asObject != null)
                {
                    currentProp = asObject;
                    continue;
                }
                break;
            }
            
            if (lastFoundNamedProp != null && lastFoundNamedProp.Property is IndexableValueProperty) //might have to append _value prop, otherwise we should be done
            {
                var asValue = (IndexableValueProperty) lastFoundNamedProp.Property;
                if (asValue.Type == ValuePropertyType.LanguageString)
                {
                    if (lastFoundNamedProp.WrappedInArray.HasValue && (bool) lastFoundNamedProp.WrappedInArray)
                    {
                        fieldName.Append(",");
                    }
                    else
                    {
                        fieldName.Append(".");
                    }
                    fieldName.Append("_value");
                }
            }

            //for now dont have to care for filters
            return fieldName + ": (" + accessedPropertyPath.ExpectedValue + ")";
        }

        public class SelectPropertyPathsBuilder
        {
            private readonly Dictionary<string, List<IMatchTriplePattern>> _tripplePatternsBySubjectParameter;
            private readonly ConstructQueryAccessibleProperties _hierarchy;

            public SelectPropertyPathsBuilder(Dictionary<string, List<IMatchTriplePattern>> tripplePatternsBySubjectParameter, ConstructQueryAccessibleProperties hierarchy)
            {
                _tripplePatternsBySubjectParameter = tripplePatternsBySubjectParameter;
                _hierarchy = hierarchy;
            }

            //for storing variables that are left unbound and have to be found in the filter
            private readonly List<Tuple<string,string>> _unboundVariables = new List<Tuple<string, string>>();

            public List<PathWithValue> CreateAccessedPropertyPaths(string variableName)
            {
                _unboundVariables.Clear();
                if (!_tripplePatternsBySubjectParameter.ContainsKey(variableName))
                {
                    throw new NotSupportedException("the root variable name has to be one of the subjects in the patterns");
                }

                //first iteration looks for properties in the root property
                var currentProperty = _hierarchy.RootProperty;

                var identifiedPathsWithExpectedValues = CreateAccessedPropertyPaths(variableName, currentProperty);
               
                return identifiedPathsWithExpectedValues;
            }

            /// <summary>
            /// Describes a property path with an expected value or variable if that has to be filtered
            /// </summary>
            public class PathWithValue
            {
                /// <summary>
                /// Gets or sets the path.
                /// </summary>
                /// <value>
                /// The path.
                /// </value>
                public string Path { get; set; }

                /// <summary>
                /// Gets or sets the name of the variable. Returns the variable name only if it is a variable (and will have to be filtered)
                /// </summary>
                /// <value>
                /// The name of the variable.
                /// </value>
                public string VariableName { get; set; }

                /// <summary>
                /// Gets or sets the expected value. Returns null if it isn't known
                /// </summary>
                /// <value>
                /// The expected value.
                /// </value>
                public string ExpectedValue { get; set; }
            }

            private List<PathWithValue> CreateAccessedPropertyPaths(string variableName, IndexableObjectProperty currentProperty)
            {
                var patternsFromVariable = _tripplePatternsBySubjectParameter[variableName];
                var identifiedPathsWithExpectedValues = new List<PathWithValue>();
                foreach (var matchTriplePattern in patternsFromVariable)
                {
                    var predicate = matchTriplePattern.Predicate;
                    if (predicate.VariableName != null)
                    {
                        throw new NotSupportedException(
                            String.Format("Variable predicates aren't supported {0}",
                            matchTriplePattern));
                    }
                    var asNodeMatch = predicate as NodeMatchPattern;
                    Debug.Assert(asNodeMatch != null, "predicate is not a NodeMatchPattern");
                    var uriNode = asNodeMatch.Node as UriNode;
                    //find predicate in current property
                    Debug.Assert(uriNode != null, "predicetes node is not a UriNode");
                    var predicateTarget = currentProperty.GetPropertyByFullName(uriNode.Uri.AbsoluteUri);
                    var abbreviatedPredicate = predicateTarget.AbbreviatedName;

                    var predicateTargetIsObject = predicateTarget.Property is IndexableObjectProperty;
                    var @object = matchTriplePattern.Object;
                    if (@object.VariableName != null)
                    {
                        //either more paths will get spawned or its bound in the filter list
                        //filter on ids not supported
                        if (predicateTargetIsObject)
                        {
                            //FILTER on object id is not supported? - can be only filtered using str() conversion to regex - not supported
                            if (!_tripplePatternsBySubjectParameter.ContainsKey(@object.VariableName))
                            {
                                throw new NotSupportedException(
                                    String.Format(
                                        "Predicate target is a variable name, but points to an object id, which can't be filtered triple {0}",
                                        matchTriplePattern));
                            }
                            var nestedPropertyAccesses = CreateAccessedPropertyPaths(@object.VariableName,
                                (IndexableObjectProperty)predicateTarget.Property);
                            //now prepend current predicate name and add to the identified paths;
                            nestedPropertyAccesses.ApplyIfNotNull(path =>
                            {
                                path.Path = abbreviatedPredicate + "." + path.Path;
                            });
                            identifiedPathsWithExpectedValues.AddRange(nestedPropertyAccesses);
                            //this could add , in case of wrapped in List
                        }
                        else //target is a value and has to be filtered
                        {
                            var identifiedPath = new PathWithValue()
                            {
                                Path = abbreviatedPredicate,
                                VariableName = @object.VariableName
                            };
                            identifiedPathsWithExpectedValues.Add(identifiedPath);
                        }
                    }
                    else //its bound to a value
                    {
                        var boundValue = @object.ToString();
                        string abbreviatedPredicateWithAccess = abbreviatedPredicate;
                        if (predicateTargetIsObject)
                        {
                            //the bound value is an ID
                            abbreviatedPredicateWithAccess = abbreviatedPredicate + ".@id";
                            //delete the <> from the value
                            boundValue = '"' + boundValue.Trim('<', '>') + '"';
                        }

                        if (!predicateTargetIsObject)
                        {
                            var asValue = (IndexableValueProperty) predicateTarget.Property;
                            //language tagged strings are not supported so try to 
                            if (asValue.Type == ValuePropertyType.LanguageString)
                            {
                                boundValue = DeleteLangTagIfPresent(boundValue);
                            }
                        }
                        //if the is a value, the access is added in the index creation

                        var identifiedPath = new PathWithValue()
                        {
                            Path = abbreviatedPredicateWithAccess,
                            ExpectedValue = boundValue
                        };
                        identifiedPathsWithExpectedValues.Add(identifiedPath);
                    }
                }
                return identifiedPathsWithExpectedValues;
            }

            private static string DeleteLangTagIfPresent(string boundValue)
            {
                var stringEnd = boundValue.LastIndexOf("\"");
                var langDelimiter = boundValue.LastIndexOf("@");
                if (langDelimiter > stringEnd)
                {
                    boundValue = boundValue.Substring(0, langDelimiter);
                }
                return boundValue;
            }
        }
        

        private static Dictionary<string, List<IMatchTriplePattern>> SortPatternsBySubjectParamName(SparqlQuery parsedSparql)
        {
            var tripplePatternsBySubjectParameter = new Dictionary<string, List<IMatchTriplePattern>>();
            foreach (var triplePattern in parsedSparql.RootGraphPattern.TriplePatterns)
            {
                switch (triplePattern.PatternType)
                {
                    case TriplePatternType.Match:
                    {
                        var typed = (IMatchTriplePattern) triplePattern;
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
                            throw new NotSupportedException(
                                "triple patterns in the select query should always start with a variable so that they form a hierarchy");
                        }

                        break;
                    }
                    default:
                        throw new NotSupportedException(
                            String.Format("triple pattern type {0} is not supported in select query analysis",
                                triplePattern.PatternType));
                }
            }
            return tripplePatternsBySubjectParameter;
        }
    }
}
