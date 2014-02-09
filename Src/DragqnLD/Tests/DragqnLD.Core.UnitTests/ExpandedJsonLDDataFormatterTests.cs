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
        void SimpleFormatTest()
        {
            var formatter = new ExpandedJsonLDDataFormatter();
            var reader = new StringReader(json1);
            var writer = new StringWriter();

            formatter.Format(reader, writer, id1);


        }


        private string id1 = @"http://linked.opendata.cz/resource/ATC/A03EA";
        private string json1 =
@"{
    ""@graph"" : [ 
        {
            ""@id"": ""http://linked.opendata.cz/resource/ATC/A03EA"",
            ""@type"": [
                ""http://linked.opendata.cz/ontology/drug-encyclopedia/ATCConcept""
            ],
            ""http://www.w3.org/2004/02/skos/core#prefLabel"": [
                {
                    ""@value"": ""Spazmolytika, psycholeptika a analgetika v kombinaci"",
                    ""@language"": ""cs""
                },
                {
                    ""@value"": ""Antispasmodics, psycholeptics and analgesics in combination"",
                    ""@language"": ""en""
                }
            ],
            ""http://www.w3.org/2004/02/skos/core#notation"": [
                {
                    ""@value"": ""A03EA""
                }
            ]
        },
        {
            ""@id"": ""http://linked.opendata.cz/resource/ATC/N02AA59"",
            ""@type"": [
                ""http://linked.opendata.cz/ontology/drug-encyclopedia/ATCConcept""
            ],
            ""http://www.w3.org/2004/02/skos/core#prefLabel"": [
                {
                    ""@value"": ""Kodein, kombinace krom\u011b psycholeptik"",
                    ""@language"": ""cs""
                },
                {
                    ""@value"": ""Codeine, combinations excl. psycholeptics"",
                    ""@language"": ""en""
                }
            ],
            ""http://www.w3.org/2004/02/skos/core#notation"": [
                {
                    ""@value"": ""N02AA59""
                }
            ]
        }
    ]
}";
    }
}
