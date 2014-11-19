using System;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.Indexes;
using DragqnLD.WebApi.Configuration;
using DragqnLD.WebApi.Controllers;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Unity.WebApi;

namespace DragqnLD.WebApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            
            //todo: solve how to register the IDocumentStore without having to initialize it immediately (although that probably doesn't matter)
            container.RegisterInstance(LazyDocStore.Value, new ContainerControlledLifetimeManager());
            container.RegisterInstance(PerQueryDefinitionTasksManager.Instance);
            //these have to exist per request (per controller creation)
            container.RegisterType<IQueryStore, QueryStore>(new HierarchicalLifetimeManager());
            container.RegisterType<IDataStore, RavenDataStore>(new HierarchicalLifetimeManager());
            container.RegisterType<ISparqlEnpointClient, SparqlEnpointClient>(new HierarchicalLifetimeManager());
            container.RegisterType<IDataLoader, DataLoader>(new HierarchicalLifetimeManager());
            
            //todo: this is a global formatter now, maybe it will be needed to be determined somewhat dynamically - so create a FormatterFactory or FormatterProvider and provide that instead?
            container.RegisterType<IDataFormatter, ExpandedJsonLDDataFormatter>(new ContainerControlledLifetimeManager()); //can be singleton, doesn't work with instance variables
            container.RegisterType<IDocumentPropertyEscaper, DocumentPropertyEscaper>(new ContainerControlledLifetimeManager());
            
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }

        private static readonly Lazy<IDocumentStore> LazyDocStore = new Lazy<IDocumentStore>(() =>
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
    }
}