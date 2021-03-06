﻿using System;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Implementations;
using Raven.Client.Embedded;
using Raven.Database.Config;
using Raven.Database.Server;
using Raven.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{

    public class DataStoreFixture : RavenTestBase, IDisposable
    {
        public readonly IDataStore RavenDataStore;
        public readonly EmbeddableDocumentStore DocumentStore;
        
        protected const int RavenWebUiPort = 8081;

        protected override void ModifyConfiguration(InMemoryRavenConfiguration configuration)
        {
            configuration.Storage.Voron.AllowOn32Bits = true;
            
            base.ModifyConfiguration(configuration);
        }

        public DataStoreFixture()
        {
            //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(RavenWebUiPort);
            var docStore = NewDocumentStore(port: RavenWebUiPort);
            
            DocumentStore = docStore;
            RavenDataStore = new RavenDataStore(docStore, new DocumentPropertyEscaper(), new PropertyUnescapesCache());
        }

        public override void Dispose()
        {
            Dispose(true);
            
            base.Dispose();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                DocumentStore.Dispose();

                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            }
        }
    }
    public abstract class DataStoreTestsBase : TestsBase
    {
        protected readonly IDataStore RavenDataStore;
        protected readonly EmbeddableDocumentStore DocumentStore;
        protected const string JsonBernersLeeFileName = @"JSON\berners-lee.jsonld";

        protected const string JsonBersersLeeId = @"http://www.w3.org/People/Berners-Lee/card#i";

        private readonly DataStoreFixture _dataStoreFixture;

        protected DataStoreTestsBase(ITestOutputHelper output, DataStoreFixture dataStoreFixture) : base(output)
        {
            _dataStoreFixture = dataStoreFixture;
            DocumentStore = _dataStoreFixture.DocumentStore;
            RavenDataStore = _dataStoreFixture.RavenDataStore;
        }
    }
}