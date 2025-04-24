using System;
using System.Collections.Generic;

namespace RaptorOS.Utils;

public static class TypeParser
{
    private static readonly Dictionary<
        string,
        Func<string, (bool Success, object? Value)>
    > _parsers = new()
    {
        ["bool"] = s => bool.TryParse(s, out var v) ? (true, v) : (false, null),
        ["int"] = s => int.TryParse(s, out var v) ? (true, v) : (false, null),
        ["long"] = s => long.TryParse(s, out var v) ? (true, v) : (false, null),
        ["float"] = s => float.TryParse(s, out var v) ? (true, v) : (false, null),
        ["double"] = s => double.TryParse(s, out var v) ? (true, v) : (false, null),
        ["decimal"] = s => decimal.TryParse(s, out var v) ? (true, v) : (false, null),
        ["Guid"] = s => Guid.TryParse(s, out var v) ? (true, v) : (false, null),
        ["string"] = s => (true, s),
    };

    public static bool TryParse(string input, out (object? Value, string TypeName) result)
    {
        foreach (var (typeName, parser) in _parsers)
        {
            var (success, value) = parser(input);
            if (success)
            {
                result = (value, typeName);
                return true;
            }
        }

        result = default;
        return false;
    }

    public static bool TryParse<T>(string input, out object? result)
    {
        string typeName = typeof(T).Name;
        result = null;

        if (!_parsers.TryGetValue(typeName, out var parser))
            return false;

        var (success, value) = parser(input);
        if (success)
            result = value;

        return result != null;
    }
}
