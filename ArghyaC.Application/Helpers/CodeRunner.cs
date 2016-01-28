using ArghyaC.Application.Compilers;
using ArghyaC.Application.Processors;
using System;
using System.IO;

namespace ArghyaC.Application.Helpers
{
    public class CodeRunner
    {
        const string _untrustedClass = "CodeComp.Program";
        const string _entryPoint = "Main";
        private static object[] _parameters = { "arghya" };

        public static string CompileAndRun(string code)
        {
            try
            {
                var compilationResult = new CSharpCompiler().Compile(code);

                if (compilationResult.IsCompiled)
                {
                    FileInfo assFileInfo = new FileInfo(compilationResult.AssemblyPath);

                    var folder = assFileInfo.Directory.FullName;
                    var assembly = assFileInfo.FullName;

                    var result = UntrustedCodeProcessor.GetResult<int>(folder, assembly, _untrustedClass, _entryPoint, _parameters);
                    return result.ToString();
                }
                return string.Join(". \n", compilationResult.Errors);
            }
            catch (Exception ex)
            {
                return "Did not complete! Exception - " + ex.Message;
            }
        }
    }
}
