using ArghyaC.Application.Compilers;
using ArghyaC.Application.Processors;
using ArghyaC.Domain.Entities;
using ArghyaC.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ArghyaC.Application.Helpers
{
    public class CodeRunner //TODO: mostly hard coded now. FIX!
    {
        const string _untrustedClass = "CodeComp.Program";
        const string _entryPoint = "Main";
        //private static object[] _parameters = { "arghya" };

        public static CodeRunResult CompileAndRun(string code, string baseDirectory)
        {
            var result = new CodeRunResult();
            try
            {
                var compilationResult = new CSharpCompiler().Compile(code, baseDirectory);

                if (compilationResult.IsCompiled)
                {
                    FileInfo assFileInfo = new FileInfo(compilationResult.AssemblyPath);

                    var folder = assFileInfo.Directory.FullName;
                    var assembly = assFileInfo.FullName;

                    //var outcome = UntrustedCodeProcessor.GetResult<int>(folder, assembly, _untrustedClass, _entryPoint, _parameters);
                    var testResults = RunTests<int>(o => { return UntrustedCodeProcessor.GetResult<int>(folder, assembly, _untrustedClass, _entryPoint, o); });
                    
                    result.TestCaseResults = testResults;
                    result.State = testResults.All(r => r.IsCorrect) ? CodeCompState.PassedAllCases : CodeCompState.Compiled;
                }
                else
                {
                    result.State = Domain.Enums.CodeCompState.DidNotCompile;
                    result.CompileErrors = compilationResult.Errors.ToList();
                }
            }
            catch (Exception ex)
            {
                result.Message = "Did not complete! Exception - " + ex.Message;
                result.State = Domain.Enums.CodeCompState.Exception;
            }
            return result;
        }

        private static IEnumerable<TestCaseResult> RunTests<T>(Func<object[], int> executor) //Func<int, int[], int>
        {
            var results = new List<TestCaseResult>();

            var testCases = new List<Tuple<int, int[], int>>(); //size, data, expected

            testCases.Add(new Tuple<int, int[], int>(1, new int[] { 5 }, 5));
            testCases.Add(new Tuple<int, int[], int>(2, new int[] { 2, 33, 7, 9 }, 51));
            testCases.Add(new Tuple<int, int[], int>(2, new int[] { 2, 33, 7, 9, 78 }, -1));
            testCases.Add(new Tuple<int, int[], int>(1, new int[] { 0 }, 0));
            testCases.Add(new Tuple<int, int[], int>(100, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 23, 67, 23, 23, 0, 23, 32, 555, 32, 32, 0, 0, 232, 4, 2121, 21, 0, 0, 0, 1, 0, 34, 443, 56 }, 3758));
            testCases.Add(new Tuple<int, int[], int>(0, new int[] { 1 }, -1));
            testCases.Add(new Tuple<int, int[], int>(501, new int[] { 5 }, -1));
            testCases.Add(new Tuple<int, int[], int>(33, null, -1));
            testCases.Add(new Tuple<int, int[], int>(2, new int[] { 1, 2, 3, int.MaxValue }, -1));
            testCases.Add(new Tuple<int, int[], int>(2, new int[] { 1, 2, -1, 2 }, -1));

            var sw = new Stopwatch();
            foreach (var testCase in testCases)
            {
                sw.Start();
                var outcome = executor(new object[] { testCase.Item1, testCase.Item2 }); //TODO: create app domain once and run all cases
                sw.Stop(); //TODO: this time is not correct. includes setup & teardown times
                var isPass = outcome == testCase.Item3;
                results.Add(new TestCaseResult { IsCorrect = isPass, Milliseconds = sw.ElapsedMilliseconds, Message = "Testcase " + (isPass ? "Passed" : "Failed") });
                sw.Reset();
            }
            return results;
        }
    }
}
