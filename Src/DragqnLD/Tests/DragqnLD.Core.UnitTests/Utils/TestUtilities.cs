using System;
using System.Diagnostics;

namespace DragqnLD.Core.UnitTests.Utils
{
    public static class TestUtilities
    {
        public static void Profile(string description, int iterations, Action func, Action withFirstRun = null )
        {
            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var watch = Stopwatch.StartNew();
            // warm up 
            if (withFirstRun != null)
            {
                withFirstRun();
            }
            func();
            var firstrunTotal = watch.Elapsed.TotalMilliseconds;

            watch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                func();
            }
            watch.Stop();
            var allRunTotal = watch.Elapsed.TotalMilliseconds;
            var oneRunAverage = allRunTotal / iterations;

            Console.WriteLine("======================");
            Console.WriteLine(description);
            Console.WriteLine("First run: {0} ms", firstrunTotal);
            Console.WriteLine("Iterations {0}, Total time {1} ms", iterations, allRunTotal);
            Console.WriteLine("Average one run time: {0} ms", oneRunAverage);
            Console.WriteLine("======================");
        }
    }
}
