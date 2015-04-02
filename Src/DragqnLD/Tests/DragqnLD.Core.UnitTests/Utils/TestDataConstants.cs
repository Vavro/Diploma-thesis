namespace DragqnLD.Core.UnitTests.Utils
{
    public static class TestDataConstants
    {
        private const string RelTestDataDir = @"..\..\..\..\..\..\Doc\Test data\";

        public const int RandomSeed = 12345;
        public const string IngredientsFolder = RelTestDataDir + "Ingredients";
        public const string IngredientsTitlesFile = RelTestDataDir + "IngredientTitles.txt";
        public const string IngredientsMayTreatPregnancyFile = RelTestDataDir + "IngredientsMayTreatPregnancy.txt";
        public const string IngredientsDescriptionsFile = RelTestDataDir + "IngredientDescriptions.txt";
        public const string MedicinalProductsFolder = @"..\..\..\..\..\..\Doc\Test data\MedicinalProducts";
        public const string MedicinalProductsTitlesFile = @"..\..\..\..\..\..\Doc\Test data\MedicinalProductsTitles.txt";
        public const string IngredientsNamespacePrefix = @"http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/";
        public const string IngredientsQueryDefinitionId = "QueryDefinitions/1";
        public const string MedicinalProductNamespacePrefix = @"http://linked.opendata.cz/resource/sukl/medicinal-product/";
        public const string MedicinalProductQueryDefinitionId = "QueryDefinitions/2";
        public const string PropertyNameIngredientsDescription = @"http://linked.opendata.cz/ontology/drug-encyclopedia/description,@value";
        public const string PropertyNameMedicinalProductsTitle = @"http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value";
        public const string PropertyNameIngredientsPregnancyCategory = @"http://linked.opendata.cz/ontology/drug-encyclopedia/hasPregnancyCategory,";
        public const string PropertyNameIngredientMayTreat = @"http://linked.opendata.cz/ontology/drug-encyclopedia/mayTreat,http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value";
        public const string PropertyNameIngredientPregnancyCategory = @"http://linked.opendata.cz/ontology/drug-encyclopedia/hasPregnancyCategory,";
        public const string AnalyzerLuceneStandard = "Lucene.Net.Analysis.Standard.StandardAnalyzer";
        public const string PropertyNameIngredientsTitle = @"http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value";
    }
}