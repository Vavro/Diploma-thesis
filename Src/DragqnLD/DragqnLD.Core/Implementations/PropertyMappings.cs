using System;
using System.Collections.Generic;
using DragqnLD.Core.Abstraction.Query;

namespace DragqnLD.Core.Implementations
{
    public class PropertyMappings
    {
        private readonly Dictionary<string,string> _mappings = new Dictionary<string, string>();

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
            foreach (var mapping in _mappings)
            {
                list.Add(new PropertyEscape(){From = mapping.Key, To = mapping.Value});
            }
            return list;
        }
    }

}