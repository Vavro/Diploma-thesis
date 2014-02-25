using System;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using Raven.Abstractions.Linq;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Client.Connection;
using Raven.Json.Linq;
using VDS.RDF.Query.Expressions.Primary;

namespace DragqnLD.Core.Implementations
{
    public class RavenDataStore : IDataStore
    {
        //todo: Inject session from current call - Raven session will be tied to one REST call?

        private readonly IDocumentStore _store;

        private readonly IDocumentPropertyEscaper _escaper;

        public RavenDataStore(IDocumentStore store)
        {
            _store = store;
        }
        public async Task StoreDocument(ConstructResult dataToStore)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var content = dataToStore.Document.Content;

                string id = GetDocumentId(dataToStore.QueryId, dataToStore.DocumentId.AbsoluteUri);

                //_store.AsyncDatabaseCommands.PutAsync(id, new Etag(UuidType.Documents, ), content, );

                await session.StoreAsync(content, id);

                //edit the entity name, so all indexed documents for the same query are together
                var metadata = session.Advanced.GetMetadataFor(content);
                metadata["Raven-Entity-Name"] = dataToStore.QueryId;
                await session.SaveChangesAsync();
            }
        }

        private static string GetDocumentId(string queryId, string documentUri)
        {
            return String.Format("{0}/{1}", queryId, documentUri);
        }


        public async Task<Document> GetDocument(string queryId, Uri documentId)
        {
            using (var session = _store.OpenAsyncSession())
            {
                string id = GetDocumentId(queryId, documentId.AbsoluteUri);
                var storedContent = await session.LoadAsync<RavenJObject>(id);
                var storedDocument = new Document() { Id = documentId.AbsoluteUri, Content = storedContent };

                return storedDocument;
            }
        }

        public async Task<IEnumerable<Uri>> QueryDocumentProperties(string queryId,
            params PropertyCondition[] conditions)
        {
            var luceneQuery = new StringBuilder();
            foreach (PropertyCondition propertyCondition in conditions)
            {
                var escapedPropertyName = propertyCondition.PropertyName.EscapePropertyName();
                
                //todo: add support for multiple values - @in<Property>:(value1, value2) 
                if (luceneQuery.Length > 0)
                {
                    luceneQuery.Append(" AND ");
                }
                luceneQuery.Append("(")
                    .Append(escapedPropertyName)
                    .Append(" : ")
                    .Append(propertyCondition.Value)
                    .Append(")");
            }
            return await QueryDocumentEscapedLuceneQuery(queryId, luceneQuery.ToString());
        }


        public async Task<IEnumerable<Uri>> QueryDocumentEscapedLuceneQuery(string queryId, string luceneQuery)
        {
            using (var session = _store.OpenAsyncSession())
            {
                var ravenLuceneQuery = session.Advanced.AsyncLuceneQuery<dynamic>()
                    .UsingDefaultOperator(QueryOperator.And)
                    .WhereEquals("@metadata.Raven-Entity-Name", queryId)
                    .Where(luceneQuery);

                //todo: paging - raven returns max 1024 documents or something like that
                //todo: returns whole documents with metadata.. probably not necessary
                var queryResults = await ravenLuceneQuery.QueryResultAsync;

                var ids = new List<string>(queryResults.Results.Count);
                foreach (var queryResult in queryResults.Results)
                {
                    var id = queryResult["@metadata"].Value<string>("@id").Substring(queryId.Length + 1);
                    ids.Add(id);
                }

                return ids.Select(id => new Uri(id));
            }
        }
    }

    public class PropertyCondition
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }

    public static class LuceneQueryStringEscape
    {
        public static string EscapePropertyName(this string propertyName)
        {
            string output;
            propertyName.ReplaceChars(SpecialCharacters.ProblematicCharacterSet, SpecialCharacters.EscapeChar,
                out output);
            return output;
        }

        public static PropertyCondition AsCondition(this string propertyName, string propertyValue)
        {
            return new PropertyCondition() {PropertyName = propertyName, Value = propertyValue};
        }
    }

}