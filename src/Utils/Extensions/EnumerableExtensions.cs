using System;
using System.Collections.Generic;

namespace RaptorOS.Utils.Extensions;

public static class EnumerableExtensions
{
    public static bool TryGetValue<T>(this List<T> list, int index, out T? value)
    {
        if (index < 0 || index >= list.Count)
        {
            value = default;
            return false;
        }
        value = list[index];
        return true;
    }

    public static bool Any<T>(this IEnumerable<T> enumerable, Func<T, bool> condition, out T? value)
    {
        foreach (T item in enumerable)
        {
            if (condition(item))
            {
                value = item;
                return true;
            }
        }

        value = default;
        return false;
    }
}
