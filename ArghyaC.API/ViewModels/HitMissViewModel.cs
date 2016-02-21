using System.Collections.Generic;

namespace ArghyaC.API.ViewModels
{
    public class HitMissViewModel<T> : HitMissBase
    {
        public HitMissViewModel()
        {
            HitsFor = new List<T>();
            CloseFor = new List<T>();
        }

        public List<T> HitsFor { get; set; }
        public List<T> CloseFor { get; set; }
    }
}
