using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{
    public class SparqlEnpointClientTests : LocalVirtuosoServerTests
    {
        private readonly ISparqlEnpointClient _sparqlEnpointClient;

        public SparqlEnpointClientTests(ITestOutputHelper output) : base(output)
        {
            _sparqlEnpointClient = new SparqlEnpointClient();
        }

        [Fact]
        public async Task CanRunSelectQuery()
        {
            var selectQuery = new SparqlQueryInfo
            {
                Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o } LIMIT 100",
                DefaultDataSet = new Uri(LocalVirtuosoDefaultDataSet),
                SparqlEndpoint = new Uri(LocalVirtuosoSparqlEnpoint)
            };

            var result = await _sparqlEnpointClient.QueryForUris(selectQuery);

            Assert.NotNull(result);
            Assert.Equal(result.Count(), 100);
        }

        /// <summary>
        /// Unfortunately not really reliable, as it depends on the state of the remote endpoint and its data
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CanGetConstructResult()
        {
            //todo: substitution of parameter in implementation
            var constructQuery = new SparqlQueryInfo
            {
                //parameters that can be substituted have to be marked as @ not just ?
                Query = @"DESCRIBE @u",
                DefaultDataSet = new Uri(LocalVirtuosoDefaultDataSet),
                SparqlEndpoint = new Uri(LocalVirtuosoSparqlEnpoint)
            };

            var stream = await _sparqlEnpointClient.GetContructResultFor(constructQuery, "u", new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"));

            var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync();

            Output.WriteLine(result);
            Assert.True(!String.IsNullOrWhiteSpace(result));

            //from downloaded data on my server - not really reliable..
            const string expectedHash = "40-03-FC-71-98-75-13-E4-67-FB-D6-8C-00-F0-1D-07";
            
            using (var md5 = MD5.Create())
            {
                var resultForHash = GetBytes(result);
                var downloadedFileHash = md5.ComputeHash(resultForHash);
                var hashString = BitConverter.ToString(downloadedFileHash);
                Output.WriteLine(hashString);
                Assert.Equal(expectedHash, hashString);
            }
        }
        static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }

    public abstract class LocalVirtuosoServerTests : TestsBase
    {
        protected static readonly string LocalVirtuosoDefaultDataSet = @"http://localhost:8890/DAV";
        protected static readonly string LocalVirtuosoSparqlEnpoint = @"http://localhost:8890/sparql";

        protected LocalVirtuosoServerTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}
