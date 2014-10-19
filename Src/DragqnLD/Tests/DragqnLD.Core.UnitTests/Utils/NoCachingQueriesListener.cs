using Raven.Client;
using Raven.Client.Listeners;

namespace DragqnLD.Core.UnitTests.Utils
{
    public class NoCachingQueriesListener : IDocumentQueryListener
    {
        public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
        {
            queryCustomization.NoCaching();
        }
    }
}
