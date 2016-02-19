using ArghyaC.CSharpRunner.Compiler;
using ArghyaC.CSharpRunner.Types;
using ArghyaC.CSharpRunner.Types.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArghyaC.CSharpRunner
{
    public class CodeRunnerOld
    {
        //Not to be used
        public static CodeRunResult CompileAndRunTestsForOld(string code, string baseDirectory, string qualifiedClassName = "CodeComp.Program", string entryPointMethod = "Main")
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

                    //var outcome = UntrustedCodeProcessor.GetResult<int>(folder, assembly, qualifiedClassName, entryPointMethod, _parameters);
                    var testCases = GetTestCases();
                    var testResults = TestCaseRunner.RunTests<int>(o => { return UntrustedCodeProcessor.GetResult<int>(folder, assembly, qualifiedClassName, entryPointMethod, o); }, testCases);

                    result.TestCaseResults = testResults;
                    result.State = testResults.All(r => r.IsCorrect) ? CodeCompState.PassedAllCases : CodeCompState.Compiled;
                }
                else
                {
                    if (compilationResult.CompileException != null)
                    {
                        result.State = CodeCompState.Exception;
                        result.Message = "Compiler threw. " + compilationResult.CompileException.Message;
                    }
                    else
                    {
                        result.State = CodeCompState.DidNotCompile;
                        result.CompileErrors = compilationResult.Errors.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = "Did not complete! Exception - " + ex.Message;
                result.State = CodeCompState.Exception;
            }
            return result;
        }

        public static List<Tuple<object[], int>> GetTestCases()
        {
            var testCases2 = new List<Tuple<object[], int>>(); //size, data, expected
            testCases2.Add(new Tuple<object[], int>(new object[] {1, new int[] { 5 }}, 5));
            testCases2.Add(new Tuple<object[], int>(new object[] {2, new int[] { 2, 33, 7, 9 }}, 51));
            testCases2.Add(new Tuple<object[], int>(new object[] {2, new int[] { 2, 33, 7, 9, 78 }}, -1));
            testCases2.Add(new Tuple<object[], int>(new object[] {1, new int[] { 0 }}, 0));
            testCases2.Add(new Tuple<object[], int>(new object[] {100, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 23, 67, 23, 23, 0, 23, 32, 555, 32, 32, 0, 0, 232, 4, 2121, 21, 0, 0, 0, 1, 0, 34, 443, 56 }}, 3758));
            testCases2.Add(new Tuple<object[], int>(new object[] {0, new int[] { 1 }}, -1));
            testCases2.Add(new Tuple<object[], int>(new object[] {501, new int[] { 5 }}, -1));
            testCases2.Add(new Tuple<object[], int>(new object[] {33, null}, -1));
            testCases2.Add(new Tuple<object[], int>(new object[] {2, new int[] { 1, 2, 3, int.MaxValue }}, -1));
            testCases2.Add(new Tuple<object[], int>(new object[] {2, new int[] { 1, 2, -1, 2 }}, -1));
            return testCases2;
        }
    }
}
