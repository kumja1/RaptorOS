using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RaptorOS.Utils.Tokenizer.Tokens;
using RaptorOS.Utils.Extensions;

namespace RaptorOS.Utils.Tokenizer;

public static class CommandLineTokenizer
{
    static class Issues
    {
        public const string InvalidCommandStart = "Command cannot start with an option. Ex: -a or --a";
        public const string CouldNotDetermineType = "Failed to determine argument.";
    }

    private static readonly Regex OptionRegex = new(@"--?[a-zA-Z0-9_-]+", RegexOptions.Compiled);
    public static TokenizerResult Tokenize(string input)
    {
        TokenizerResult result = new(new CommandToken());
        Span<string> parts = input.TrimStart().Split(' ');
        result.Token.Name = parts[0];

        Span<string> arguments = parts[1..];

        int i = 0;
        while (i < arguments.Length)
        {
            string arg = arguments[i];
            if (IsOptionToken(arg))
            {
                List<string> optionArgs = [.. arguments.TakeWhile(t => !IsOptionToken(t))];
                TokenizeOption(result, optionArgs);

                i += optionArgs.Count;
                continue;
            }

            TokenizeArgument(result, arg);
            i++;
        }

    }

    public static ArgumentToken TokenizeArgument(TokenizerResult tokenizerResult, string arg)
    {
        if (!TypeParser.TryParse(arg, out (object Value, string TypeName)? tuple))
        {
            tokenizerResult.Issues.Add(Issues.CouldNotDetermineType + $" {arg}");
            return null;
        }

        var (value, typeName) = tuple.Value;
        return new ArgumentToken
        {
            Value = value,
            TypeName = typeName
        };
    }

    public static void TokenizeOption(TokenizerResult tokenizerResult, List<string> arguments)
    {
        if (arguments.Count == 0)
        {
            tokenizerResult.Issues.Add(Issues.InvalidCommandStart);
            return;
        }

        string option = arguments[0];
        if (option.StartsWith("--"))
        {
            option = option[2..];
        }
        else
        {
            option = option[1..];
        }

        tokenizerResult.Token.Options.Add(new OptionToken
        {
            Name = option,
            Arguments = [.. arguments.Select(arg => TokenizeArgument(tokenizerResult, arg))]
        });
    }

    private static bool IsOptionToken(string str) => OptionRegex.IsMatch(str);
}