using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Implementations.Utils;
using DragqnLD.Core.UnitTests.Utils;
using Raven.Abstractions.Data;
using Raven.Tests.Helpers;
using Xunit;

namespace DragqnLD.Core.UnitTests.PerfTests.DynamicIndex
{
    public class RandomizedStartingWithTests : RavenDataStoreQueryPerformanceTests
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
                var randomTitle = titles[_rnd.Next(titles.Count)];
                var startWithTitle = randomTitle.Substring(0, _rnd.Next(1,randomTitle.Length)) + "*";
                if (startWithTitle.Contains(" "))
                {
                    startWithTitle = "\"" + startWithTitle + "\"";
                }

                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsTitle.AsCondition(startWithTitle));
                
                var isEmpty = !uris.Any();
                Assert.NotEmpty(uris);
            });
        }

        [Fact]
        public async void RandomMedicalProductsTitleStartWith()
        {
            var titles = TestUtilities.ReadValuesFromFile(TestDataConstants.MedicinalProductsTitlesFile);

            await TestUtilities.Profile("Random medicinal products title starts with", 100, async () =>
            {
                var randomTitle = titles[_rnd.Next(titles.Count)];
                var startWithTitle = randomTitle.Substring(0, _rnd.Next(1, randomTitle.Length)) + "*";
                if (startWithTitle.Contains(" "))
                {
                    startWithTitle = "\"" + startWithTitle + "\"";
                }

                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsTitle.AsCondition(startWithTitle));

                Assert.NotEmpty(uris);
            });
        }
    }

    
}
