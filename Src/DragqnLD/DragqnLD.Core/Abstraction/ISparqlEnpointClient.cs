using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Query;

namespace DragqnLD.Core.Abstraction
{
    public interface ISparqlEnpointClient
    {
        Task<IEnumerable<Uri>> QueryForUris(SparqlQueryInfo selectSparqlQuery);

        Task<Stream> GetContructResultFor(SparqlQueryInfo constructSparqlQuery, string parameterName, Uri objectUri);
    }
}
