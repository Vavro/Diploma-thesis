using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.Data;
using VDS.RDF.Parsing.Handlers;

namespace DragqnLD.Core.Abstraction
{
    /// <summary>
    /// Data store interface
    /// </summary>
    public interface IDataStore
    {
        Task StoreDocument(ConstructResult dataToStore);
        Task<dynamic> GetDocument(string queryId, Uri documentId);

        Task<IEnumerable<Uri>> QueryDocumentProperty(string queryId, string luceneQuery);
    }

}
