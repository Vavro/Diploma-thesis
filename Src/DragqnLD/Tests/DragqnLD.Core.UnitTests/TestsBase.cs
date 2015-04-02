using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests
{
    public abstract class TestsBase
    {
        protected readonly ITestOutputHelper Output;

        protected TestsBase(ITestOutputHelper output)
        {
            Output = output;
        }
    }
}
