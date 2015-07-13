using System;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Indexes;
using DragqnLD.WebApi.Configuration;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace DragqnLD.WebApi.App_Start
{
    /// <summary>
    /// Ravendb configuration
    /// </summary>
    public class RavenDbConfig
    {
        private static Lazy<IDocumentStore> _docStore = new Lazy<IDocumentStore>(() =>
        {
            var docStore = new DocumentStore
            {
                Url = DragqnLdConfig.Instance.DatabaseUrl,
                DefaultDatabase = DragqnLdConfig.Instance.DatabaseName
            };
            docStore.Initialize();

            IndexCreation.CreateIndexes(typeof(Documents_CountByCollection).Assembly, docStore);

            return docStore;
        });

        /// <summary>
        /// Gets the document store.
        /// </summary>
        /// <returns></returns>
        public static Lazy<IDocumentStore> GetDocumentStore()
        {
            return _docStore;
        }
    }
}