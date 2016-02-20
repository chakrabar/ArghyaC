using ArghyaC.CSharpRunner.Compiler;
using ArghyaC.CSharpRunner.Types;
using ArghyaC.CSharpRunner.Types.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArghyaC.CSharpRunner
{
    public class CodeRunner
    {
        public static CodeRunResult<TOutcome> CompileAndRun<TOutcome>(string code, string baseDirectory, object[] inputToMethod, 
                                                                string qualifiedClassName = "CodeComp.Program", string entryPointMethod = "Main")
        {
            var result = new CodeRunResult<TOutcome>();
            try
            {
                var compilationResult = new CSharpCompiler().Compile(code, baseDirectory);

                if (compilationResult.IsCompiled)
                {
                    FileInfo assFileInfo = new FileInfo(compilationResult.AssemblyPath);

                    var folder = assFileInfo.Directory.FullName;
                    var assembly = assFileInfo.FullName;

                    //var outcome = UntrustedCodeProcessor.GetResult<int>(folder, assembly, qualifiedClassName, entryPointMethod, _parameters);
                    //var testResults = TestCaseRunner.RunTests<int>(o => { return UntrustedCodeProcessor.GetResult<int>((folder, assembly, qualifiedClassName, entryPointMethod, o); });
                    var testResults = TestCaseRunner.RunTestCase<TOutcome>(o => { return UntrustedCodeProcessor.GetResult<TOutcome>(folder, assembly, qualifiedClassName, entryPointMethod, o); }, inputToMethod);

                    result.TestCaseResults = new List<TestCaseResult<TOutcome>> { testResults };
                    result.TestCaseResultsWithOutput = new List<TestCaseResult<TOutcome>> { testResults };
                    result.Message = testResults.HasException ? testResults.Message : "Compiled and ran";
                    result.State = testResults.HasException ? CodeCompState.Exception : CodeCompState.Compiled;
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

        public static CodeRunResult CompileAndRunTestCases<TOutcome>(string code, string baseDirectory, object[] inputToMethod, IEnumerable<Tuple<object[], TOutcome>> testCases,
                                                                string qualifiedClassName = "CodeComp.Program", string entryPointMethod = "Main") where TOutcome : IEquatable<TOutcome>
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

                    var testResults = TestCaseRunner.RunTests<TOutcome>(o => { return UntrustedCodeProcessor.GetResult<TOutcome>(folder, assembly, qualifiedClassName, entryPointMethod, o); }, testCases);

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
                
    }
}
