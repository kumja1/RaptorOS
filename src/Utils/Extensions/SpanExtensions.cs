using System;
using System.Collections.Generic;

namespace RaptorOS.Utils.Extensions;

public static class SpanExtensions
{
    public static IEnumerable<T> TakeWhile<T>(this Span<T> span, Func<T, bool> condition)
    {
        var result = new List<T>();

        for (int i = 0; i < span.Length; i++)
        {
            T item = span[i];
            if (condition(item))
                result.Add(item);
            else
                break;
        }
        return result;
    }

    public static IEnumerable<T> Skip<T>(this Span<T> span, int count)
    {
        var result = new List<T>();
        for (int i = count; i < span.Length; i++)
            result.Add(span[i]);
        return result;
    }
}
