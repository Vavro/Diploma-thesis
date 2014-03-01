using Raven.Client;
using Raven.Client.Listeners;

namespace DragqnLD.Core.UnitTests.Utils
{
    public class NoTrackingQueriesListener : IDocumentQueryListener
    {
        public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
        {
            queryCustomization.NoTracking();
        }
    }
}