
namespace ArghyaC.API.ViewModels
{
    public class HitMissBase
    {
        public bool IsValid { get; set; }
        public string Entry { get; set; }
        public int Hit { get; set; }
        public int Close { get; set; }
        public int RepeatHit { get; set; }
        public int RepeatClose { get; set; }
        public int TryCount { get; set; }
        public string Message { get; set; }

        public static HitMissBase Map(HitMissBase o)
        {
            return new HitMissBase
            {
                Close = o.Close,
                Entry = o.Entry,
                Hit = o.Hit,
                IsValid = o.IsValid,
                Message = o.Message,
                RepeatClose = o.RepeatClose,
                RepeatHit = o.RepeatHit
            };
        }
    }
}