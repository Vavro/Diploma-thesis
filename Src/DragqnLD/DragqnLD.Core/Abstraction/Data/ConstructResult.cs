using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragqnLD.Core.Abstraction.Data
{
    public class ConstructResult
    {
        //todo: query id as URI
        public string QueryId { get; set; }
        public dynamic Document { get; set; }
        public Uri DocumentId { get; set; }
    }
}
