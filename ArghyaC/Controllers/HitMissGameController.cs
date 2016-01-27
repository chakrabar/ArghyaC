using ArghyaC.Utilities;
using ArghyaC.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ArghyaC.Controllers
{
    public class HitMissGameController : Controller
    {
        const string _UserDataKey = "userData";
        const string _SecretKey = "secretNumber";

        public ActionResult Index()        
        {
            if (Session[_SecretKey] == null)
                Session.Add(_SecretKey, HitMissUtilities.CreateNumber());

            var gameData = Session[_UserDataKey] == null ? new HmGameViewModel() : (HmGameViewModel)Session[_UserDataKey];

            return View(gameData);
        }

        [HttpPost]
        public ActionResult Index(string entry)
        {
            var gameData = Session[_UserDataKey] == null ? new HmGameViewModel() : (HmGameViewModel)Session[_UserDataKey];
            var secret = (List<int>)Session[_SecretKey];

            var result = HitMissUtilities.Check(secret, gameData.Results, entry);
            if (gameData.GameState == GameState.GameOn)
                gameData.Results.Add(result);

            if (result.Hit == 4)
            {
                gameData.Answer = string.Join("", secret);
                gameData.GameState = GameState.UserWon;
            }
            Session.Add(_UserDataKey, gameData);

            return View(gameData);
        }

        [HttpPost]
        public ActionResult GiveUp()
        {
            var gameData = Session[_UserDataKey] == null ? new HmGameViewModel() : (HmGameViewModel)Session[_UserDataKey];
            var secret = (List<int>)Session[_SecretKey];

            gameData.Answer = string.Join("", secret);
            gameData.GameState = GameState.GaveUp;
            Session.Add(_UserDataKey, gameData);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult NewGame()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}