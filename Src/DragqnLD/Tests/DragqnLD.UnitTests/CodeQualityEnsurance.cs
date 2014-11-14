using System;
using DragqnLD.Core.Abstraction;
using DragqnLD.UnitTests.Extensions;
using DragqnLD.WebApi.Controllers;
using Xunit;

namespace DragqnLD.UnitTests
{
    public class CodeQualityEnsurance
    {
        [Fact]
        public void EnsureNoAsyncVoidTest()
        {
            AssertExtensions.AssertNoAsyncVoidMethods(GetType().Assembly);
            AssertExtensions.AssertNoAsyncVoidMethods(typeof(IDataLoader).Assembly);
            AssertExtensions.AssertNoAsyncVoidMethods(typeof(BaseApiController).Assembly);
        }
    }
}
