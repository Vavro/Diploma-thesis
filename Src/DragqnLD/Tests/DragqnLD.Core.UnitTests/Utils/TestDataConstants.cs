namespace DragqnLD.Core.UnitTests.Utils
{
    static internal class TestDataConstants
    {
        
        public const int RandomSeed = 12345;
        public const string IngredientsFolder = @"..\..\..\..\..\..\Doc\Test data\Ingredients";
        public const string IngredientsTitlesFile = @"..\..\..\..\..\..\Doc\Test data\IngredientTitles.txt";
        public const string IngredientsDescriptionsFile = @"..\..\..\..\..\..\Doc\Test data\IngredientDescriptions.txt";
        public const string MedicinalProductsFolder = @"..\..\..\..\..\..\Doc\Test data\MedicinalProducts";
        internal const string IngredientsNamespacePrefix = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/";
        internal const string IngredientsQueryDefinitionId = "QueryDefinitions/1";
        internal const string MedicinalProductNamespacePrefix = @"http://linked.opendata.cz/resource/sukl/medicinal-product/";
        internal const string MedicinalProductQueryDefinitionId = "QueryDefinitions/2";
        public const string PropertyNameIngredientsDescription = @"http://linked.opendata.cz/ontology/drug-encyclopedia/description,@value";
        public const string PropertyNameMedicinalProductsTitle = @"http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value";
        public const string PropertyNameIngredientsPregnancyCategory = @"http://linked.opendata.cz/ontology/drug-encyclopedia/hasPregnancyCategory,";
        public const string PropertyNameIngredientMayTreat = @"http://linked.opendata.cz/ontology/drug-encyclopedia/mayTreat,http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value";
        public const string PropertyNameIngredientPregnancyCategory = @"http://linked.opendata.cz/ontology/drug-encyclopedia/hasPregnancyCategory,";
        public const string AnalyzerLuceneStandard = "Lucene.Net.Analysis.Standard.StandardAnalyzer";
        public const string PropertyNameIngredientsTitle = @"http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value";
    }
}