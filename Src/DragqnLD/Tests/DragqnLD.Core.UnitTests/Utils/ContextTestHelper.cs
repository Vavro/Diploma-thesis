using JsonLD.Core;

namespace DragqnLD.Core.UnitTests.Utils
{
    public class ContextTestHelper
    {
        public static Context EmptyContext()
        {
            var context = new Context();
            context.Remove("@base");

            return context;
        }
    }
}