using ArghyaC.Application.Helpers.Extensions;
using ArghyaC.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace ArghyaC.Utilities
{
    public class HitMissUtilities
    {
        static int[] _numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

        public static List<int> CreateNumber()
        {
            _numbers.Shuffle();
            return _numbers.Take(4).ToList();
        }

        public static HitMissViewModel Check(List<int> secretNumber, List<HitMissViewModel> history, string entry)
        {
            int hit = 0, almost = 0, repeatHit = 0;
            var hitsFor = new List<int>();

            var inputs = entry.Select(c => int.Parse(c.ToString())).ToList();

            for (int i = 0; i < inputs.Count; i++)
            {
                var digit = inputs.ElementAt(i);
                var indexInOrig = secretNumber.IndexOf(digit);
                if (indexInOrig >= 0)
                {
                    if (indexInOrig == i)
                    {
                        hit++;
                        hitsFor.Add(digit);
                        if (history.Any(h => h.HitsFor.Contains(digit)))
                            repeatHit++;
                    }
                    else
                        almost++;
                }
            }

            return new HitMissViewModel
            {
                Entry = entry,
                Hit = hit,
                Almost = almost,
                RepeatHit = repeatHit,
                HitsFor = hitsFor
            };
        }
    }
}