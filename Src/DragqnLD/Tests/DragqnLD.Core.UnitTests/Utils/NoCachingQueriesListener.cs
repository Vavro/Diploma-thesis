using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
