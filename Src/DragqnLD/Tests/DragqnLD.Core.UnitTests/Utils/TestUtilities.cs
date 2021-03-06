﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace DragqnLD.Core.UnitTests.Utils
{
    public static class TestUtilities
    {
        public static void Profile(string description, int iterations, ITestOutputHelper output, Action func, Action withFirstRun = null)
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

            output.WriteLine("======================");
            output.WriteLine(description);
            output.WriteLine("First run: {0} ms", firstrunTotal);
            output.WriteLine("Iterations {0}, Total time {1} ms", iterations, allRunTotal);
            output.WriteLine("Average one run time: {0} ms", oneRunAverage);
            output.WriteLine("======================");
        }
        public async static Task Profile(string description, int iterations, ITestOutputHelper output, Func<Task> func, Func<Task> withFirstRun = null)
        {
            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var watch = Stopwatch.StartNew();
            // warm up 
            if (withFirstRun != null)
            {
                await withFirstRun();
            }
            await func();
            var firstrunTotal = watch.Elapsed.TotalMilliseconds;

            watch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                await func();
            }
            watch.Stop();
            var allRunTotal = watch.Elapsed.TotalMilliseconds;
            var oneRunAverage = allRunTotal / iterations;

            output.WriteLine("======================");
            output.WriteLine(description);
            output.WriteLine("First run: {0} ms", firstrunTotal);
            output.WriteLine("Iterations {0}, Total time {1} ms", iterations, allRunTotal);
            output.WriteLine("Average one run time: {0} ms", oneRunAverage);
            output.WriteLine("======================");
        }


        public static List<string> ReadValuesFromFile(string valuesFile)
        {
            var values = new List<string>();
            using (var fileReader = new StreamReader(valuesFile))
            {
                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    values.Add(line);
                }
            }

            return values;
        }
    }
}
