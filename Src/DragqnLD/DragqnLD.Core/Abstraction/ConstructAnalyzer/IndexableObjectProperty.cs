using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragqnLD.Core.Abstraction.ConstructAnalyzer
{
    public class IndexableObjectProperty : IIndexableProperty
    {
        private readonly Dictionary<string, IIndexableProperty> _childPropertiesByFullName = new Dictionary<string, IIndexableProperty>();
        private readonly Dictionary<string, IIndexableProperty> _childPropertiesByAbbreviatedName = new Dictionary<string, IIndexableProperty>();


        public void AddProperty(string abbreviatedName, string fullUriName, IIndexableProperty property)
        {
            _childPropertiesByFullName.Add(fullUriName, property);
            _childPropertiesByAbbreviatedName.Add(abbreviatedName, property);
        }
    }
}
