﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using VDS.RDF.Query;

namespace DragqnLD.Core.Implementations
{
    public class SparqlEnpointClient : ISparqlEnpointClient
    {
        public async Task<IEnumerable<Uri>> QueryForUris(SparqlQueryInfo selectSparqlQuery)
        {
            var endpoint = new SparqlRemoteEndpoint(selectSparqlQuery.SparqlEnpoint, selectSparqlQuery.DefaultDataSet);
            //todo: run in taks and await
            return await Task.Run(() =>
                {
                    var results = endpoint.QueryWithResultSet(selectSparqlQuery.Query);

                    //todo: check only URIs are returned
                    return new[] { new Uri("test") };
                });
        }

        public async Task<Stream> GetContructResultFor(SparqlQueryInfo constructSparqlQuery, string parameterName, Uri objectUri)
        {
            //todo: substitute parameter name for object Uri
            return await Task.Run(() =>
                {
                    var endpoint = new SparqlRemoteEndpoint(constructSparqlQuery.SparqlEnpoint,
                    constructSparqlQuery.DefaultDataSet);

                    var result = endpoint.QueryRaw(constructSparqlQuery.Query, new[] { "application/ld+json" });
                    return result.GetResponseStream();
                });
        }
    }
}
