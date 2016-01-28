using ArghyaC.Application.Helpers;
using ArghyaC.Domain.Entities;
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
            var result = CodeRunner.CompileAndRun(code);

            return View(result);
        }
    }
}