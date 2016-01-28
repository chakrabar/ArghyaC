using ArghyaC.Application.Helpers;
using ArghyaC.Domain.Entities;
using System.IO;
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

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            var result = CodeRunner.CompileAndRun(code, tempFolder);

            if (Directory.Exists(tempFolder))
            {
                foreach (var file in Directory.EnumerateFiles(tempFolder))
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.Delete(file);
                }
                Directory.Delete(tempFolder, true);
            }

            return View(result);
        }
    }
}