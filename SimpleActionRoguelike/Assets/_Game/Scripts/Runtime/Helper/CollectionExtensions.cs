using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Helper
{
    public static class CollectionExtension
    {
        public static void Replace<TKey, TValue>(this Dictionary<TKey, TValue> original, TKey key, TValue value)
        {
            if (original.ContainsKey(key))
            {
                original.Remove(key);
            }
            original.Add(key, value);
        }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> original,
            TKey key, TValue defaultValue)
        {
            if (original.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static T GetRandomFromList<T>(this List<T> list)
        {
            T random = list[Random.Range(0, list.Count)];
            return random;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}