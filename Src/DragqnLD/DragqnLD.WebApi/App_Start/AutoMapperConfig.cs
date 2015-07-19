using System.Linq;
using AutoMapper;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Abstraction.Indexes;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using DragqnLD.WebApi.Models;
using Raven.Abstractions.Indexing;

// ReSharper disable once CheckNamespace
namespace DragqnLD.WebApi.App_Start
{
    /// <summary>
    /// AutoMapper configuration
    /// </summary>
    public static class AutoMapperConfig
    {
        class DragqnLDProfile : Profile
        {
            protected override void Configure()
            {
                base.Configure();

                Mapper.CreateMap<QueryDefinition, QueryDefinitionMetadataDto>();

                Mapper.CreateMap<QueryDefinition, QueryDefinitionDto>();
                Mapper.CreateMap<QueryDefinitionDto, QueryDefinition>();

                Mapper.CreateMap<SparqlQueryInfo, SparqlQueryInfoDto>();
                Mapper.CreateMap<SparqlQueryInfoDto, SparqlQueryInfo>();

                //todo: temporary map probably before statistics in the db
                Mapper.CreateMap<QueryDefinition, QueryDefinitionWithStatusDto>();
                Mapper.CreateMap<Progress, ProgressDto>();
                Mapper.CreateMap<QueryDefinitionStatus, QueryDefinitionStatusDto>();

                Mapper.CreateMap<DocumentMetadata, DocumentMetadataDto>();
                Mapper.CreateMap<PagedDocumentMetadata, PagedDocumentMetadataDto>();

                Mapper.CreateMap<DragqnLDIndexDefinitions, IndexDefinitionsDto>();
                Mapper.CreateMap<DragqnLDIndexDefiniton, IndexDefinitionMetadataDto>()
                    .ConvertUsing(id => new IndexDefinitionMetadataDto()
                    {
                        Name = id.Name,
                        IndexedFields = id.Requirements.PropertiesToIndex.Select(prop => prop.PropertyPath).ToList()
                    });


            }
        }

        /// <summary>
        /// Configures the mapper.
        /// </summary>
        public static void ConfigureMapper()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<DragqnLDProfile>());
        }
    }
}