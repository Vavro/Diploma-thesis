using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Implementations;
using Raven.Abstractions.Indexing;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{
    public class SelectAnalyzerWithIndexTests : TestsBase
    {
        private readonly SelectAnalyzer _selectAnalyzer;

        private readonly DragqnLDIndexDefiniton _ingredientsAllPropsIndexDefinition =
            TestQueries.IngredientsAllPropertiesIndexDefinition();

        public SelectAnalyzerWithIndexTests(ITestOutputHelper output) : base(output)
        {
            _selectAnalyzer = new SelectAnalyzer();
        }

        [Fact]
        public void CanConvertSimpleQueryNoIndex()
        {
            var simpleSparql =
                @"PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
SELECT ?s
WHERE { ?s enc:title ""Nitrous oxide"" }";

            //todo: need two versions 
            //- one for no index - will have to add proper , and .
            //- second for indexed - will have to query the existing fields in the index
            // ---- for indexed need to store path to field name map
            var root = new IndexableObjectProperty();
            var hierarchy = new ConstructQueryAccessibleProperties()
            {
                RootProperty = root
            };
            root.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString });
            root.GetPropertyByAbbreviatedName("title").WrappedInArray = true;

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneWithIndex(simpleSparql, hierarchy, _ingredientsAllPropsIndexDefinition);

            const string expectedLuceneQuery = @"+title: (""Nitrous oxide"")";

            Assert.Equal(expectedLuceneQuery, convertedQuery);
        }

        [Fact]
        public void CanConvertQueryWithLangTagNoIndex()
        {
            var languageTaggedString =
                @"PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
SELECT ?s
WHERE { ?s enc:title ""Nitrous oxide""@en }";

            //todo: need two versions 
            //- one for no index - will have to add proper , and .
            //- second for indexed - will have to query the existing fields in the index
            // ---- for indexed need to store path to field name map
            var root = new IndexableObjectProperty();
            var hierarchy = new ConstructQueryAccessibleProperties()
            {
                RootProperty = root
            };
            root.AddProperty("title",
                "http://linked.opendata.cz/ontology/drug-encyclopedia/title",
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString }, true);

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneWithIndex(languageTaggedString, hierarchy, _ingredientsAllPropsIndexDefinition);

            const string expectedLuceneQuery = @"+title: (""Nitrous oxide"")";

            Assert.Equal(expectedLuceneQuery, convertedQuery);
        }

        [Fact]
        public void CanConvertComplexQuery()
        {
            var query = @"PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
SELECT ?s
WHERE 
{ 
  ?s enc:contraindicatedWith ?cw.
  ?s enc:hasPharmacologicalAction ?pa.
  ?pa enc:title ""Neopioidní analgetika""@cs.
  ?cw enc:title ""Léková alergie""@cs.
}";
            var hierarchy = TestQueries.IngredientsQueryHierarchy();

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneWithIndex(query, hierarchy, _ingredientsAllPropsIndexDefinition);

            const string expectedLuceneQuery =
@"+contraindicatedWith_title: (""Léková alergie"") +hasPharmacologicalAction_title: (""Neopioidní analgetika"")";

            Assert.Equal(expectedLuceneQuery, convertedQuery);
        }

        [Fact]
        public void CanOnvertQueryForId()
        {
            var query = @"PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
SELECT ?s
WHERE 
{ 
  ?s enc:contraindicatedWith <http://linked.opendata.cz/resource/ndfrt/disease/N0000000999>.
}";
            var hierarchy = TestQueries.IngredientsQueryHierarchy();

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneWithIndex(query, hierarchy, _ingredientsAllPropsIndexDefinition);

            const string expectedLuceneQuery = @"+contraindicatedWith__id: (""http://linked.opendata.cz/resource/ndfrt/disease/N0000000999"")";

            Assert.Equal(expectedLuceneQuery, convertedQuery);

        }

        [Fact]
        public void CanConvertNestedQuery()
        {
            var query = @"PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX skos: <http://www.w3.org/2004/02/skos/core#>

SELECT ?s
WHERE 
{ 
  ?s enc:hasMedicinalProduct ?mp.
  ?mp enc:hasATCConcept ?mpatc.
  ?mpatc skos:notation ""A03EA""
}";
            var hierarchy = TestQueries.IngredientsQueryHierarchy();

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneWithIndex(query, hierarchy, _ingredientsAllPropsIndexDefinition);

            const string expectedLuceneQuery = @"+hasMedicinalProduct_hasATCConcept_notation: (""A03EA"")";

            Assert.Equal(expectedLuceneQuery, convertedQuery);
        }

        [Fact]
        public void CanConvertVeryComplexQuery()
        {
            var query = @"PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX skos: <http://www.w3.org/2004/02/skos/core#>

SELECT ?s
WHERE 
{ 
  ?s enc:hasMedicinalProduct ?mp ;
     enc:contraindicatedWith ?cw ;
     enc:hasPharmacologicalAction ?pa.
  ?pa enc:title ""Antitusika""@cs.
  ?cw enc:title ""Léková alergie""@cs.
  ?mp enc:hasATCConcept ?mpatc;
      enc:title ""SPASMOPAN""@cs.
  ?mpatc skos:notation ""A03EA""
}";
            var hierarchy = TestQueries.IngredientsQueryHierarchy();

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneWithIndex(query, hierarchy, _ingredientsAllPropsIndexDefinition);

            const string expectedLuceneQuery = @"+contraindicatedWith_title: (""Léková alergie"") +hasMedicinalProduct_title: (""SPASMOPAN"") +hasMedicinalProduct_hasATCConcept_notation: (""A03EA"") +hasPharmacologicalAction_title: (""Antitusika"")";

            Assert.Equal(expectedLuceneQuery, convertedQuery);
        }
    }
}
