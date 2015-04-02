using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragqnLD.Core.Implementations.Utils;
using DragqnLD.Core.UnitTests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests.PerfTests.DynamicIndex
{
    public class RandomizedTwoPropertyValuesTests : DataStorePerfTestsBase
    {
        private Random _rnd;

        public RandomizedTwoPropertyValuesTests(ITestOutputHelper output, PerfDataStoreFixture fixture) : base(output, fixture)
        {
            _rnd = new Random(TestDataConstants.RandomSeed);
        }   

        //todo: dotaz na ty co nemaji Pregnancy Category - M0000171
        [Fact]
        public async Task RandomizedIngredientAndPregnancyCategory()
        {
            var items = ReadMayTreatTreatAndPregnancyItems();

            await Profile("Randomized Ingredient May Treat and Pregnancy Category", 100, async () =>
            {
                var randomItem = RandomItem(items);
                var randomPc = EncloseValue(RandomItem(randomItem.PregnancyCategories));
                var randomMt = EncloseValue(RandomItem(randomItem.MayTreatTitles));

                var result = await RavenDataStore.QueryDocumentProperties(TestDataConstants.IngredientsQueryDefinitionId,
                        TestDataConstants.PropertyNameIngredientMayTreat.AsCondition(randomMt),
                        TestDataConstants.PropertyNameIngredientPregnancyCategory.AsCondition(randomPc));

                Assert.NotEmpty(result);
            });
        }

        private string EncloseValue(string s)
        {
            return String.Format("\"{0}\"", s);
        }

        private T RandomItem<T>(List<T> items)
        {
            return items[_rnd.Next(items.Count)];
        }

        private class MayTreatAndPregnancyItem
        {
            public MayTreatAndPregnancyItem(List<string> mayTreatTitles, List<string> pregnancyCategories)
            {
                MayTreatTitles = mayTreatTitles;
                PregnancyCategories = pregnancyCategories;
            }

            public List<string> MayTreatTitles { get; private set; }
            public List<string> PregnancyCategories { get; private set; } 
        }

        private static List<MayTreatAndPregnancyItem> ReadMayTreatTreatAndPregnancyItems()
        {
            var lines = TestUtilities.ReadValuesFromFile(TestDataConstants.IngredientsMayTreatPregnancyFile);

            var results = lines.Select(s =>
            {
                var split = s.Split(new [] {";;"}, StringSplitOptions.None);
                var pcs = new List<string>();
                var mts = new List<string>();
                foreach (string s1 in split)
                {
                    var type = s1.Substring(0, 4);
                    var value = s1.Substring(4);
                    switch (type)
                    {
                        case "mt: ":
                            mts.Add(value);
                            break;
                        case "pc: ":
                            pcs.Add(value);
                            break;
                    }
                }

                return new MayTreatAndPregnancyItem(mts, pcs);
            }).ToList();

            return results;
        }
    }
}
