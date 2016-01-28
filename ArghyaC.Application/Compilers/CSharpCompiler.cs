using ArghyaC.Domain.Entities;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace ArghyaC.Application.Compilers
{
    public class CSharpCompiler
    {
        const string codeFolder = @"C:\Arghya\Repos\ArghyaC\SubmittedCodes";

        public CompileResult Compile(string code)
        {
            var timestamp = System.DateTime.Now.ToString("yyyyMMdd_hh.mm.ss.fffffff");
            var assemblyName = System.IO.Path.Combine(codeFolder, "Crap_" + timestamp + ".dll");
            var fileName = System.IO.Path.Combine(codeFolder, "Code_" + timestamp + ".cs");

            try
            {
                System.IO.File.WriteAllText(fileName, code);
                                
                var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
                var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll", "System.dll" }, assemblyName, false); //foo.exe, true
                parameters.GenerateExecutable = false; //true

                CompilerResults results = csc.CompileAssemblyFromSource(parameters, code);

                var allErrors = new List<string>();
                if (results.Errors.HasErrors)
                {
                    var errors = results.Errors;
                    var totalErrors = results.Errors.Count;
                    for (int i = 0; i < totalErrors; i++)
                    {
                        var error = errors[i];
                        allErrors.Add(string.Format("{0} : {1} at ({2})", error.ErrorNumber, error.ErrorText, error.Line));
                    }
                }
                return new CompileResult
                {
                    AssemblyPath = assemblyName,
                    FilePath = fileName,
                    Errors = allErrors,
                    IsCompiled = !results.Errors.HasErrors
                };
            }
            catch(Exception ex)
            {
                return new CompileResult
                {
                    AssemblyPath = assemblyName,
                    FilePath = fileName,
                    IsCompiled = false,
                    CompileException = ex
                };
            }
        }
    }
}
