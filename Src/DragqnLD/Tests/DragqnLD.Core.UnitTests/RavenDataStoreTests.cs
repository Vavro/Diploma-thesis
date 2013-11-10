﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Tests.Helpers;
using Xunit;

namespace DragqnLD.Core.UnitTests
{
    public class RavenDataStoreTests
    {
        private readonly IDataStore _ravenDataStore;
        private readonly EmbeddableDocumentStore documentStore;

        private const int RavenWebUiPort = 8081;

        public RavenDataStoreTests()
        {
            var docStore = new EmbeddableDocumentStore()
            {
                RunInMemory = true,
                Configuration = { Port = RavenWebUiPort }
            };
            Raven.Database.Server.NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(RavenWebUiPort);

            docStore.Initialize();

            documentStore = docStore;
             _ravenDataStore = new RavenDataStore(docStore);
        }

        [Fact]
        public async Task CanStoreAndGetPlainJSONData()
        {
            var content = "{ \"name\" : \"Petr\"}";

            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = new Document() { Content = content }
            };

            await _ravenDataStore.StoreDocument(dataToStore);

            RavenTestBase.WaitForUserToContinueTheTest(documentStore);
        }
    }
}
