using Raven.Client;
using Raven.Client.Listeners;

namespace DragqnLD.Core.UnitTests.Utils
{
    public class NoStaleQueriesListener : IDocumentQueryListener
    {
        public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
        {
            queryCustomization.WaitForNonStaleResults();
        }
    }
}