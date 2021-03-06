﻿using System;
using DragqnLD.Core.Abstraction;
using DragqnLD.Core.UnitTests;
using DragqnLD.UnitTests.Extensions;
using DragqnLD.WebApi.Controllers;
using Xunit;

namespace DragqnLD.UnitTests
{
    public class CodeQualityEnsurance
    {
        [Trait("Category", "Code Quality")]
        [Fact]
        public void EnsureNoAsyncVoidTest()
        {
            AssertExtensions.AssertNoAsyncVoidMethods(GetType().Assembly);
            AssertExtensions.AssertNoAsyncVoidMethods(typeof(QueryStoreTests).Assembly);
            AssertExtensions.AssertNoAsyncVoidMethods(typeof(IDataLoader).Assembly);
            AssertExtensions.AssertNoAsyncVoidMethods(typeof(BaseApiController).Assembly);
        }
    }
}
