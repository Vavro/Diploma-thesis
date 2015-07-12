using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using Raven.Json.Linq;
using Xunit;

namespace DragqnLD.Core.UnitTests
{
    class TestQueries
    {
        public const string IngredientsQueryParamaterName = @"thingURI";

        public const string IngredientsQuery = 
@"PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX skos: <http://www.w3.org/2004/02/skos/core#>

CONSTRUCT
{
  @thingURI a enc:Ingredient ;
    enc:title ?title ;
    enc:description ?description ;
    enc:indication ?indication ;
    enc:hasPharmacologicalAction ?pa ;
    enc:hasMechanismOfAction ?moa ;
    enc:hasPhysiologicEffect ?pe ;
    enc:hasPharmacokinetics ?pk ;
    enc:hasPregnancyCategory ?pc ;
    enc:mayTreat ?mt ;
    enc:mayPrevent ?mp ;
    enc:contraindicatedWith ?ci ;
    enc:hasMedicinalProductGroup ?mpg .
  ?pa a enc:PharmacologicalAction ;
    enc:title ?paTitle ;
    enc:description ?paDescription .
  ?moa a enc:MechanismOfAction ;
    enc:title ?moaTitle .
  ?pe a enc:PhysiologicEffect ;
    enc:title ?peTitle .
  ?pk a enc:Pharmacokinetics ;
    enc:title ?pkTitle .
  ?mt a enc:DiseaseOrFinding ;
    enc:title ?mtTitle ;
    enc:description ?mtDescription .
  ?mp a enc:DiseaseOrFinding ;
    enc:title ?mpTitle ;
    enc:description ?mpDescription .
  ?ci a enc:DiseaseOrFinding ;
    enc:title ?ciTitle ;
    enc:description ?ciDescription .
  ?mpg a enc:MedicinalProductGroup ;
    enc:title ?mpgTitle ;
    enc:description ?mpgDescription ;
    enc:hasRouteOfAdministration ?mpgRoALabel ;
    enc:hasDosageForm ?mpgDFLabel ;
    enc:hasATCConcept ?mpgATC .
  ?mpgATC a enc:ATCConcept ;
    skos:prefLabel ?mpgATCLabel ;
    skos:notation ?mpgATCNotation .
}
WHERE
{
  {
    @thingURI
      enc:title ?title .
    OPTIONAL {
      @thingURI
        enc:description ?description .
    }
    OPTIONAL {
      @thingURI
        enc:indication ?indication .
    }
    OPTIONAL {
      @thingURI
        enc:hasPharmacologicalAction ?pa .
      ?pa enc:title ?paTitle .
      OPTIONAL {
        ?pa enc:description ?paDescription .
      }
    }
    OPTIONAL {
      @thingURI
        enc:hasMechanismOfAction ?moa .
      ?moa enc:title ?moaTitle .
    }
    OPTIONAL {
      @thingURI
        enc:hasPhysiologicEffect ?pe .
      ?pe enc:title ?peTitle .
    }
    OPTIONAL {
      @thingURI
        enc:hasPharmacokinetics ?pk .
      ?pk enc:title ?pkTitle .
    }
    OPTIONAL {
      @thingURI
        enc:hasPregnancyCategory ?pc .
    }
  }
  UNION
  {
    @thingURI
      enc:hasMedicinalProductGroup ?mpg .
    ?mpg enc:title ?mpgTitle .
    OPTIONAL {
      ?mpg enc:description ?mpgDescription .
    }
    OPTIONAL {
      ?mpg enc:hasRouteOfAdministration ?mpgRoA .
      ?mpgRoA skos:prefLabel ?mpgRoALabel .
    }
    OPTIONAL {
      ?mpg enc:hasDosageForm ?mpgDF .
      ?mpgDF skos:prefLabel ?mpgDFLabel .
    }
    OPTIONAL {
      ?mpg enc:hasATCConcept ?mpgATC .
      ?mpgATC skos:prefLabel ?mpgATCLabel ;
              skos:notation ?mpgATCNotation .
    }
  }
  UNION
  {
    @thingURI
      enc:mayTreat ?mt .
    ?mt enc:title ?mtTitle .
    OPTIONAL {
      ?mt enc:description ?mtDescription .
    }
  }
  UNION
  {
    @thingURI
      enc:mayPrevent ?mp .
    ?mp enc:title ?mpTitle .
    OPTIONAL {
      ?mp enc:description ?mpDescription .
    }
  }
  UNION
  {
    @thingURI
      enc:contraindicatedWith ?ci .
    ?ci enc:title ?ciTitle .
    OPTIONAL {
      ?ci enc:description ?ciDescription .
    }
  }
}";
    }

    public class ConstructAnalyzerTests
    {
        public ConstructAnalyzerTests()
        {
            _constructAnalyzer = new ConstructAnalyzer();
        }

        private readonly IConstructAnalyzer _constructAnalyzer;

        [Fact]
        public void CanAnalyzeQuery()
        {
            var queryDefinition = new QueryDefinition
            {
                ConstructQuery = new SparqlQueryInfo()
                {
                    DefaultDataSet = new Uri("http://test"),
                    Query = TestQueries.IngredientsQuery,
                    SparqlEndpoint = new Uri("http://test")
                },
                ConstructQueryUriParameterName = TestQueries.IngredientsQueryParamaterName
                //rest of the properties isn't read in this test
            };

            var context = _constructAnalyzer.CreateCompactionContextForQuery(queryDefinition);

            var contextContent = (RavenJObject)context.First().Value;
            var contextContentValues = contextContent.Values();

            Assert.Equal(19, contextContent.Count);
            Assert.Contains("enc", contextContent.Keys);
            Assert.Contains("skos", contextContent.Keys);
            Assert.Contains("enc:title", contextContentValues);
        }
    }
}
