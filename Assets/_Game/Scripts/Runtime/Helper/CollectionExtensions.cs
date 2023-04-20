using System.Collections.Generic;

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
    }
}