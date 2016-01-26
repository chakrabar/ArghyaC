using System.Web.Mvc;

namespace ArghyaC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About - ArghyaC on Azure";

            return View();
        }
    }
}