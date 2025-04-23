using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RaptorOS.Utils.Extensions;
using RaptorOS.Utils.Tokenizer.Tokens;

namespace RaptorOS.Utils.Tokenizer;

public static class CommandLineTokenizer
{
    private static readonly Regex OptionRegex = new(@"--?[a-zA-Z0-9_-]+", RegexOptions.RightToLeft);

    public static TokenizerResult Tokenize(Span<string> parts)
    {
        TokenizerResult result = new(new CommandToken());
        result.Token.Name = parts[0];
        Span<string> arguments = parts[1..];

        int i = 0;
        while (i < arguments.Length)
        {
            string arg = arguments[i];
            if (IsOptionToken(arg))
            {
                Span<string> optionArgs = [.. arguments.Skip(i).TakeWhile(t => !IsOptionToken(t))];
                if (TryTokenizeOption(result.Issues, optionArgs, out OptionToken optionToken))
                    result.Token.Options.Add(optionToken);

                i += optionArgs.Length;
                continue;
            }

            if (TryTokenizeArgument(result.Issues, arg, out ArgumentToken argToken))
                result.Token.Arguments.Add(argToken);

            i++;
        }
        return result;
    }

    public static bool TryTokenizeArgument(List<string> issues, string arg, out ArgumentToken token)
    {
        token = default;

        if (!TypeParser.TryParse(arg, out (object? Value, string TypeName) tuple))
        {
            issues.Add($"Failed to determine argument type for argument {arg}.");
            return false;
        }

        var (value, typeName) = tuple;
        token = new ArgumentToken(typeName, value);
        return true;
    }

    public static bool TryTokenizeOption(
        List<string> issues,
        Span<string> arguments,
        out OptionToken token
    )
    {
        token = default;

        if (arguments.Length == 0)
            return false;

        string option = arguments[0];
        bool isLongOption = option.StartsWith("--");

        EquatableArray<ArgumentToken> parsedArgs = [];
        for (int i = 1; i < arguments.Length; i++)
        {
            if (TryTokenizeArgument(issues, arguments[i], out ArgumentToken argToken))
                parsedArgs.Add(argToken);
        }

        token = new OptionToken
        {
            Name = isLongOption ? option[2..] : option[1..],
            IsShortHand = !isLongOption,
            Arguments = parsedArgs,
        };

        return true;
    }

    public static bool IsOptionToken(string str) => OptionRegex.IsMatch(str);
}
