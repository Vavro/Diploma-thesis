using System;
using System.IO;
using DragqnLD.Core.Implementations;
using DragqnLD.Core.UnitTests.Utils;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions;

namespace DragqnLD.Core.UnitTests
{
    //todo: batch import of documents
    //todo: logging
    //todo: handle special characters in lucene queries (i.e. collon in value)
    //todo: handle inserted document for reserved properties
        //possible solutions: 
            //wrap into another property
            //exchange the @ character on start of property
        //change queries accordingly   
    //todo: batch update of documents
    //todo: metrics of querying

    public class ExpandedJsonLDDataFormatterTests : TestsBase
    {
        public ExpandedJsonLDDataFormatterTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        void CanFormatSimpleDataTest()
        {
            const string inputJson = @"ExpanderTestsData\json1-simple.json";
            const string expectedOutputJson = @"ExpandertestsData\json1out.json";
            const string id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            TestFormat(inputJson, expectedOutputJson, id);
        }

        private static void TestFormat(string inputJsonFileName, string expectedOutputJsonFileName, string id)
        {
            var writer = new StringWriter();

            var output = GetFormatted(inputJsonFileName, id, writer);

            var expectedOutputReader = new StreamReader(expectedOutputJsonFileName);
            var expectedOutput = expectedOutputReader.ReadToEnd();

            Assert.Equal(output, expectedOutput);
        }

        private static string GetFormatted(string inputJsonFileName, string id, TextWriter writer)
        {
            var formatter = new ExpandedJsonLDDataFormatter();
            var reader = new StreamReader(inputJsonFileName);

            PropertyMappings mappings;
            formatter.Format(reader, writer, id, out mappings);

            var output = writer.ToString();
            return output;
        }

        [Fact]
        void CanFormatComplexDataTest()
        {
            const string inputJson = @"ExpanderTestsData\json2-whole.json";
            const string expectedOutputJson = @"ExpandertestsData\json2out.json";
            const string id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            TestFormat(inputJson, expectedOutputJson, id);
        }

        [Fact]
        void CanFormatHeavilyNestedJson()
        {
            const string inputJson = @"ExpanderTestsData\json3-nested.json";
            const string expectedOutputJson = @"ExpanderTestsData\json3out.json";
            const string id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            TestFormat(inputJson, expectedOutputJson, id);
        }

        [Fact]
        void WillThrowOnNestedData()
        {
            const string inputJson = @"ExpanderTestsData\json4-recursive.json";
            const string id = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

            //todo: add more specific exception
            var ex = Assert.Throws<NotSupportedException>(() => TestFormat(inputJson, null, id));
            Output.WriteLine(ex.ToString());
        }

        [Theory]
        [InlineData(TestDataConstants.IngredientsFolder, @"Output\Ingredients\", @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/")]
        [InlineData(TestDataConstants.MedicinalProductsFolder, @"Output\MedicinalProducts\", @"http://linked.opendata.cz/resource/sukl/medicinal-product/")]
        void CanConvertAllTestData(string inputFolder, string outputFolder, string idPrefix)
        {
            var inputDirectoryInfo = new DirectoryInfo(inputFolder);
            var inputFiles = inputDirectoryInfo.GetFiles();
            
            Directory.CreateDirectory(outputFolder);

            foreach (var inputFile in inputFiles)
            {
                var inputFileName = inputFile.FullName;
                var idWithoutNamespace = inputFile.Name.Substring(0, inputFile.Name.Length - ".json".Length);
                var id = idPrefix + idWithoutNamespace;

                var outputFileName = outputFolder + idWithoutNamespace + ".out.json";

                FileStream o = null;
                try
                {
                    o = new FileStream(outputFileName, FileMode.Create);
                    using (var outputFile = new StreamWriter(o))
                    {
                        o = null;

                        GetFormatted(inputFileName, id, outputFile);

                        outputFile.Flush();
                    }
                }
                finally
                {
                    if (o != null)
                    {
                        o.Dispose();
                    }
                }
            }

        }


    }
}
