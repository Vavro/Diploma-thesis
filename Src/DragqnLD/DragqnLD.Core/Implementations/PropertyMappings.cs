using System;
using System.Collections.Generic;

namespace DragqnLD.Core.Implementations
{
    public class PropertyMappings
    {
        private Dictionary<string,string> _mappings = new Dictionary<string, string>();

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
    }
}