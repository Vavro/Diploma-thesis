using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Abstraction.Indexes;
using DragqnLD.Core.Abstraction.Query;
using Newtonsoft.Json.Serialization;
using Raven.Abstractions.Indexing;
using VDS.RDF.Query.Algebra;

namespace DragqnLD.Core.Implementations
{
    public enum DragqnLDIndexAnalyzer
    {
        None
    }

    public static class KnownRavenDBAnalyzers
    {
        public const string AnalyzerLuceneStandard = "Lucene.Net.Analysis.Standard.StandardAnalyzer";
    }

    public class FieldDefinition
    {
        public DragqnLDIndexAnalyzer Analyzer { get; set; }
        public string FieldAccessString { get; set; }
    }

    public class IndexDefinitionCreater : IIndexDefinitionCreater
    {


        public DragqnLDIndexDefiniton CreateIndexDefinitionFor(QueryDefinition queryDefinition, ConstructQueryAccessibleProperties propertyPaths, DragqnLDIndexRequirements requirements)
        {
            var mapBuilder = new StringBuilder();
            mapBuilder.AppendLine("from doc in docs");
            mapBuilder.Append(@"where doc[""@metadata""][""Raven-Entity-Name""] == ");
            mapBuilder.Append('"' + queryDefinition.Id + '"');
            mapBuilder.AppendLine();
            mapBuilder.AppendLine("select new { ");
            var createdPropNames = new List<string>();
            var createdPropNamesMap = new Dictionary<string, string>();

            var analyzers = new Dictionary<string, string>();

            foreach (var propertyToIndex in requirements.PropertiesToIndex)
            {
                var mapLine = CreateIndexedFieldNameAndAccess(propertyPaths, propertyToIndex);
                mapBuilder.Append(mapLine.Name + " = " + mapLine.Access);
                mapBuilder.AppendLine(",");
                if (propertyToIndex.Fulltext)
                {
                    analyzers.Add(mapLine.Name, KnownRavenDBAnalyzers.AnalyzerLuceneStandard);
                }
                createdPropNames.Add(mapLine.Name);
                createdPropNamesMap.Add(propertyToIndex.AbbreviatedName, mapLine.Name);
            }
            mapBuilder.Append(@"_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}");

            var map = mapBuilder.ToString();
            var ravenIndexDefinition = new IndexDefinition() { Map = map, Analyzers = analyzers};
            
            DragqnLDIndexDefiniton indexDefinition = new DragqnLDIndexDefiniton()
            {
                Name = CreateNameFor(queryDefinition, createdPropNames),
                Requirements = requirements,
                PropertyNameMap = createdPropNamesMap,
                RavenMap = ravenIndexDefinition.Map,
                RavenAnalyzers = ravenIndexDefinition.Analyzers
            };

            return indexDefinition;
        }

        private static string CreateNameFor(QueryDefinition ingredientsQd, IEnumerable<string> propNames)
        {
            const string delimiter = "_";
            var concatenatedReplacedPropNames = propNames.Aggregate((str1, str2) => str1 + delimiter + str2);
            
            return ingredientsQd.Id + "/" + concatenatedReplacedPropNames;
            
        }

        private class UniqueVarProvider
        {
            private int _index = 0;

            public string GetNewVar()
            {
                var prefix = "x" + _index;
                _index++;
                return prefix;
            }
        }

        public class FieldNameAndAccess
        {
            public FieldNameAndAccess(string name, string access)
            {
                Name = name;
                Access = access;
            }

            public string Name { get; set; }
            public string Access { get; set; }
        }

