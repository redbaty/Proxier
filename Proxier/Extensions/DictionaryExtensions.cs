using System;
using System.Collections.Generic;

namespace Proxier.Extensions
{
    internal static class DictionaryExtensions
    {
        public static TItem GetOrAdd<TKey, TItem>(this Dictionary<TKey, TItem> dictionary, TKey key, Func<TItem> factory)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];

            var newItem = factory.Invoke();
            dictionary.Add(key, newItem);
            return newItem;
        }
    }
}