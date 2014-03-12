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
        private const string PropertyNameIngredientsTitle = @"http://linked.opendata.cz/ontology/drug-encyclopedia/title,@value";

        [Fact]
        public void RandomIngredientTitle()
        {
            var titles = ReadAllTitles();
            
            var rnd = new Random(TestDataConstants.RandomSeed);

            TestUtilities.Profile("Random ingredients title", 100, async () =>
            {
                var randomTitle = titles[rnd.Next(titles.Count)];

                var uris = await _ravenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                    PropertyNameIngredientsTitle.AsCondition(randomTitle));

                Assert.NotEqual(0, uris.Count());
            });
            
        }

        private static List<string> ReadAllTitles()
        {
            var titles = new List<string>();
            using (var fileReader = new StreamReader(TestDataConstants.IngredientsTitlesFile))
            {
                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    titles.Add(line);
                }
            }

            return titles;
        }
    }
}
