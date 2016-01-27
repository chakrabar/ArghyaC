using System.Collections.Generic;

namespace ArghyaC.ViewModels
{
    public class HmGameViewModel
    {
        public HmGameViewModel()
        {
            GameState = ViewModels.GameState.GameOn;
            Results = new List<HitMissViewModel>();
        }

        public GameState GameState { get; set; }
        public string Answer { get; set; }
        public List<HitMissViewModel> Results { get; set; }
    }
}