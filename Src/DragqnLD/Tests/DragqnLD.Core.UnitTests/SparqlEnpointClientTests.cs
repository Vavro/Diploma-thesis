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

        /// <summary>
        /// Unfortunately not really reliable, as it depends on the state of the remote endpoint and its data
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CanGetConstructResult()
        {
            var expectedResult = "";
            
            //todo: substitution of parameter in implementation
            var constructQuery = new SparqlQueryInfo()
            {
                //parameters that can be substituted have to be marked as @ not just ?
                Query = @"DESCRIBE @u",
                DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                SparqlEnpoint = new Uri(@"http://linked.opendata.cz/sparql")
            };

            var stream = await _sparqlEnpointClient.GetContructResultFor(constructQuery, "u", new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"));

            var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync();

            Console.WriteLine(result);
            //todo: maybe a better check?
            Assert.True(!String.IsNullOrWhiteSpace(result));
        }
    }
}
