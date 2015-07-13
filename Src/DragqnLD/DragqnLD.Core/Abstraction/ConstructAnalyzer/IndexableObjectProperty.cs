using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private readonly List<NamedIndexableProperty> _childProperties = new List<NamedIndexableProperty>();

        public IReadOnlyList<NamedIndexableProperty> ChildProperties { get; private set; }

        public IndexableObjectProperty()
        {
            ChildProperties = new ReadOnlyCollection<NamedIndexableProperty>(_childProperties);
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

            _childProperties.Add(namedProperty);
        }

        public IIndexableProperty GetPropertyByFullName(string fullName)
        {
            return _childPropertiesByFullName[fullName];
        }

        public IIndexableProperty GetPropertyByAbbreviatedName(string abbreviatedName)
        {
            return _childPropertiesByAbbreviatedName[abbreviatedName];
        }
    }
}
