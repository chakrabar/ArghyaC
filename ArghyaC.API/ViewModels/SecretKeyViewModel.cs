using ArghyaC.Infrastructure.Mongo;
using System.Collections.Generic;

namespace ArghyaC.API.ViewModels
{
    public class SecretKeyViewModel : MongoEntity
    {
        public SecretKeyViewModel()
        {
            Entries = new List<string>();
            History = new List<HitMissViewModel<char>>();
        }

        public string SessionKey { get; set; }
        public string SecretKey { get; set; }
        public List<string> Entries { get; set; }
        public List<HitMissViewModel<char>> History { get; set; }

        public void AddHistory(HitMissViewModel<char> currentResult)
        {
            History.Add(currentResult);
        }
    }
}