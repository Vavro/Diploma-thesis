using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragqnLD.Core.Abstraction.Indexes
{
    public class PropertyToIndex
    {
        public string PropertyPath { get; set; }
        public bool FulltextSearchable { get; set; }
    }

}
