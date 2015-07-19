using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DragqnLD.WebApi.App_Start;
using Xunit;

namespace DragqnLD.UnitTests
{
    [Trait("Category", "Basic-Infrastructure")]
    public class MapperMappingOk
    {
        [Fact]
        public void MappingsAreWorking()
        {
            Assert.Throws<AutoMapperConfigurationException>(() =>
            {
                try
                {
                    AutoMapperConfig.ConfigureMapper();

                    Mapper.AssertConfigurationIsValid();
                }
                catch (AutoMapperConfigurationException ex)
                {
                    throw;
                }
            });
        }
    }
}
