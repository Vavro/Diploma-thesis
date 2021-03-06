﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction.ConstructAnalyzer;
using DragqnLD.Core.Implementations;
using Raven.Abstractions.Data;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{
    [Trait("Category","Basic")]
    public class SelectAnalyzerNoIndexTests : TestsBase
    {
        private SelectAnalyzer _selectAnalyzer;

        public SelectAnalyzerNoIndexTests(ITestOutputHelper output) : base(output)
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
                new IndexableValueProperty() { Type = ValuePropertyType.LanguageString});
            root.GetPropertyByAbbreviatedName("title").WrappedInArray = true;

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneNoIndex(simpleSparql, hierarchy);

            const string expectedLuceneQuery = @"+title,_value: (""Nitrous oxide"")";

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

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneNoIndex(languageTaggedString, hierarchy);
            
            const string expectedLuceneQuery = @"+title,_value: (""Nitrous oxide"")";

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

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneNoIndex(query, hierarchy);

            const string expectedLuceneQuery = 
@"+contraindicatedWith,title,_value: (""Léková alergie"") +hasPharmacologicalAction,title,_value: (""Neopioidní analgetika"")";

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

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneNoIndex(query, hierarchy);

            const string expectedLuceneQuery = @"+contraindicatedWith,_id: (""http://linked.opendata.cz/resource/ndfrt/disease/N0000000999"")";

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

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneNoIndex(query, hierarchy);

            const string expectedLuceneQuery = @"+hasMedicinalProduct,hasATCConcept,notation: (""A03EA"")";

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

            var convertedQuery = _selectAnalyzer.ConvertSparqlToLuceneNoIndex(query, hierarchy);

            const string expectedLuceneQuery = @"+contraindicatedWith,title,_value: (""Léková alergie"") +hasMedicinalProduct,title,_value: (""SPASMOPAN"") +hasMedicinalProduct,hasATCConcept,notation: (""A03EA"") +hasPharmacologicalAction,title,_value: (""Antitusika"")";

            Assert.Equal(expectedLuceneQuery, convertedQuery);
        }
    }
}
