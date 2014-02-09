using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Implementations;
using Xunit;

namespace DragqnLD.Core.UnitTests
{
    public class ExpandedJsonLDDataFormatterTests
    {
        [Fact]
        void CanFormatSimpleDataTest()
        {
            string inputJson = @"ExpanderTestsData\json1-simple.json";
            string expectedOutputJson = @"ExpandertestsData\json1out.json";
            string id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            TestFormat(inputJson, expectedOutputJson, id);
        }

        private static void TestFormat(string inputJson, string expectedOutputJson, string id)
        {
            var formatter = new ExpandedJsonLDDataFormatter();
            var reader = new StreamReader(inputJson);
            var writer = new StringWriter();

            formatter.Format(reader, writer, id);
            var output = writer.ToString();

            var expectedOutputReader = new StreamReader(expectedOutputJson);
            var expectedOutput = expectedOutputReader.ReadToEnd();

            Assert.Equal(output, expectedOutput);
        }
        
        [Fact]
        void CanFormatComplexDataTest()
        {
            string inputJson = @"ExpanderTestsData\json2-whole.json";
            string expectedOutputJson = @"ExpandertestsData\json2out.json";
            var id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            TestFormat(inputJson, expectedOutputJson, id);
        }

        [Fact]
        void CanFormatHeavilyNestedJSON()
        {
            string inputJson = @"ExpanderTestsData\json3-nested.json";
            string expectedOutputJson = @"ExpanderTestsData\json3out.json";
            var id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            TestFormat(inputJson, expectedOutputJson, id);
        }

        [Fact]
        void WillThrowOnNestedData()
        {
            string inputJson = @"ExpanderTestsData\json4-recursive.json";
            var id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            //todo: add more specific exception
            var ex = Assert.Throws<NotSupportedException>(() => { TestFormat(inputJson, null, id); });
            Console.WriteLine(ex.ToString());
        }
    }
}
