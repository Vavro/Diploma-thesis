﻿using System;
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


        private string id1 = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115";
        private string json1 =
@"{
    ""@graph"" : [ 
        {
            ""@id"": ""http://linked.opendata.cz/resource/drug-encyclopedia/pharmacological-action/M0028038"",
            ""@type"": [
                ""http://linked.opendata.cz/ontology/drug-encyclopedia/PharmacologicalAction""
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/title"": [
                {
                    ""@value"": ""Neopioidn\u00ed analgetika"",
                    ""@language"": ""cs""
                },
                {
                    ""@value"": ""Analgesics, non-narcotic"",
                    ""@language"": ""en""
                }
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/description"": [
                {
                    ""@value"": ""A subclass of analgesic agents that typically do not bind to OPIOID RECEPTORS and are not addictive. Many non-narcotic analgesics are offered as NONPRESCRIPTION DRUGS.     "",
                    ""@language"": ""en""
                }
            ]
        },
        {
            ""@id"": ""http://linked.opendata.cz/resource/drug-encyclopedia/pharmacological-action/M0028037"",
            ""@type"": [
                ""http://linked.opendata.cz/ontology/drug-encyclopedia/PharmacologicalAction""
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/title"": [
                {
                    ""@value"": ""Antipyretics"",
                    ""@language"": ""en""
                },
                {
                    ""@value"": ""Antipyretika"",
                    ""@language"": ""cs""
                }
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/description"": [
                {
                    ""@value"": ""Drugs that are used to reduce body temperature in fever.     "",
                    ""@language"": ""en""
                }
            ]
        },
        {
            ""@id"": ""http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0000115"",
            ""@type"": [
                ""http://linked.opendata.cz/ontology/drug-encyclopedia/Ingredient""
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/title"": [
                {
                    ""@value"": ""Acetaminophen"",
                    ""@language"": ""en""
                },
                {
                    ""@value"": ""Acetaminofen"",
                    ""@language"": ""cs""
                }
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/hasPharmacologicalAction"": [
                ""http://linked.opendata.cz/resource/drug-encyclopedia/pharmacological-action/M0028038"",
                ""http://linked.opendata.cz/resource/drug-encyclopedia/pharmacological-action/M0028037""
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/description"": [
                {
                    ""@value"": ""Analgesic antipyretic derivative of acetanilide. It has weak anti-inflammatory properties and is used as a common analgesic, but may cause liver, blood cell, and kidney damage.     "",
                    ""@language"": ""en""
                }
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/hasMechanismOfAction"": [
                ""http://linked.opendata.cz/resource/ndfrt/mechanism-of-action/N0000000108""
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/hasPregnancyCategory"": [
                ""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/C""
            ],    
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/mayTreat"": [
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000002683"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000000863"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000000522"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000002055"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000001956"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000002655"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000002984"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000002278"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000000824"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000001681"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000002162"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000001242"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000001418"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000171630""
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/contraindicatedWith"": [
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000000999"",
                ""http://linked.opendata.cz/resource/ndfrt/disease/N0000001360""
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/hasPhysiologicEffect"": [
                ""http://linked.opendata.cz/resource/ndfrt/physiologic-effect/N0000008836"",
                ""http://linked.opendata.cz/resource/ndfrt/physiologic-effect/N0000009072""
            ],
            ""http://linked.opendata.cz/ontology/drug-encyclopedia/hasPharmacokinetics"": [
                ""http://linked.opendata.cz/resource/ndfrt/pharmacokinetics/N0000000026"",
                ""http://linked.opendata.cz/resource/ndfrt/pharmacokinetics/N0000000042""
            ]
        }
    ]
}";
    }
}
