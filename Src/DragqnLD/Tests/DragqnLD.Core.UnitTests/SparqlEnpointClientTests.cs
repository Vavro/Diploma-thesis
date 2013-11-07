using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using Xunit;

namespace DragqnLD.Core.UnitTests
{
    public class SparqlEnpointClientTests
    {
        private readonly ISparqlEnpointClient _sparqlEnpointClient;

        public SparqlEnpointClientTests()
        {
            _sparqlEnpointClient = new SparqlEnpointClient();
        }

        [Fact]
        public async Task CanGetConstructResult()
        {
            var expectedResult = "";


            //todo: substitution of parameter in implementation
            var constructQuery = new SparqlQueryInfo()
            {
                Query = @"DESCRIBE <http://linked.opendata.cz/resource/ATC/M01AE01>",
                DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                SparqlEnpoint = new Uri(@"http://linked.opendata.cz/sparql")
            };

            var stream = await _sparqlEnpointClient.GetContructResultFor(constructQuery, "u", new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"));

            var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync();

            Console.WriteLine(result);
            Assert.True(!String.IsNullOrWhiteSpace(result));
        }
    }
}
