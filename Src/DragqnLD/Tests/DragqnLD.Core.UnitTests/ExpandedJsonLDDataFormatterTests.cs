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
        private const string id1 = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";
        private const string id2 = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";

        [Fact]
        void SimpleFormatTest()
        {
            var formatter = new ExpandedJsonLDDataFormatter();
            var reader = new StreamReader(@"ExpanderTestsData\json1.json");
            var writer = new StringWriter();

            formatter.Format(reader, writer, id1);
            var output = writer.ToString();

            var expectedOutputReader = new StreamReader(@"ExpandertestsData\json1out.json");
            var expectedOutput = expectedOutputReader.ReadToEnd();

            Assert.Equal(output, expectedOutput);
        }


        [Fact]
        void ComplexFormatTest()
        {
            var formatter = new ExpandedJsonLDDataFormatter();
            var reader = new StreamReader(@"ExpanderTestsData\json2.json");
            var writer = new StringWriter();

            formatter.Format(reader, writer, id2);
            var output = writer.ToString();

            var expectedOutputReader = new StreamReader(@"ExpandertestsData\json2out.json");
            var expectedOutput = expectedOutputReader.ReadToEnd();

            Assert.Equal(output, expectedOutput);
        }
    }
}
