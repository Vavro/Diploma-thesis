using AutoMapper;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.WebApi.Models;

// ReSharper disable once CheckNamespace
namespace DragqnLD.WebApi.App_Start
{
    public static class AutoMapperConfig
    {
        class DragqnLDProfile : Profile
        {
            protected override void Configure()
            {
                base.Configure();

                Mapper.CreateMap<QueryDefinition, QueryDefinitionMetadataDto>();
                Mapper.CreateMap<QueryDefinition, QueryDefinitionDto>();
                Mapper.CreateMap<SparqlQueryInfo, SparqlQueryInfoDto>();
            }
        }

        public static void ConfigureMapper()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<DragqnLDProfile>());
        }
    }
}