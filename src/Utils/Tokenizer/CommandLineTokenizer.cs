using System;
using System.Collections.Generic;
using RaptorOS.Utils.Extensions;
using RaptorOS.Utils.Tokenizer.Tokens;

namespace RaptorOS.Utils.Tokenizer;

public static class CommandLineTokenizer
{
    public static TokenizerResult Tokenize(string[] parts)
    {
        try
        {
            Logger.LogInfo($"Tokenizing command line: [{string.Join(' ', parts)}]");

            TokenizerResult result = new(new CommandToken(name: parts[0]));
            Logger.LogInfo($"Created command token: {parts[0]}");


            string[] arguments = [..parts.Skip(1)];
            Logger.LogInfo($"Arguments: {string.Join(',', arguments)}");

            if (!arguments.Any())
                return result;

            Logger.LogInfo($"Arguments length: {arguments.Length}");
            int i = 0;
            while (i < arguments.Length)
            {
                string arg = arguments[i];
                Logger.LogInfo($"Processing argument: {arg}");

                if (IsOptionToken(arg))
                {
                    try
                    {
                        Logger.LogInfo($"Detected option token: {arg}");

                        string[] optionArgs =
                        [
                            arg, .. arguments.Skip(i + 1).TakeWhile(t =>
                            {
                                Logger.LogInfo(
                                    $"Is Arg Empty: {string.IsNullOrEmpty(t)}. Is Arg Whitespace: {string.IsNullOrWhiteSpace(t)}");
                                return !string.IsNullOrWhiteSpace(t) && !IsOptionToken(t);
                            })
                        ];
                        Logger.LogInfo($"Collected option arguments: [{string.Join(", ", optionArgs)}]");

                        if (TryTokenizeOption(result.Issues, optionArgs, out OptionToken optionToken))
                        {
                            result.Token.Options.Add(optionToken);
                            Logger.LogInfo(
                                $"Added option token: --{optionToken.Name} with {optionToken.Arguments.Count} args");
                        }

                        i += optionArgs.Length + 1;
                        continue;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed to parse option token: {arg}. Error:{e}");
                    }
                }

                if (TryTokenizeArgument(result.Issues, arg, out ArgumentToken argToken))
                {
                    result.Token.Arguments.Add(argToken);
                    Logger.LogInfo($"Added argument token: {argToken.TypeName} = {argToken.Value}");
                }

                i++;
            }

            Logger.LogInfo("Tokenization complete.");
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Tokenization failed with exception: {ex}");
            return new TokenizerResult(default);
        }
    }

    public static bool TryTokenizeArgument(
        List<string> issues,
        string arg,
        out ArgumentToken token
    )
    {
        token = default;
        try
        {
            Logger.LogInfo($"Attempting to parse argument: {arg}");

            if (!TypeParser.TryParse(arg, out (object? Value, string TypeName) tuple))
            {
                string error = $"Failed to determine argument type for argument {arg}.";
                issues.Add(error);
                Logger.LogError(error);
                return false;
            }

            var (value, typeName) = tuple;
            token = new ArgumentToken(typeName, value);
            Logger.LogInfo($"Parsed argument: type={typeName}, value={value}");
            return true;
        }
        catch (Exception ex)
        {
            string error = $"Exception while parsing argument '{arg}': {ex}";
            issues.Add(error);
            Logger.LogError(error);
            return false;
        }
    }

    public static bool TryTokenizeOption(
        List<string> issues,
        string[] arguments,
        out OptionToken token
    )
    {
        token = default;
        try
        {
            string option = arguments[0];
            Logger.LogInfo($"Parsing option token: {option}");

            bool isLongOption = option.StartsWith("--");

            EquatableArray<ArgumentToken> argumentTokens = [];
            for (int i = 1; i < arguments.Length; i++)
            {
                Logger.LogInfo($"Parsing option argument: {arguments[i]}");

                if (!TryTokenizeArgument(issues, arguments[i], out ArgumentToken argToken)) continue;
                
                argumentTokens.Add(argToken);
                Logger.LogInfo($"Parsed option argument: {argToken.TypeName} = {argToken.Value}");
            }

            token = new OptionToken(isLongOption ? option[2..] : option[1..], argumentTokens);
            Logger.LogInfo(
                $"Created option token: {(isLongOption ? "--" : "-")}{token.Name} with {argumentTokens.Capacity} argument(s)");
            return true;
        }
        catch (Exception ex)
        {
            string error = $"Exception while parsing option '{(arguments.Length > 0 ? arguments[0] : "null")}': {ex}";
            issues.Add(error);
            
            Logger.LogError(error);
            return false;
        }
    }

    private static bool IsOptionToken(string str)
    {
        try
        {
            Logger.LogInfo($"Checking if '{str}' is an option token");

            if (string.IsNullOrWhiteSpace(str))
                return false;

            if (str.StartsWith("--"))
                str = str[2..];
            else if (str.StartsWith("-"))
                str = str[1..];
            else
                return false;

            if (str.Length == 0)
                return false;

            foreach (char c in str)
            {
                if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
                    return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Exception in IsOptionToken for input '{str}': {ex}");
            return false;
        }
    }
}