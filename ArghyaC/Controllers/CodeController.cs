using ArghyaC.Application.Compilers;
using ArghyaC.ViewModels;
using System.Web.Mvc;

namespace ArghyaC.Controllers
{
    public class CodeController : Controller
    {
        public ActionResult Index()
        {
            return View(new HmGameViewModel());
        }

        [HttpPost]
        public ActionResult Index(string code)
        {
            var result = new CSharpCompiler().Compile(code);

            return View(new HmGameViewModel());
        }
    }
}