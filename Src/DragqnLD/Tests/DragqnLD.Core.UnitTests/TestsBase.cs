using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{
    [Collection("Tests")]
    public abstract class TestsBase
    {
        protected readonly ITestOutputHelper Output;

        protected TestsBase(ITestOutputHelper output)
        {
            Output = output;
        }
    }
}
