using System.Collections.Generic;

namespace ArghyaC.ViewModels
{
    public class HmGameViewModel
    {
        public bool IsGameOver { get; set; }
        public List<HitMissViewModel> Results { get; set; }
    }
}