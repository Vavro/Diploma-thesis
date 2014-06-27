using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.WebApi.Models;

namespace DragqnLD.WebApi.App_Start
{
    public class AutoMapperConfig
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

        public static void Configure()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<DragqnLDProfile>());
        }
    }
}