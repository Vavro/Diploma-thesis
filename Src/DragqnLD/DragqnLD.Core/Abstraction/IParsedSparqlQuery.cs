using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Query;

namespace DragqnLD.Core.Abstraction
{

    public interface IParsedSparqlQuery
    {
        SparqlQuery Query { get; set; }
        string StartingParameterName { get; set; }
    }

}