        public FieldNameAndAccess CreateIndexedFieldNameAndAccess(ConstructQueryAccessibleProperties propertyPaths, PropertyToIndex propertyToIndex)
        {
            var pathNames = propertyToIndex.AbbreviatedName.Split('.');
            IIndexableProperty currentProperty = propertyPaths.RootProperty;

            var fieldNameBuilder = new StringBuilder();
            var fieldAccessBuilder = new StringBuilder();
            var fieldAccessNoArrayBuilder = new StringBuilder();
            var variableProvider = new UniqueVarProvider();
            var trailingBrackets = 0;

            for (int i = 0; i < pathNames.Length; i++)
            {
                if (i == 0)
                {
                    fieldAccessNoArrayBuilder.Append("doc");
                }

                fieldAccessNoArrayBuilder.Append(".");

                var searchedPropName = pathNames[i];

                var objectProp = currentProperty as IndexableObjectProperty;
                if (objectProp != null)
                {
                    switch (searchedPropName)
                    {
                        case "@id":
                            {
                                CheckCanIndexId(objectProp);
                                fieldNameBuilder.Append("_id");
                                fieldAccessNoArrayBuilder.Append("_id");
                                break;
                            }
                        case "@type":
                            {
                                CheckCanIndexType(objectProp);
                                fieldNameBuilder.Append("_type");
                                fieldAccessNoArrayBuilder.Append("_type");
                                break;
                            }
                        default:
                            {
                                //will throw if not present
                                var nestedProperty = objectProp.GetPropertyByAbbreviatedName(searchedPropName);
                                
                                fieldNameBuilder.Append(searchedPropName);
                                if (i != pathNames.Length - 1) // if not last property add _ to bind array or prop access
                                {
                                    fieldNameBuilder.Append("_");
                                }
                                //while not an array object add only the name of the property delimited by "."
                                //when an array comes wrap name in ((IEnumerable<dynamic>FIELD_ACCESS).DefaultIfEmpty().Select(VAR => VAR.
                                var isWrappedInArray = nestedProperty.WrappedInArray.HasValue &&
                                                       (bool)nestedProperty.WrappedInArray;

                                fieldAccessNoArrayBuilder.Append(nestedProperty.AbbreviatedName);

                                if (isWrappedInArray)
                                {
                                    fieldAccessBuilder.Append("((IEnumerable<dynamic>)");
                                    fieldAccessBuilder.Append(fieldAccessNoArrayBuilder);
                                    fieldAccessBuilder.Append(").DefaultIfEmpty().Select(");
                                    trailingBrackets++;
                                    var newVariable = variableProvider.GetNewVar();
                                    fieldAccessBuilder.Append(newVariable);
                                    fieldAccessBuilder.Append(" => ");
                                    fieldAccessBuilder.AppendLine();
                                    fieldAccessNoArrayBuilder = new StringBuilder();
                                    fieldAccessNoArrayBuilder.Append(newVariable);
                                }
                                
                                currentProperty = nestedProperty.Property;
                                break;
                            }
                    }
                    continue;
                }

                if (i != pathNames.Length - 1) //has to be the end of the path
                {
                    throw new ArgumentException(String.Format("Path is pointing to a non existing property, finished at {0}, whole path {1}", searchedPropName, propertyToIndex.AbbreviatedName));
                }
                break;                
            }

            //at the end could be a value property of type languagaString we need to handle
            var valueProp = currentProperty as IndexableValueProperty;
            if (valueProp != null)
            {
                //name of this value property is already added, from traversing the above value, now only add _value access if needed - thats needed only on langTaggedString
                if (valueProp.Type == ValuePropertyType.LanguageString)
                {
                    fieldAccessNoArrayBuilder.Append("._value");
                }
            }

            fieldAccessBuilder.Append(fieldAccessNoArrayBuilder);

            for (int i = trailingBrackets; i > 0; i--)
            {
                fieldAccessBuilder.Append(")");
            }

            return new FieldNameAndAccess(fieldNameBuilder.ToString(), fieldAccessBuilder.ToString());
        }

        private void CheckCanIndexType(IndexableObjectProperty property)
        {
            if (!(property.HasId.HasValue
                  && (bool)property.HasId))
            {
                throw new ArgumentException("Cannot index Type");
            }
        }

        private void CheckCanIndexId(IndexableObjectProperty property)
        {
            if (!(property.HasId.HasValue
                  && (bool)property.HasId))
            {
                throw new ArgumentException("Cannot index Id");
            }
        }
    }

}
