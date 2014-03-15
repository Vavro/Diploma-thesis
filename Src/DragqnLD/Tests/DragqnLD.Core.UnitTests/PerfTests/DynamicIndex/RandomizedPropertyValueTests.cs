using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using DragqnLD.Core.Implementations.Utils;
using DragqnLD.Core.UnitTests.Utils;
using Raven.Database.Data;
using Xunit;

namespace DragqnLD.Core.UnitTests.PerfTests.DynamicIndex
{
    public class RandomizedPropertyValueTests : DataStorePerfTestsBase
    {
        private readonly Random _rnd;

        public RandomizedPropertyValueTests()
        {
            _rnd = new Random(TestDataConstants.RandomSeed);
        }

        [Fact]
        public async Task RandomIngredientTitle()
        {
            var titles = TestUtilities.ReadValuesFromFile(TestDataConstants.IngredientsTitlesFile);

            await TestUtilities.Profile("Random ingredients title", 100, async () =>
            {
                var randomTitle = String.Format("\"{0}\"",titles[_rnd.Next(titles.Count)]);

                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsTitle.AsCondition(randomTitle));

                Assert.NotEmpty(uris);
            });
        }

        [Fact]
        public async Task RandomIngredientDescription() 
        {
            var descriptions = TestUtilities.ReadValuesFromFile(TestDataConstants.IngredientsDescriptionsFile);

            await TestUtilities.Profile("Random ingredients description", 100, async () =>
            {
                var randomDescription = String.Format("\"{0}\"",descriptions[_rnd.Next(descriptions.Count)]);
                
                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsDescription.AsCondition(randomDescription));

                Assert.NotEmpty(uris);
            });
        }

        [Fact]
        public async Task RandomMedicinalProductPregnancyCategory()
        {
            var categories = new List<string>()
            {
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/A""",     
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/B""", 
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/C""", 
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/D""",
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/X"""
            };

            await TestUtilities.Profile("Random ingredients description", 100, async () =>
            {
                var randomDescription = categories[_rnd.Next(categories.Count)];

                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsPregnancyCategory.AsCondition(randomDescription));

                Assert.NotEmpty(uris);
            });
        }

        [Fact]
        public async Task RandomMedicinalProductTitle()
        {
            var titles = TestUtilities.ReadValuesFromFile(TestDataConstants.MedicinalProductsTitlesFile);

            await TestUtilities.Profile("Random ingredients title", 100, async () =>
            {
                var randomTitle = String.Format("\"{0}\"",titles[_rnd.Next(titles.Count)]);

                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.MedicinalProductQueryDefinitionId,
                    TestDataConstants.PropertyNameMedicinalProductsTitle.AsCondition(randomTitle));

                Assert.NotEmpty(uris);
            });
        }
    }
}
