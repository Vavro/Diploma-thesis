﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using Raven.Client.Linq;
using VDS.RDF;
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
                    var sparqlResultSet = endpoint.QueryWithResultSet(selectSparqlQuery.Query);

                    string variableName = null;
                    try
                    {
                        variableName = sparqlResultSet.Variables.Single();
                    }
                    catch (Exception ex)
                    {
                        throw new NotSupportedException("Select query result doesn't contain exactly one variable", ex);
                    }

                    var resultUris = sparqlResultSet.Results.Select(result =>
                                        {
                                            var node = result[variableName];
                                            var uriNode = node as UriNode;
                                            if (uriNode == null)
                                            {
                                                throw new NotSupportedException(
                                                    String.Format("Node {0} is not of type UriNode and cannot be parsed to uri", node));
                                            }
                                            return uriNode.Uri;
                                        });

                    return resultUris;
                });
        }

        public async Task<Stream> GetContructResultFor(SparqlQueryInfo constructSparqlQuery, string parameterName, Uri objectUri)
        {
            //todo: make mime type of donwloaded data configurable? 
            return await Task.Run(() =>
            {
                var parametrizedQuery = new SparqlParameterizedString(constructSparqlQuery.Query);
                parametrizedQuery.SetUri(parameterName, objectUri);
                var substitutedQuery = parametrizedQuery.ToString();

                var endpoint = new SparqlRemoteEndpoint(constructSparqlQuery.SparqlEnpoint,
                constructSparqlQuery.DefaultDataSet);

                var result = endpoint.QueryRaw(substitutedQuery, new[] { "application/ld+json" });
                return result.GetResponseStream();
            });
        }
    }
}
