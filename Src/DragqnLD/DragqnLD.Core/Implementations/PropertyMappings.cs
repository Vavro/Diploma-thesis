using System;
using System.Collections.Generic;
using System.Linq;
using DragqnLD.Core.Abstraction.Query;

namespace DragqnLD.Core.Implementations
{
    public class PropertyMappings
    {
        private readonly Dictionary<string,string> _mappings = new Dictionary<string, string>();

        public PropertyMappings() { }

        public PropertyMappings(List<PropertyEscape> escapes)
        {
            foreach (var propertyEscape in escapes)
            {
                _mappings.Add(propertyEscape.OldName, propertyEscape.NewName);
            }
        }

        public void AddMapping(string oldPropertyName, string newPropertyName)
        {
            string containedMapping;
            var contained = _mappings.TryGetValue(oldPropertyName, out containedMapping);
            if (!contained)
            {
                _mappings.Add(oldPropertyName, newPropertyName);
            }
            else
            {
                //todo: invert this condition - the new names are more important to not overlap (and old ones will always have the same new one (since they are escaped the same way)
                if (containedMapping != newPropertyName)
                {
                    throw new NotSupportedException(String.Format("Can't add mapping {0} to {1}, because already mapped to {2}", oldPropertyName, newPropertyName, containedMapping));
                }
            }
        }

        public void Merge(PropertyMappings mappings)
        {
            foreach (var mergedMapping in mappings._mappings)
            {
                string currentValue;
                var succ = _mappings.TryGetValue(mergedMapping.Key, out currentValue);
                if (succ)
                {
                    if (currentValue != mergedMapping.Value)
                    {
                        throw new NotSupportedException(String.Format("Mapping for key {0} has value {1}, can't add value {2}", mergedMapping.Key, currentValue, mergedMapping.Value));
                    }
                }
                else
                {
                    _mappings.Add(mergedMapping.Key, mergedMapping.Value);
                }
            }
        }

        public List<PropertyEscape> AsList()
        {
            var list = new List<PropertyEscape>(_mappings.Count);
            list.AddRange(_mappings.Select(mapping => new PropertyEscape() {OldName = mapping.Key, NewName = mapping.Value}));
            return list;
        }
    }

}