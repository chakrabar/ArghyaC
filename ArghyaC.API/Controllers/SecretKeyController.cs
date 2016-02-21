using ArghyaC.API.Utilities;
using ArghyaC.Infrastructure.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ArghyaC.API.ViewModels;

namespace ArghyaC.API.Controllers
{
    public class SecretKeyController : ApiController
    {
        MongoRepository _repo;

        public SecretKeyController()
        {
            _repo = new MongoRepository("secretkeys");
        }

        [HttpGet]
        public string GetNewSecretKey()
        {
            return new string(HitMissAlphaUtilities.CreateNewSecretKey().ToArray());
        }

        [HttpGet]
        [Route("secretkey/validate/{sessionKey}/{entry}")]
        public HitMissBase Validate(string entry, string sessionKey)
        {
            var data = _repo.Get<SecretKeyViewModel>().FirstOrDefault(d => d.SessionKey == sessionKey);
            HitMissViewModel<char> hitMiss = new HitMissViewModel<char>();
            if (data == null)
            {
                var secretKey = new string(HitMissAlphaUtilities.CreateNewSecretKey().ToArray());
                hitMiss = HitMissAlphaUtilities.Check(secretKey.Select(c => c).ToList(), new List<HitMissViewModel<char>>(), entry);
                data = new SecretKeyViewModel();
                data.SecretKey = secretKey;
                data.SessionKey = sessionKey;
                data.Entries.Add(entry);
                data.AddHistory(hitMiss);

                _repo.Insert<SecretKeyViewModel>(data);
            }
            else
            {
                hitMiss = HitMissAlphaUtilities.Check(data.SecretKey.Select(c => c).ToList(), data.History, entry);
                data.Entries.Add(entry);
                data.AddHistory(hitMiss);
                _repo.Update(data);
            }
            var result = HitMissBase.Map(hitMiss);
            result.TryCount = data.Entries.Count;
            return result;
        }
    }
}
