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
            return View(new CodeRunResult());
        }

        [HttpPost]
        public ActionResult Index(string code)
        {
            var timestamp = System.DateTime.Now.ToString("yyyyMMdd_hh.mm.ss.fffffff");
            var tempFolder = Server.MapPath("~/SubmittedCode/Temp_" + timestamp);

            var result = FileUtilities.TempCreateDirAndExecute<CodeRunResult>(tempFolder, 
                () => { return CodeRunner.CompileAndRun(code, tempFolder); });

            return View(result);
        }
    }
}