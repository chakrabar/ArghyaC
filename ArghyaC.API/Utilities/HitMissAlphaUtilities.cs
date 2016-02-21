using ArghyaC.API.ViewModels;
using ArghyaC.Infrastructure.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace ArghyaC.API.Utilities
{
    public static class HitMissAlphaUtilities
    {
        const string _AllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@$%^_{}[]";
        static List<char> _allChars;

        static HitMissAlphaUtilities()
        {
            var configChars = ConfigUtilities.GetAppSettings("CharSet");
            _allChars = string.IsNullOrWhiteSpace(configChars) ? _AllChars.Select(c => c).ToList() : configChars.Select(c => c).ToList();
        }

        public static List<char> CreateNewSecretKey(int size = 10)
        {
            var configSecretKey = ConfigUtilities.GetAppSettings("SecretKey");
            if (string.IsNullOrWhiteSpace(configSecretKey))
            {
                _allChars.Shuffle();
                return _allChars.Take(size).ToList();
            }
            return configSecretKey.Select(c => c).ToList();
        }

        private static bool IsValidEntry(string entry, int size = 10)
        {
            if (entry == null || entry.Length != size)
                return false;
            var uniqueChars = entry.Select(c => c).Distinct();
            if (uniqueChars.Count() != size)
                return false;
            if (uniqueChars.Any(uc => !_allChars.Contains(uc)))
                return false;
            return true;
        }

        public static HitMissViewModel<char> Check(List<char> secretKey, List<HitMissViewModel<char>> history, string entry)
        {
            if (!IsValidEntry(entry, secretKey.Count))
                return new HitMissViewModel<char> { IsValid = false };

            int hit = 0, close = 0, repeatHit = 0, repeatClose = 0;
            var hitsFor = new List<char>();
            var closesFor = new List<char>();

            var inputs = entry.Select(c => c).ToList();

            for (int i = 0; i < inputs.Count; i++)
            {
                var currentChar = inputs.ElementAt(i);
                var indexInOrig = secretKey.IndexOf(currentChar);
                if (indexInOrig >= 0)
                {
                    if (indexInOrig == i)
                    {
                        hit++;
                        hitsFor.Add(currentChar);
                        if (history.Any(h => h.HitsFor.Contains(currentChar)))
                            repeatHit++;
                    }
                    else
                    {
                        close++;
                        closesFor.Add(currentChar);
                        if (history.Any(h => h.CloseFor.Contains(currentChar)))
                            repeatClose++;
                    }
                }
            }

            return new HitMissViewModel<char>
            {
                IsValid = true,
                Entry = entry,
                Hit = hit,
                Close = close,
                RepeatHit = repeatHit,
                RepeatClose = repeatClose,
                HitsFor = hitsFor,
                CloseFor = closesFor
            };
        }
    }
}