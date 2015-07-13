using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace DragqnLD.Core.Abstraction.ConstructAnalyzer
{
    public class NamedIndexableProperty
    {

        public string AbbreviatedName { get; set; }

        public string FullName { get; set; }


        public IIndexableProperty Property { get; set; }
    }

    public class IndexableObjectProperty : IIndexableProperty
    {
        private readonly Dictionary<string, IIndexableProperty> _childPropertiesByFullName = new Dictionary<string, IIndexableProperty>();
        private readonly Dictionary<string, IIndexableProperty> _childPropertiesByAbbreviatedName = new Dictionary<string, IIndexableProperty>();

        public List<NamedIndexableProperty> ChildProperties { get; private set; }
        public bool? HasId { get; set; }
        public bool? HasType { get; set; }

        public IndexableObjectProperty()
        {
            ChildProperties = new List<NamedIndexableProperty>();
        }

        public void AddProperty(string abbreviatedName, string fullUriName, IIndexableProperty property)
        {
            var namedProperty = new NamedIndexableProperty()
            {
                AbbreviatedName = abbreviatedName,
                FullName = fullUriName,
                Property = property
            };
            _childPropertiesByFullName.Add(fullUriName, property);
            _childPropertiesByAbbreviatedName.Add(abbreviatedName, property);

            ChildProperties.Add(namedProperty);
        }

        public IIndexableProperty GetPropertyByFullName(string fullName)
        {
            return _childPropertiesByFullName[fullName];
        }

        public IIndexableProperty GetPropertyByAbbreviatedName(string abbreviatedName)
        {
            return _childPropertiesByAbbreviatedName[abbreviatedName];
        }

        public void InitializeDictionaries()
        {
            if (_childPropertiesByAbbreviatedName.Count != 0 || _childPropertiesByFullName.Count != 0)
            {
                throw new Exception("InitializeDictionaries can be called only if the internal dictioanries havent been initializeed");
            }

            foreach (var namedIndexableProperty in ChildProperties)
            {
                _childPropertiesByAbbreviatedName.Add(namedIndexableProperty.AbbreviatedName, namedIndexableProperty.Property);
                _childPropertiesByFullName.Add(namedIndexableProperty.FullName, namedIndexableProperty.Property);
                //if the hierarchy is a graph, this will fail
                var propertyAsObject = namedIndexableProperty.Property as IndexableObjectProperty;
                if (propertyAsObject != null)
                {
                    propertyAsObject.InitializeDictionaries();
                }
            }
        }
    }
}
