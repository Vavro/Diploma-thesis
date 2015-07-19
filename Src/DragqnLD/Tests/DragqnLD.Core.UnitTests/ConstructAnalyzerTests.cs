using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Abstraction.Indexes;
using DragqnLD.Core.Abstraction.Query;
using DragqnLD.Core.Implementations;
using Raven.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{
    public class TestQueries
    {
        public const string IngredientsQueryParamaterName = @"thingURI";

        #region IngredientsQuery
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
        #endregion

        public static DragqnLDIndexRequirements IngredientsAllPropertiesToIndex()
        {
            var propertiesToIndex = new DragqnLDIndexRequirements();
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "title" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "description" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasPharmacologicalAction.@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasPharmacologicalAction.title" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex()
            {
                PropertyPath = "hasPharmacologicalAction.description"
            });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasMechanismOfAction.@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasMechanismOfAction.title" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasPhysiologicEffect.@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasPhysiologicEffect.title" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasPharmacokinetics.@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasPharmacokinetics.title" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasPregnancyCategory" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "mayTreat.@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "mayTreat.title" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "mayTreat.description" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "mayPrevent.@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "mayPrevent.title" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "mayPrevent.description" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "contraindicatedWith.@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "contraindicatedWith.title" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "contraindicatedWith.description" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasMedicinalProduct.@id" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasMedicinalProduct.title" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex() { PropertyPath = "hasMedicinalProduct.description" });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex()
            {
                PropertyPath = "hasMedicinalProduct.hasRouteOfAdministration"
            });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex()
            {
                PropertyPath = "hasMedicinalProduct.hasDosageForm"
            });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex()
            {
                PropertyPath = "hasMedicinalProduct.hasATCConcept.@id"
            });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex()
            {
                PropertyPath = "hasMedicinalProduct.hasATCConcept.prefLabel"
            });
            propertiesToIndex.PropertiesToIndex.Add(new PropertyToIndex()
            {
                PropertyPath = "hasMedicinalProduct.hasATCConcept.notation"
            });
            return propertiesToIndex;
        }

        public static ConstructQueryAccessibleProperties IngredientsQueryHierarchy()
        {
            var root = new IndexableObjectProperty();
            var hierarchy = new ConstructQueryAccessibleProperties() {RootProperty = root};
            root.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);
            root.AddProperty("description", 
                "http://linked.opendata.cz/ontology/drug-encyclopedia/description",
                new IndexableValueProperty() {Type = ValuePropertyType.LanguageString }, true);
            var pharmacologicalAction = new IndexableObjectProperty()
            {
                HasId = true,
                HasType = true
            };
            root.AddProperty("hasPharmacologicalAction",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasPharmacologicalAction",
                pharmacologicalAction, true);
            pharmacologicalAction.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);
            pharmacologicalAction.AddProperty("description",
               "http://linked.opendata.cz/ontology/drug-encyclopedia/description",
               new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);

            var mechanismOfAction = new IndexableObjectProperty() {HasId = true, HasType = true};
            root.AddProperty("hasMechanismOfAction",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasMechanismOfAction",
                mechanismOfAction, true);
            mechanismOfAction.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);

            var physiologicEffect = new IndexableObjectProperty() {HasId = true, HasType = true};
            root.AddProperty("hasPhysiologicEffect",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasPhysiologicEffect",
                physiologicEffect, true);
            physiologicEffect.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);

            var pharmacokinetics = new IndexableObjectProperty() {HasId = true, HasType = true};
            root.AddProperty("hasPharmacokinetics",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasPharmacokinetics",
                pharmacokinetics, true);
            pharmacokinetics.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);
            pharmacokinetics.AddProperty("description",
               "http://linked.opendata.cz/ontology/drug-encyclopedia/description",
               new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);

            var pregnancyCategory = new IndexableValueProperty() {Type = ValuePropertyType.Value};
            root.AddProperty("hasPregnancyCategory",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasPregnancyCategory",
                pregnancyCategory, true);

            var treat = new IndexableObjectProperty() {HasId = true, HasType = true};
            root.AddProperty("mayTreat",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/mayTreat",
                treat, true);
            treat.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);
            treat.AddProperty("description",
               "http://linked.opendata.cz/ontology/drug-encyclopedia/description",
               new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);

            var prevent = new IndexableObjectProperty() {HasId = true, HasType = true};
            root.AddProperty("mayPrevent",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/mayPrevent",
                prevent, true);
            prevent.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);
            prevent.AddProperty("description",
               "http://linked.opendata.cz/ontology/drug-encyclopedia/description",
               new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);

            var contraindicatedWith = new IndexableObjectProperty() { HasId = true, HasType = true };
            root.AddProperty("contraindicatedWith",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/contraindicatedWith",
                contraindicatedWith, true);
            contraindicatedWith.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);
            contraindicatedWith.AddProperty("description",
               "http://linked.opendata.cz/ontology/drug-encyclopedia/description",
               new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);

            var medicinalProduct = new IndexableObjectProperty() {HasId = true, HasType = true};
            root.AddProperty("hasMedicinalProduct",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasMedicinalProduct",
                medicinalProduct, true);
            medicinalProduct.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);
            medicinalProduct.AddProperty("description",
               "http://linked.opendata.cz/ontology/drug-encyclopedia/description",
               new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);
            var hasRouteOfAdministration = new IndexableValueProperty() {Type = ValuePropertyType.LanguageString};
            medicinalProduct.AddProperty("hasRouteOfAdministration",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasRouteOfAdministration",
                hasRouteOfAdministration);
            medicinalProduct.AddProperty("hasDosageForm",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasDosageForm",
                new IndexableValueProperty() {Type = ValuePropertyType.LanguageString});
            var atcConcept = new IndexableObjectProperty() {HasId = true, HasType = true};
            medicinalProduct.AddProperty("hasATCConcept",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/hasATCConcept",
                atcConcept,true);
            atcConcept.AddProperty("prefLabel",
                "http://www.w3.org/2004/02/skos/core#prefLabel",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);
            atcConcept.AddProperty("notation",
               "http://www.w3.org/2004/02/skos/core#notation",
               new IndexableValueProperty() { Type = ValuePropertyType.Value }, true);

            return hierarchy;
        }

        public static DragqnLDIndexDefiniton IngredientsAllPropertiesIndexDefinition()
        {
            var definition = new DragqnLDIndexDefiniton()
            {
                Name = "Query/1/AllProps",
                PropertyNameMap = new Dictionary<string, string>()
                {
                    {"@id", "_id"},
                    {"title", "title"},
                    {"description", "description"},
                    {"hasPharmacologicalAction.@id", "hasPharmacologicalAction__id"},
                    {"hasPharmacologicalAction.title", "hasPharmacologicalAction_title"},
                    {"hasPharmacologicalAction.description", "hasPharmacologicalAction_description"},
                    {"hasMechanismOfAction.@id", "hasMechanismOfAction__id"},
                    {"hasMechanismOfAction.title", "hasMechanismOfAction_title"},
                    {"hasPhysiologicEffect.@id", "hasPhysiologicEffect__id"},
                    {"hasPhysiologicEffect.title", "hasPhysiologicEffect_title"},
                    {"hasPharmacokinetics.@id", "hasPharmacokinetics__id"},
                    {"hasPharmacokinetics.title", "hasPharmacokinetics_title"},
                    {"hasPregnancyCategory", "hasPregnancyCategory"},
                    {"mayTreat.@id", "mayTreat__id"},
                    {"mayTreat.title", "mayTreat_title"},
                    {"mayTreat.description", "mayTreat_description"},
                    {"mayPrevent.@id", "mayPrevent__id"},
                    {"mayPrevent.title", "mayPrevent_title"},
                    {"mayPrevent.description", "mayPrevent_description"},
                    {"contraindicatedWith.@id", "contraindicatedWith__id"},
                    {"contraindicatedWith.title", "contraindicatedWith_title"},
                    {"contraindicatedWith.description", "contraindicatedWith_description"},
                    {"hasMedicinalProduct.@id", "hasMedicinalProduct__id"},
                    {"hasMedicinalProduct.title", "hasMedicinalProduct_title"},
                    {"hasMedicinalProduct.description", "hasMedicinalProduct_description"},
                    {"hasMedicinalProduct.hasRouteOfAdministration", "hasMedicinalProduct_hasRouteOfAdministration"},
                    {"hasMedicinalProduct.hasDosageForm", "hasMedicinalProduct_hasDosageForm"},
                    {"hasMedicinalProduct.hasATCConcept.@id", "hasMedicinalProduct_hasATCConcept__id"},
                    {"hasMedicinalProduct.hasATCConcept.prefLabel", "hasMedicinalProduct_hasATCConcept_prefLabel"},
                    {"hasMedicinalProduct.hasATCConcept.notation", "hasMedicinalProduct_hasATCConcept_notation"}
                },
                RavenAnalyzers = new Dictionary<string, string>(),
                RavenMap = "from doc in docs\r\nwhere doc[\"@metadata\"][\"Raven-Entity-Name\"] == \"Query/1\"\r\nselect new { \r\n_id = doc._id,\r\ntitle = ((IEnumerable<dynamic>)doc.title).DefaultIfEmpty().Select(x0 => \r\nx0._value),\r\ndescription = ((IEnumerable<dynamic>)doc.description).DefaultIfEmpty().Select(x0 => \r\nx0._value),\r\nhasPharmacologicalAction__id = ((IEnumerable<dynamic>)doc.hasPharmacologicalAction).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasPharmacologicalAction_title = ((IEnumerable<dynamic>)doc.hasPharmacologicalAction).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasPharmacologicalAction_description = ((IEnumerable<dynamic>)doc.hasPharmacologicalAction).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.description).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasMechanismOfAction__id = ((IEnumerable<dynamic>)doc.hasMechanismOfAction).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasMechanismOfAction_title = ((IEnumerable<dynamic>)doc.hasMechanismOfAction).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasPhysiologicEffect__id = ((IEnumerable<dynamic>)doc.hasPhysiologicEffect).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasPhysiologicEffect_title = ((IEnumerable<dynamic>)doc.hasPhysiologicEffect).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasPharmacokinetics__id = ((IEnumerable<dynamic>)doc.hasPharmacokinetics).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasPharmacokinetics_title = ((IEnumerable<dynamic>)doc.hasPharmacokinetics).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasPregnancyCategory = ((IEnumerable<dynamic>)doc.hasPregnancyCategory).DefaultIfEmpty().Select(x0 => \r\nx0),\r\nmayTreat__id = ((IEnumerable<dynamic>)doc.mayTreat).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nmayTreat_title = ((IEnumerable<dynamic>)doc.mayTreat).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nmayTreat_description = ((IEnumerable<dynamic>)doc.mayTreat).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.description).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nmayPrevent__id = ((IEnumerable<dynamic>)doc.mayPrevent).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nmayPrevent_title = ((IEnumerable<dynamic>)doc.mayPrevent).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nmayPrevent_description = ((IEnumerable<dynamic>)doc.mayPrevent).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.description).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\ncontraindicatedWith__id = ((IEnumerable<dynamic>)doc.contraindicatedWith).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\ncontraindicatedWith_title = ((IEnumerable<dynamic>)doc.contraindicatedWith).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\ncontraindicatedWith_description = ((IEnumerable<dynamic>)doc.contraindicatedWith).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.description).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasMedicinalProduct__id = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\nx0._id),\r\nhasMedicinalProduct_title = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.title).DefaultIfEmpty().Select(x1 => \r\nx1._value)),\r\nhasMedicinalProduct_description = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\nx0.description),\r\nhasMedicinalProduct_hasRouteOfAdministration = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\nx0.hasRouteOfAdministration),\r\nhasMedicinalProduct_hasDosageForm = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\nx0.hasDosageForm),\r\nhasMedicinalProduct_hasATCConcept__id = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => \r\nx1._id)),\r\nhasMedicinalProduct_hasATCConcept_prefLabel = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => \r\n((IEnumerable<dynamic>)x1.prefLabel).DefaultIfEmpty().Select(x2 => \r\nx2._value))),\r\nhasMedicinalProduct_hasATCConcept_notation = ((IEnumerable<dynamic>)doc.hasMedicinalProduct).DefaultIfEmpty().Select(x0 => \r\n((IEnumerable<dynamic>)x0.hasATCConcept).DefaultIfEmpty().Select(x1 => \r\n((IEnumerable<dynamic>)x1.notation).DefaultIfEmpty().Select(x2 => \r\nx2))),\r\n_metadata_Raven_Entity_Name = doc[\"@metadata\"][\"Raven-Entity-Name\"]}",
                Requirements = IngredientsAllPropertiesToIndex()
            };
            return definition;
        }

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

    [Trait("Category", "Basic")]
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
