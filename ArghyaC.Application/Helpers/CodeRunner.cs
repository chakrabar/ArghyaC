using ArghyaC.Application.Compilers;
using ArghyaC.Application.Processors;
using ArghyaC.Domain.Entities;
using System;
using System.IO;
using System.Linq;

namespace ArghyaC.Application.Helpers
{
    public class CodeRunner
    {
        const string _untrustedClass = "CodeComp.Program";
        const string _entryPoint = "Main";
        private static object[] _parameters = { "arghya" };

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

                    var outcome = UntrustedCodeProcessor.GetResult<int>(folder, assembly, _untrustedClass, _entryPoint, _parameters);
                    result.State = Domain.Enums.CodeCompState.Compiled;
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
    }
}
