using ArghyaC.Infrastructure.Mongo;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using ArghyaC.API.ViewModels;
using System;

namespace ArghyaC.API.Controllers
{
    public class MongoController : ApiController
    {
        MongoRepository _repo;

        public MongoController()
        {
            _repo = new MongoRepository("bots");
        }

        [HttpGet]
        public bool InsertRestaurant(string name)
        {
            return new MongoReader().InsertRestaurant(name);
        }

        [HttpGet]
        public List<string> GetRestaurants(string name)
        {
            return new List<string>(); //new MongoReader().Get("name", name).Select < (bson => bson.ToString()).ToList();
        }

        [HttpGet]
        public BotViewModel CreateABot(string name)
        {
            
            //repo.CreateSimpleCollection("bots");
            var bot = new BotViewModel
            {
                Badges = new List<string> { "Peace", "Smart" },
                Creator = "ArghyaC",
                CreatorId = "1390",
                League = "Mentors",
                Manufactured = DateTime.UtcNow,
                Name = name
            };
            var inserted = _repo.Insert(bot);
            IEnumerable<BotViewModel> bots = new List<BotViewModel>();
            if (inserted) 
            {
                bots = _repo.Get<BotViewModel>().AsEnumerable();
            }
            return inserted ? bots.FirstOrDefault(b => b.Name == name) : null;
        }

        [HttpGet]
        public bool DeleteBot(string name)
        {
            Func<BotViewModel, string> func = b => b.Name;
            var found = _repo.Get<BotViewModel>().FirstOrDefault(b => b.Name == name);
            var deleted = false;
            if (found != null)
                deleted = _repo.DeleteById(found.Id);
            return deleted;
        }
    }
}
