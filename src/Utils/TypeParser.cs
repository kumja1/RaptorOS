using System;
using System.Collections.Generic;
using System.Globalization;

namespace RaptorOS.Utils;

public static class TypeParser
{
    private static readonly List<Func<string, (bool Success, object Value, string TypeName)>> _parsers =
    [
        s => bool.TryParse(s, out var v) ? (true, v, "bool") : (false, null, string.Empty),
        s => int.TryParse(s, out var v) ? (true, v, "int") : (false, null, string.Empty),
        s => long.TryParse(s, out var v) ? (true, v, "long") : (false, null, string.Empty),
        s => float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? (true, v, "float") : (false, null, string.Empty),
        s => double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? (true, v, "double") : (false, null, string.Empty),
        s => decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? (true, v, "decimal") : (false, null, string.Empty),
        s => DateTime.TryParse(s, out var v) ? (true, v, "DateTime") : (false, null, string.Empty),
        s => Guid.TryParse(s, out var v) ? (true, v, "Guid") : (false, null, string.Empty),
        s => (true, s, "string")
    ];

    public static bool TryParse(string input, out (object? Value, string TypeName)? result)
    {
        foreach (var parser in _parsers)
        {
            var (success, value, typeName) = parser(input);
            if (success)
            {
                result = (value, typeName);
                return true;
            }
        }

        result = null;
        return false;
    }
}
