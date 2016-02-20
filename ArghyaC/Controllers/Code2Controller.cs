using ArghyaC.CSharpRunner;
using ArghyaC.CSharpRunner.Types;
using ArghyaC.Infrastructure.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ArghyaC.Controllers
{
    public class Code2Controller : Controller
    {
        List<string> _instructions = new List<string>
        {
            @"Given a starting position, design an intelligent bot who can take the data (int m, int n, int x, int y, int [m][n] data), process & analyse it, and come up with a set of steps to reach a safe spot travelling the longest path possible.",
            "Int m | width (1 – 10,000)",
            "Int n | height (1 – 10,000)",
            "Int x | start position x (starts from 0, from left)",
            "Int y | start position y (starts from 0, from bottom)",
            "Int[m][n] data (where data[i][j] = 0 or 1), starting from left to right, bottom to top"
        };

        private string _code =
@"//Do NOT change the namespace & class name `Program` and `Main` method signature
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeComp
{
    public class Program
    {
        public static string[] Main(int m, int n, int x, int y, int [][] data)
        {
            return null;
        }
    }
    //other classes if required
}";

        public ActionResult Index()
        {
            return View(new CodeRunResult { Code = _code, Instructions = _instructions });
        }

        [HttpPost]
        public ActionResult Index(string code)
        {
            var timestamp = System.DateTime.Now.ToString("yyyyMMdd_HH.mm.ss.fffffff");
            var tempFolder = Server.MapPath("~/SubmittedCode/Temp_" + timestamp);

            var result = FileUtilities.TempCreateDirAndExecute<CodeRunResult<string[]>>(tempFolder,
                () => { return CodeRunner.CompileAndRun<string[]>(code, tempFolder, new object[] { 1, 2, 3, 4, null }); });

            result.Code = code;
            result.Instructions = _instructions;
            if (result.State == CSharpRunner.Types.Enums.CodeCompState.Compiled && result.TestCaseResultsWithOutput != null
                    && result.TestCaseResultsWithOutput.Count() > 0 && result.TestCaseResultsWithOutput.First().Output != null)
            {
                var progSpits = result.TestCaseResultsWithOutput.First().Output.Select(r => r ?? string.Empty);
                result.Message = string.Join(", ", progSpits);
            }
            return View(result);
        }
    }
}