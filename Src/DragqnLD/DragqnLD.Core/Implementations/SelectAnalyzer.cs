using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using Raven.Abstractions.Data;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Expressions.Functions.Sparql.Boolean;
using VDS.RDF.Query.Patterns;

namespace DragqnLD.Core.Implementations
{
    public class SelectAnalyzer : ISelectAnalyzer
    {
        public string ConvertSparqlToLuceneNoIndex(string sparql, ConstructQueryAccessibleProperties hierarchy)
        {
            var parser = new SparqlQueryParser();
            var parsedSparql = parser.ParseFromString(sparql);

            if (parsedSparql.Variables.Count() > 1)
            {
                throw new NotSupportedException("only one variable, that will be bound to the resulting ids is supported");
            }

            var rootVariable = parsedSparql.Variables.Single();

            //find all property paths that are getting queried
            //  enumerate triples start from variable and traverse down all possible paths
            //  end of a path is designated by a literal, or by finding the variable in the FILTER clause

            //consult hierarchy for abbrevieted names and array wrapping 
            //  to construct correct lucene title name for each identified path, dont forget possible "_value" at the end
            //if path points to a objectProp -> add _id
            if (parsedSparql.RootGraphPattern.HasChildGraphPatterns)
                throw  new NotSupportedException("Child graph patterns are not supported");

            var tripplePatternsBySubjectParameter = SortPatternsBySubjectParamName(parsedSparql);

            var propertyPathsBuidler = new SelectPropertyPathsBuilder(tripplePatternsBySubjectParameter, hierarchy);
            var accessedPropertyPaths = propertyPathsBuidler.CreateAccessedPropertyPaths(rootVariable.Name);

            throw new NotImplementedException();
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
                    //find predicate in current property
                    var predicateTarget = currentProperty.GetPropertyByFullName(predicate.ToString());
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
                        string abbreviatedPredicateWithAccess = abbreviatedPredicate;
                        if (predicateTargetIsObject)
                        {
                            //the bound value is an ID
                            abbreviatedPredicateWithAccess = abbreviatedPredicate + ".@id";
                        }
                        //if the is a value, the access is added in the index creation

                        var identifiedPath = new PathWithValue()
                        {
                            Path = abbreviatedPredicateWithAccess,
                            ExpectedValue = @object.ToString()
                        };
                        identifiedPathsWithExpectedValues.Add(identifiedPath);
                    }
                }
                return identifiedPathsWithExpectedValues;
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
