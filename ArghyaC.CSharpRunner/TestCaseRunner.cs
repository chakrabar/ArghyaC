using ArghyaC.CSharpRunner.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArghyaC.CSharpRunner
{
    class TestCaseRunner
    {
        internal static TestCaseResult<TOutcome> RunTestCase<TOutcome>(Func<object[], TOutcome> codeToExecute, object[] testCase)
        {
            var result = new TestCaseResult<TOutcome>();

            var sw = new Stopwatch();
            TOutcome outcome = default(TOutcome);
            try
            {
                sw.Start();
                outcome = codeToExecute(testCase); //TODO: create app domain once and run all cases                    
            }
            catch (Exception ex)
            {
                result.HasException = true;
            }
            sw.Stop(); //TODO: this time is not correct. This includes setup & teardown times
            result = new TestCaseResult<TOutcome> { Milliseconds = sw.ElapsedMilliseconds, Output = outcome  };
            return result;
        }

        internal static IEnumerable<TestCaseResult> RunTests<T>(Func<object[], T> codeToExecute, IEnumerable<Tuple<object[], T>> testCases) where T : IEquatable<T>
        {
            var results = new List<TestCaseResult>();

            var sw = new Stopwatch();
            foreach (var testCase in testCases)
            {
                T outcome = default(T);
                try
                {
                    sw.Start();
                    outcome = codeToExecute(testCase.Item1); //TODO: create app domain once and run all cases                    
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("SecurityException")) //this is BAD!!
                    {
                        throw;
                    }
                }
                sw.Stop(); //TODO: this time is not correct. This includes setup & teardown times
                var isPass = outcome.Equals(testCase.Item2);
                results.Add(new TestCaseResult { IsCorrect = isPass, Milliseconds = sw.ElapsedMilliseconds, Message = "Testcase " + (isPass ? "Passed" : "Failed") });
                sw.Reset();
            }
            return results;
        }
    }
}
