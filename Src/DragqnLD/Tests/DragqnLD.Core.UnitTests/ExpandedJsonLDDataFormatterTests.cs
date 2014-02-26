using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Implementations;
using Xunit;
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

        [Theory]
        [InlineData(TestDataFolders.Ingredients, @"Output\Ingedients\", @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/")]
        [InlineData(TestDataFolders.MedicinalProducts, @"Output\MedicinalProducts\", @"http://linked.opendata.cz/resource/sukl/medicinal-product/")]
        void CanConvertAllTestData(string inputFolder, string outputFolder, string idprefix)
        {
            var inputDirectoryInfo = new DirectoryInfo(inputFolder);
            var inputFiles = inputDirectoryInfo.GetFiles();
            
            Directory.CreateDirectory(outputFolder);

            foreach (var inputFile in inputFiles)
            {
                var inputFileName = inputFile.FullName;
                var idWithoutNamespace = inputFile.Name.Substring(0, inputFile.Name.Length - ".json".Length);
                var id = idprefix + idWithoutNamespace;

                var outputFileName = outputFolder + idWithoutNamespace + ".out.json";
                
                using( var o = new FileStream(outputFileName, FileMode.Create))
                using(var outputFile = new StreamWriter(o))
                { 
                
                    GetFormatted(inputFileName, id, outputFile);

                    outputFile.Flush();
                    outputFile.Close();
                }
            }

        }


    }
}
