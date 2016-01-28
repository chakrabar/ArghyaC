using ArghyaC.Application.Helpers;
using ArghyaC.Domain.Entities;
using ArghyaC.Infrastructure.Utilities;
using System.Web.Mvc;

namespace ArghyaC.Controllers
{
    public class CodeController : Controller
    {
        public ActionResult Index()
        {
            var code = @"
//Do NOT change the namespace &amp; class name `Program` and `Main` method signature
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeComp
{
    public class Program
    {
        public static int Main(string name)
        {
            return name.Length;
        }
    }
    //other classes if required
}";
            return View(new CodeRunResult { Code = code });
        }

        [HttpPost]
        public ActionResult Index(string code)
        {
            var timestamp = System.DateTime.Now.ToString("yyyyMMdd_hh.mm.ss.fffffff");
            var tempFolder = Server.MapPath("~/SubmittedCode/Temp_" + timestamp);

            var result = FileUtilities.TempCreateDirAndExecute<CodeRunResult>(tempFolder, 
                () => { return CodeRunner.CompileAndRun(code, tempFolder); });

            result.Code = code;
            return View(result);
        }
    }
}