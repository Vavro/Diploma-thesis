using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Abstraction.Query;
using Newtonsoft.Json.Serialization;
using Raven.Abstractions.Indexing;

namespace DragqnLD.Core.Implementations
{
    public enum DragqnLDIndexAnalyzer
    {
        None
    }

    public class FieldDefinition
    {
        public DragqnLDIndexAnalyzer Analyzer { get; set; }
        public string FieldAccessString { get; set; }
    }

    public class IndexDefinitionCreater : IIndexDefinitionCreater
    {
        public IndexDefinition CreateIndexDefinitionFor(QueryDefinition ingredientsQd, ConstructQueryAccessibleProperties propertyPaths, DragqnLDIndexDefinition propertiesToIndex)
        {
            var mapBuilder = new StringBuilder();
            mapBuilder.AppendLine("from doc in docs");
            mapBuilder.Append(@"where doc[""@metadata""][""Raven-Entity-Name""] == ");
            mapBuilder.Append('"' + ingredientsQd.Id + '"');
            mapBuilder.AppendLine();
            mapBuilder.AppendLine("select new { ");

            foreach (var propertyToIndex in propertiesToIndex.PropertiesToIndex)
            {
                var mapLine = CreateIndexedFieldNameMapLine(propertyPaths, propertyToIndex);
                mapBuilder.AppendLine(mapLine);
                mapBuilder.AppendLine();
            }
            mapBuilder.AppendLine(@"_metadata_Raven_Entity_Name = doc[""@metadata""][""Raven-Entity-Name""]}");

            var map = mapBuilder.ToString();
            var indexDefintion = new IndexDefinition() { Map = map };
            return indexDefintion;
        }
        private class UniqueVarProvider
        {
            private int _index = 0;
            private string _lastVar = null;

            public string GetNewVar()
            {
                var prefix = "x" + _index;
                _index++;
                _lastVar = prefix;
                return prefix;
            }

            public string CurrentVar()
            {
                return _lastVar;
            }
        }

        public string CreateIndexedFieldNameMapLine(ConstructQueryAccessibleProperties propertyPaths, PropertiesToIndex propertyToIndex)
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
                                break;
                            }
                        case "@type":
                            {
                                CheckCanIndexType(objectProp);
                                fieldNameBuilder.Append("_type");
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

                                if (i == 0)
                                {
                                    fieldAccessNoArrayBuilder.Append("doc");
                                }

                                fieldAccessNoArrayBuilder.Append(".");
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

                var valueProp = (IndexableValueProperty)currentProperty;
                if (i != pathNames.Length - 1) //has to be the end of the path
                {
                    throw new ArgumentException(String.Format("Path is pointing to a non existing property, finished at {0}, whole path {1}", searchedPropName, propertyToIndex.AbbreviatedName));
                }
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

            return fieldNameBuilder + " = " + fieldAccessBuilder;
        }

        private void CheckCanIndexType(IndexableObjectProperty property)
        {
            if (!(property.HasId.HasValue
                  && (bool)property.HasId))
            {
                throw new ArgumentException("Cannot index Id");
            }
        }

        private void CheckCanIndexId(IndexableObjectProperty property)
        {
            if (!(property.HasId.HasValue
                  && (bool)property.HasId))
            {
                throw new ArgumentException("Cannot index type");
            }
        }
    }

}
