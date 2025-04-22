using System;
using System.Collections.Generic;

namespace RaptorOS.Utils.Extensions;

public static class SpanExtensions
{

    public static IEnumerable<T> TakeWhile<T>(this Span<T> span, Func<T, bool> condition)
    {
        foreach (T item in span)
        {
            if (condition(item))
                yield return item;
            else
                break;
        }
    }
}