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
        public void GetRandomIdIngredients()
        {
            GetRandomIdTest(IngredientsIds, "Random Get Id Ingredients");
        }

        [Fact]
        public void GetRandomIdMedicinalProducts()
        {
            GetRandomIdTest(MedicinalProductsIds, "Random Get Id Medicinal Products");
        }

        private void GetRandomIdTest(List<Uri> idsList, string description)
        {
            var rnd = new Random(TestDataConstants.RandomSeed);

            var ingredientsCount = idsList.Count;

            TestUtilities.Profile(description,
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
