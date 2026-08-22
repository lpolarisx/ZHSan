
using System.Collections.Generic;

namespace Extensions;

public static class DictionaryExtensions
{
    public static int GetNewId<TValue>(this Dictionary<int, TValue> dictionary)
    {
        var id = 1;

        if (dictionary == null || dictionary.Count == 0) return id;

        foreach (var key in dictionary.Keys)
        {
            if (key > id)
                id = key;
        }

        return ++id;
    }
}