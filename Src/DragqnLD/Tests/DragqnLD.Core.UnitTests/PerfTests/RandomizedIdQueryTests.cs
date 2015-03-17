using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragqnLD.Core.UnitTests.Utils;
using Xunit;

namespace DragqnLD.Core.UnitTests.PerfTests
{
    public class RandomizedIdQueryTests : DataStorePerfTestsBase
    {
        [Fact]
        public async Task GetRandomIdIngredients()
        {
            await GetRandomIdTest(IngredientsIds, "Random Get Id Ingredients", TestDataConstants.IngredientsQueryDefinitionId);
        }

        [Fact]
        public async Task GetRandomIdMedicinalProducts()
        {
            await GetRandomIdTest(MedicinalProductsIds, "Random Get Id Medicinal Products", TestDataConstants.MedicinalProductQueryDefinitionId);
        }

        private async Task GetRandomIdTest(List<Uri> idsList, string description, string queryDefinitionId)
        {
            var rnd = new Random(TestDataConstants.RandomSeed);

            var ingredientsCount = idsList.Count;

            await TestUtilities.Profile(description,
                1000,
                async () =>
                {
                    var randomIdPosition = rnd.Next(ingredientsCount);

                    var documentId = idsList[randomIdPosition];

                    //Console.WriteLine("Getting document id {0}", documentId);

                    var document = await RavenDataStore.GetDocument(queryDefinitionId, documentId);

                    Assert.NotNull(document.Content);
                });
        }
    }
}
