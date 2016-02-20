using ArghyaC.CSharpRunner.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ArghyaC.CSharpRunner
{
    class TestCaseRunner
    {
        internal static TestCaseResult<TOutcome> RunTestCaseWithHardTimeout<TOutcome>(Func<object[], TOutcome> codeToExecute, object[] testCase, int waitMilliseconds = 1000)
        {
            TOutcome outcome = default(TOutcome);
            var result = new TestCaseResult<TOutcome>();
            var sw = new Stopwatch();

            try
            {
                sw.Start();
                var func = new Func<TOutcome>(() =>
                {
                    return codeToExecute(testCase);
                });

                //TryExecute(func, waitMilliseconds, out outcome);
                outcome = Execute(func, waitMilliseconds);
            }
            catch (AggregateException ae)
            {
                result.HasException = true;
                result.Message = ae.InnerExceptions != null && ae.InnerExceptions.Any(e => e.Message.ToLower().Contains("task was cancel"))
                                    ? "Code execution timed out!"
                                    : (ae.InnerExceptions == null ? "Code failed to run!" : ae.InnerExceptions.First().Message);
            }
            catch (Exception ex)
            {
                result.HasException = true;
                result.Message = ex.Message;
            }
            finally
            {
                sw.Stop();
            }

            result = new TestCaseResult<TOutcome> { Milliseconds = sw.ElapsedMilliseconds, Output = outcome, HasException = result.HasException, Message = result.Message };
            return result;
        }

        private static T Execute<T>(Func<T> func, int timeout)
        {
            T result;
            TryExecute(func, timeout, out result);
            return result;
        }

        private static bool TryExecute<T>(Func<T> func, int timeout, out T result)
        {
            //http://stackoverflow.com/questions/1370811/implementing-a-timeout-on-a-function-returning-a-value
            var t = default(T);
            var thread = new Thread(() => t = func());
            thread.IsBackground = true; //added
            thread.Start();
            var completed = thread.Join(timeout);
            if (!completed)
            {
                thread.Abort();
                throw new TimeoutException("Code ran for too long!"); //added
            }
            result = t;
            return completed;
        }

        internal static TestCaseResult<TOutcome> RunTestCaseWithSoftTimeout<TOutcome>(Func<object[], TOutcome> codeToExecute, object[] testCase, int waitMilliseconds = 1000)
        {
            TOutcome outcome = default(TOutcome);
            var result = new TestCaseResult<TOutcome>();
            CancellationTokenSource cts;
            var sw = new Stopwatch();

            try
            {
                cts = new CancellationTokenSource(waitMilliseconds);
                cts.CancelAfter(waitMilliseconds);
                var token = cts.Token;
                Action codeAction = () => { outcome = codeToExecute(testCase); token.ThrowIfCancellationRequested(); };
                //Action codeAction = () => { outcome = codeToExecute(testCase); };
                //Func<TOutcome> func = () => { return codeToExecute(testCase);};

                sw.Start();
                var codeTask = Task.Run(codeAction, token);
                //var codeTask = Task.Run(codeAction);
                Task.WaitAll(new Task[] { codeTask }, waitMilliseconds);
                //Task.WaitAll(new Task[] { codeTask }); //, waitMilliseconds##############
            }
            catch (AggregateException ae)
            {
                result.HasException = true;
                result.Message = ae.InnerExceptions != null && ae.InnerExceptions.Any(e => e.Message.ToLower().Contains("task was cancel"))
                                    ? "Code execution timed out!"
                                    : (ae.InnerExceptions == null ? "Code failed to run!" : ae.InnerExceptions.First().Message);
            }
            catch (Exception ex)
            {
                result.HasException = true;
                result.Message = ex.Message;
            }
            finally
            {
                sw.Stop();
                cts = null;
            }

            result = new TestCaseResult<TOutcome> { Milliseconds = sw.ElapsedMilliseconds, Output = outcome, HasException = result.HasException, Message = result.Message };
            return result;
        }

        internal static TestCaseResult<TOutcome> RunTestCaseWithTimeoutTry2<TOutcome>(Func<object[], TOutcome> codeToExecute, object[] testCase, int waitMilliseconds = 1000)
        {
            TOutcome outcome = default(TOutcome);
            var result = new TestCaseResult<TOutcome>();
            CancellationTokenSource cts;
            var sw = new Stopwatch();


            Task timeoutTask = null;
            try
            {
                //cts = new CancellationTokenSource(waitMilliseconds);
                //cts.CancelAfter(waitMilliseconds);
                //var token = cts.Token;
                //Action codeAction = () => { outcome = codeToExecute(testCase); token.ThrowIfCancellationRequested(); };
                //Action codeAction = () => { outcome = codeToExecute(testCase); };
                //Func<TOutcome> func = () => { return codeToExecute(testCase);};

                sw.Start();
                //var codeTask = Task.Run(codeAction, token);
                //var codeTask = Task.Run(codeAction);
                //Task.WaitAll(codeTask, waitMilliseconds);
                //Task.WaitAll(new Task[] { codeTask }); //, waitMilliseconds##############

                Action codeAction = () => { outcome = codeToExecute(testCase); };
                //Func<TOutcome> func = () => { return codeToExecute(testCase);};

                var delay = Task.Delay(TimeSpan.FromMilliseconds(waitMilliseconds));
                var codeTask = Task.Run(codeAction);
                timeoutTask = Task.WhenAll(codeTask, delay);

                timeoutTask.Wait();

                //if (t.Status == TaskStatus.RanToCompletion)
            }
            catch (AggregateException ae)
            {
                result.HasException = true;
                result.Message = ae.InnerExceptions != null && ae.InnerExceptions.Any(e => e.Message.ToLower().Contains("task was cancel"))
                                    ? "Code execution timed out!"
                                    : (ae.InnerExceptions == null ? "Code failed to run!" : ae.InnerExceptions.First().Message);
            }
            catch (Exception ex)
            {
                result.HasException = true;
                result.Message = ex.Message;
            }
            finally
            {
                sw.Stop();
                cts = null;
            }

            if (timeoutTask.IsCanceled)
            {
                result.HasException = true;
                result.Message = "Code execution timed out!";
            }
            if (timeoutTask.IsFaulted)
            {
                result.HasException = true;
                result.Message = timeoutTask.Exception.InnerException.Message;
            }

            result = new TestCaseResult<TOutcome> { Milliseconds = sw.ElapsedMilliseconds, Output = outcome, HasException = result.HasException, Message = result.Message };
            return result;
        }


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
                result.Message = ex.Message;
            }
            sw.Stop(); //TODO: this time is not correct. This includes setup & teardown times
            result = new TestCaseResult<TOutcome> { Milliseconds = sw.ElapsedMilliseconds, Output = outcome, HasException = result.HasException, Message = result.Message  };
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
