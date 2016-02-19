using ArghyaC.CSharpRunner.Types;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace ArghyaC.CSharpRunner.Compiler
{
    public class CSharpCompiler
    {
        string _notUsedCodeFolder = @"C:\Arghya\Repos\ArghyaC\SubmittedCodes"; //cannot be used for Azure

        public CompileResult Compile(string code, string baseDirectory)
        {
            _notUsedCodeFolder = baseDirectory;
            var timestamp = System.DateTime.Now.ToString("yyyyMMdd_HH.mm.ss.fffffff");
            var assemblyName = System.IO.Path.Combine(_notUsedCodeFolder, "Crap_" + timestamp + ".dll");
            var fileName = System.IO.Path.Combine(_notUsedCodeFolder, "Code_" + timestamp + ".cs");

            try
            {
                if (!IsSafeCodde(code))
                    throw new SecurityException("Some suspicious code was detected!");

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
                        allErrors.Add(string.Format("{0} : {1}, @ line {2}", error.ErrorNumber, error.ErrorText, error.Line));
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
            catch (Exception ex)
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

        private bool IsSafeCodde(string code)
        {
            var unsafeTexts = new UnWantedTexts().Texts; //to static maybe
            return !unsafeTexts.Any(ut => code.Contains(ut));
        }
    }
}
