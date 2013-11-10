using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.Abstraction.Data;
using DragqnLD.Core.Implementations;
using Xunit;

namespace DragqnLD.Core.UnitTests
{
    public class RavenDataStoreTests
    {
        private readonly IDataStore _ravenDataStore;

        public RavenDataStoreTests()
        {
             _ravenDataStore = new RavenDataStore();
        }

        [Fact]
        public async Task CanStoreAndGetPlainJSONData()
        {
            var document = "{ \"name\" : \"Petr\"}";

            var dataToStore = new ConstructResult()
            {
                QueryId = "QueryDefinitions/1",
                DocumentId = new Uri(@"http://linked.opendata.cz/resource/ATC/M01AE01"),
                Document = document
            };

            await _ravenDataStore.StoreDocument(dataToStore);
        }
    }
}
