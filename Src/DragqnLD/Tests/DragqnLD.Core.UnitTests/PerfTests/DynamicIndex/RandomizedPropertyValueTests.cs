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
        public void RandomIngredientTitle()
        {
            var titles = ReadValuesFromFile(TestDataConstants.IngredientsTitlesFile);

            TestUtilities.Profile("Random ingredients title", 100, async () =>
            {
                var randomTitle = titles[_rnd.Next(titles.Count)];

                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsTitle.AsCondition(randomTitle));

                Assert.NotEmpty(uris);
            });
        }

        [Fact]
        public void RandomIngredientDescription()
        {
            var descriptions = ReadValuesFromFile(TestDataConstants.IngredientsDescriptionsFile);

            TestUtilities.Profile("Random ingredients description", 100, async () =>
            {
                var randomDescription = descriptions[_rnd.Next(descriptions.Count)];

                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsDescription.AsCondition(randomDescription));

                Assert.NotEmpty(uris);
            });
        }

        [Fact]
        public void RandomMedicinalProductPregnancyCategory()
        {
            var categories = new List<string>()
            {
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/A""",     
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/B""", 
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/C""", 
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/D""",
                @"""http://linked.opendata.cz/resource/fda-spl/pregnancy-category/X"""
            };

            TestUtilities.Profile("Random ingredients description", 100, async () =>
            {
                var randomDescription = categories[_rnd.Next(categories.Count)];

                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    TestDataConstants.PropertyNameIngredientsPregnancyCategory.AsCondition(randomDescription));

                Assert.NotEmpty(uris);
            });
        }

        private static List<string> ReadValuesFromFile(string valuesFile)
        {
            var values = new List<string>();
            using (var fileReader = new StreamReader(valuesFile))
            {
                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    values.Add(line);
                }
            }

            return values;
        }
    }
}
