using AutoMapper;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.WebApi.Models;

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