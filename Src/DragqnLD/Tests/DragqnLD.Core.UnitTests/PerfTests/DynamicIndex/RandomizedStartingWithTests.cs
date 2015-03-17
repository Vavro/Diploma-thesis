using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragqnLD.Core.Implementations.Utils;
using DragqnLD.Core.UnitTests.Utils;
using Xunit;

namespace DragqnLD.Core.UnitTests.PerfTests.DynamicIndex
{
    public class RandomizedStartingWithTests : DataStorePerfTestsBase
    {
        private readonly Random _rnd;

        public RandomizedStartingWithTests()
        {
            _rnd = new Random(TestDataConstants.RandomSeed);
        }

        [Fact]
        public async Task RandomIngredientsTitleStartWith()
        {
            var titles = TestUtilities.ReadValuesFromFile(TestDataConstants.IngredientsTitlesFile);

            await TestUtilities.Profile("Random ingredients title starts with", 100, async () =>
            {
                var startWithTitle = NextRandomStartsWithValue(titles);

                var uris = await RavenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsTitle.AsCondition(startWithTitle));
                
                Assert.NotEmpty(uris);
            });
        }

        [Fact]
        public async Task RandomMedicalProductsTitleStartWith()
        {
            var titles = TestUtilities.ReadValuesFromFile(TestDataConstants.MedicinalProductsTitlesFile);

            await TestUtilities.Profile("Random medicinal products title starts with", 100, async () =>
            {
                var startWithTitle = NextRandomStartsWithValue(titles);

                var uris = await RavenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsTitle.AsCondition(startWithTitle));

                Assert.NotEmpty(uris);
            });
        }

        private string NextRandomStartsWithValue(List<string> list)
        {
            var randomTitle = list[_rnd.Next(list.Count)];
            var startWithTitle = randomTitle.Substring(0, _rnd.Next(1, randomTitle.Length)) + "*";
            if (startWithTitle.Contains(" "))
            {
                startWithTitle = "\"" + startWithTitle + "\"";
            }
            return startWithTitle;
        }
    }

    
}
