using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragqnLD.Core.UnitTests
{
    public static class TestUtilities
    {
        public static void Profile(string description, int iterations, Action func)
        {
            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var watch = Stopwatch.StartNew();
            // warm up 
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
