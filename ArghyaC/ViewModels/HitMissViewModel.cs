using System.Collections.Generic;

namespace ArghyaC.ViewModels
{
    public class HitMissViewModel
    {
        public HitMissViewModel()
        {
            HitsFor = new List<int>();
        }

        public string Entry { get; set; }
        public int Hit { get; set; }
        public int Almost { get; set; }
        public int RepeatHit { get; set; }
        public List<int> HitsFor { get; set; }
    }
}