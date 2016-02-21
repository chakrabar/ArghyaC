using ArghyaC.Infrastructure.Mongo;
using System;
using System.Collections.Generic;

namespace ArghyaC.API.ViewModels
{
    public class BotViewModel : MongoEntity
    {
        public string Name { get; set; }
        public string League { get; set; }
        public DateTime Manufactured { get; set; }
        public string Creator { get; set; }
        public string CreatorId { get; set; }
        public IList<string> Badges { get; set; }
    }
}