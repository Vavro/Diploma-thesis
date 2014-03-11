using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            await GetRandomIdTest(IngredientsIds);
        }

        [Fact]
        public async Task GetRandomIdMedicinalProducts()
        {
            await GetRandomIdTest(MedicinalProductsIds);
        }

        private async Task GetRandomIdTest(List<Uri> idsList)
        {
            var rnd = new Random(1234);

            var ingredientsCount = idsList.Count;

            TestUtilities.Profile("RandomGetIdIngredients",
                1000,
                async () =>
                {
                    var randomIdPosition = rnd.Next(ingredientsCount);

                    var documentId = idsList[randomIdPosition];

                    //Console.WriteLine("Getting document id {0}", documentId);

                    var document = await _ravenDataStore.GetDocument(TestDataConstants.IngredientsQueryDefinitionId, documentId);

                    Assert.NotNull(document.Content);
                });
        }
    }
}
