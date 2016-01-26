using ArghyaC.Utilities;
using ArghyaC.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ArghyaC.Controllers
{
    public class HitMissController : Controller
    {
        const string _EntriesKey = "userTries";
        const string _SecretKey = "secretNumber";

        public ActionResult Index()        
        {
            if (Session[_SecretKey] == null)
                Session.Add(_SecretKey, HitMissUtilities.CreateNumber());

            var list = Session[_EntriesKey] == null ? new List<HitMissViewModel>() : (List<HitMissViewModel>)Session[_EntriesKey];

            return View(new HmGameViewModel { Results = list });
        }

        [HttpPost]
        public ActionResult Index(string entry) //(HitMissViewModel model)
        {
            var list = Session[_EntriesKey] == null ? new List<HitMissViewModel>() : (List<HitMissViewModel>)Session[_EntriesKey];
            var secret = (List<int>)Session[_SecretKey];

            var result = HitMissUtilities.Check(secret, list, entry);
            list.Add(result);
            Session.Add(_EntriesKey, list);

            return View(new HmGameViewModel { Results = list });
        }

        [HttpPost]
        public ActionResult GiveUp() //(HitMissViewModel model)
        {
            var list = Session[_EntriesKey] == null ? new List<HitMissViewModel>() : (List<HitMissViewModel>)Session[_EntriesKey];
            var secret = (List<int>)Session[_SecretKey];

            return View("Index", new HmGameViewModel { Results = list, IsGameOver = true, Answer = string.Join("", secret) });
        }
    }
}