using ArghyaC.Application.Helpers;
using ArghyaC.Domain.Entities;
using ArghyaC.Infrastructure.Utilities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ArghyaC.Controllers
{
    public class CodeController : Controller
    {
        List<string> _instructions = new List<string>
        {
            "Find sum of all items in a square matrix. size 1 - 500.",
            "All matrix values are positive integer (or 0).",
            "Input to the method are int (size of matrix), int[] data in sequential order.",
            "e.g. (2, { 1, 2, 3, 4 }). Expected result is 10.",
            "If data supplied is insufficient, assume rest are 0.",
            "For any invalid scenario/exception, return -1."
        };

        public ActionResult Index()
        {
            var code = @"
//Do NOT change the namespace & class name `Program` and `Main` method signature
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeComp
{
    public class Program
    {
        public static int Main(int size, params int[] data)
        {
            return size; //example
        }
    }
    //other classes if required
}";
            return View(new CodeRunResult { Code = code, Instructions = _instructions });
        }

        [HttpPost]
        public ActionResult Index(string code)
        {
            var timestamp = System.DateTime.Now.ToString("yyyyMMdd_hh.mm.ss.fffffff");
            var tempFolder = Server.MapPath("~/SubmittedCode/Temp_" + timestamp);

            var result = FileUtilities.TempCreateDirAndExecute<CodeRunResult>(tempFolder, 
                () => { return CodeRunner.CompileAndRun(code, tempFolder); });

            result.Code = code;
            result.Instructions = _instructions;
            return View(result);
        }
    }
}