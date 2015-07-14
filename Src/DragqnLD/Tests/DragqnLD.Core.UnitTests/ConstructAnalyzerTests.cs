using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using Raven.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using DragqnLD.Analyzers;

namespace DragqnLD.Core.UnitTests
{
    public class TestQueries
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
    enc:hasPharmacologicalAction ?pa ;
    enc:hasMechanismOfAction ?moa ;
    enc:hasPhysiologicEffect ?pe ;
    enc:hasPharmacokinetics ?pk ;
    enc:hasPregnancyCategory ?pc ;
    enc:mayTreat ?mt ;
    enc:mayPrevent ?mp ;
    enc:contraindicatedWith ?ci ;
    enc:hasMedicinalProduct ?mpg .
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
  ?mpg a enc:MedicinalProduct ;
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
      enc:MedicinalProduct ?mpg .
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
        public static readonly QueryDefinition TestQueryDefinitionIngredients = new QueryDefinition
        {
            Id = "Query/1",
            ConstructQuery = new SparqlQueryInfo()
            {
                DefaultDataSet = new Uri("http://test"),
                Query = IngredientsQuery,
                SparqlEndpoint = new Uri("http://test")
            },
            ConstructQueryUriParameterName = IngredientsQueryParamaterName
            //rest of the properties isn't read in this test
        };
    }

    public class ConstructAnalyzerTests : TestsBase
    {
        public ConstructAnalyzerTests(ITestOutputHelper output) : base(output)
        {
            _constructAnalyzer = new ConstructAnalyzer();
        }

        private readonly IConstructAnalyzer _constructAnalyzer;


        [Fact]
        public void CanAnalyzeQuery()
        {
            var queryDefinition = TestQueries.TestQueryDefinitionIngredients;

            var parsedSparqlQuery = ConstructAnalyzerHelper.ReplaceParamAndParseConstructQuery(queryDefinition);
            var context = _constructAnalyzer.CreateCompactionContextForQuery(parsedSparqlQuery);

            var contextContent = (RavenJObject)context.BuildContext.First().Value;
            var contextContentValues = contextContent.Values();

            Assert.Equal(18, contextContent.Count);
            Assert.Contains("enc", contextContent.Keys);
            Assert.Contains("skos", contextContent.Keys);
            Assert.Contains("enc:title", contextContentValues);
        }

        [Fact]
        public void CanExtractPropertyPaths()
        {
            var queryDefinition = TestQueries.TestQueryDefinitionIngredients;

            var parsedSparqlQuery = ConstructAnalyzerHelper.ReplaceParamAndParseConstructQuery(queryDefinition);
            var compactionContext = _constructAnalyzer.CreateCompactionContextForQuery(parsedSparqlQuery);
            
            var hierarchy = _constructAnalyzer.CreatePropertyPathsForQuery(parsedSparqlQuery, compactionContext);

            var root = ((IndexableObjectProperty) hierarchy.RootProperty);
            Assert.Equal(11, root.ChildProperties.Count);
            var pharmacologicalAction = (IndexableObjectProperty)root.GetPropertyByAbbreviatedName("hasPharmacologicalAction").Property;
            Assert.Equal(2, pharmacologicalAction.ChildProperties.Count);
            var medicinalProductGroup =
                (IndexableObjectProperty) root.GetPropertyByAbbreviatedName("hasMedicinalProduct").Property;
            Assert.Equal(5, medicinalProductGroup.ChildProperties.Count);
            var mpgAtc = (IndexableObjectProperty) medicinalProductGroup.GetPropertyByAbbreviatedName("hasATCConcept").Property;
            Assert.Equal(2, mpgAtc.ChildProperties.Count);
        }
    }
}
