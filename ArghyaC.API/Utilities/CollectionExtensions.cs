using System;
using System.Collections.Generic;
using System.Linq;

namespace ArghyaC.API.Utilities
{
    public static class CollectionExtensions
    {
        private static Random rn = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rn.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool IsEmpty<T>(IEnumerable<T> collection)
        {
            return collection == null || collection.Count() == 0;
        }

        public static bool IsNotEmpty<T>(IEnumerable<T> collection)
        {
            return collection != null && collection.Count() > 0;
        }
    }
}