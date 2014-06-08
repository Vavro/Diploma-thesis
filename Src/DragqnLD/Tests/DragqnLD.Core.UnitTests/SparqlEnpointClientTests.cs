using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        public async Task CanRunSelectQuery()
        {
            var selectQuery = new SparqlQueryInfo()
            {
                Query = @"SELECT DISTINCT ?s WHERE { ?s ?p ?o } LIMIT 100",
                DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
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
            var constructQuery = new SparqlQueryInfo()
            {
                //parameters that can be substituted have to be marked as @ not just ?
                Query = @"DESCRIBE @u",
                DefaultDataSet = new Uri(@"http://linked.opendata.cz/resource/dataset/ATC"),
                SparqlEndpoint = new Uri(@"http://linked.opendata.cz/sparql")
            };

            var stream = await _sparqlEnpointClient.GetContructResultFor(constructQuery, "u", new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"));

            var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync();

            Console.WriteLine(result);
            Assert.True(!String.IsNullOrWhiteSpace(result));

            //from downloaded data on 7.11.2013 23:37 - not really reliable..
            var expectedHash = "0C-FF-1D-9F-7D-3E-37-48-83-BB-C3-7F-B5-A6-42-5F";
            
            using (var md5 = MD5.Create())
            {
                var resultForHash = GetBytes(result);
                var downloadedFileHash = md5.ComputeHash(resultForHash);
                var hashString = BitConverter.ToString(downloadedFileHash);
                Console.WriteLine(hashString);
                Assert.Equal(hashString, expectedHash);
            }
        }
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
