using System;
using System.Collections.Generic;
using RaptorOS.Commands.Core;
using RaptorOS.Commands.Core.Models;
using RaptorOS.Utils.Extensions;
using RaptorOS.Utils.Tokenizer;
using RaptorOS.Utils.Tokenizer.Tokens;

namespace RaptorOS.Utils;

public static class CommandLineParser
{
    public static bool ParseCommand(Command command, TokenizerResult result)
    {
        Logger.LogInfo($"Parsing command: {command.Name}");
        Logger.LogInfo(
            $"Token result contains {result.Token.Options.Count} options and {result.Token.Arguments.Count} arguments"
        );

        try
        {
            if (result.Issues.Count <= 0)
            {
                Logger.LogInfo("No issues found in tokenization, parsing options and arguments...");
                bool optionsResult = ParseCommandOptions(command, result);
                Logger.LogInfo($"Options parsing result: {optionsResult}");

                bool argumentsResult = ParseCommandArguments(command, result);
                Logger.LogInfo($"Arguments parsing result: {argumentsResult}");

                return optionsResult && argumentsResult;
            }

            Logger.LogError(
                $"Error while parsing command arguments. Found {result.Issues.Count} issues:"
            );

            foreach (string issue in result.Issues)
                Logger.LogError(issue);

            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Exception during ParseCommand: {ex.Message}");
            return false;
        }
    }

    public static bool ParseCommandOptions(Command command, TokenizerResult result)
    {
        Logger.LogInfo($"Parsing command options for: {command.Name}");
        Logger.LogInfo(
            $"Command has {command.Options.Count} defined options, processing {result.Token.Options.Count} provided options"
        );

        try
        {
            int requiredOptionLength = command.Options.Count(x => x.Value.IsRequired);
            Logger.LogInfo(
                $"Required option count: {requiredOptionLength}, Provided options: {result.Token.Options.Count}"
            );

            if (result.Token.Options.Count < requiredOptionLength)
            {
                Logger.LogError(
                    $"Command {command.Name} requires {requiredOptionLength} options, but only {result.Token.Options.Count} provided"
                );
                HashSet<string> optionNames = [.. result.Token.Options.Select(x => x.Name)];
                foreach (var kvp in command.Options)
                {
                    if (!kvp.Value.IsRequired || optionNames.Contains(kvp.Key))
                        continue;

                    Logger.LogError($"Missing required option: {kvp.Key}");
                }

                return false;
            }

            for (int i = 0; i < result.Token.Options.Count; i++)
            {
                OptionToken token = result.Token.Options[i];
                Logger.LogInfo(
                    $"Parsing option token: {token.Name} with {token.Arguments.Count} arguments"
                );

                bool hasDefinition =
                    command.Options.TryGetValue(token.Name, out OptionDefinition def)
                    || command.Options.Values.Any(x => x.Aliases.Contains(token.Name), out def);

                Logger.LogInfo($"Option {token.Name} definition found: {hasDefinition}");

                if (!hasDefinition)
                {
                    Logger.LogError($"Option {token.Name} is not valid for command {command.Name}");
                    return false;
                }

                Logger.LogInfo(
                    $"Option {token.Name} value required: {def.ValueDefinition.IsRequired}, values provided: {token.Arguments.Count}"
                );
                if (def.ValueDefinition.IsRequired && token.Arguments.Count == 0)
                {
                    Logger.LogError($"Option {token.Name} requires a value but none was provided");
                    return false;
                }

                Logger.LogInfo("Getting option index");
                int index = result.Token.Options.IndexOf(token);
                Logger.LogInfo(
                    $"Option {token.Name} is at index {index} of {result.Token.Options.Count}"
                );

                if (token.Arguments.Count > def.ArgumentLength)
                {
                    Logger.LogWarning(
                        $"Option {token.Name} has more arguments ({token.Arguments.Count}) than allowed ({def.ArgumentLength})"
                    );
                    Logger.LogInfo(
                        $"Checking if option is last in sequence (index {index} vs length {result.Token.Options.Count})"
                    );
                    if (index == result.Token.Options.Count - 1)
                    {
                        Logger.LogInfo(
                            $"Adding {token.Arguments.Count - def.ArgumentLength} extra arguments from option {token.Name} to command arguments"
                        );

                        EquatableArray<ArgumentToken> extraArgs = token
                            .Arguments.Skip(def.ArgumentLength)
                            .ToArray();

                        result.Token.Arguments.AddRange(extraArgs);
                        token.Arguments.RemoveRange(extraArgs);
                    }
                    else
                    {
                        Logger.LogError(
                            $"Option {token.Name} has too many arguments and is not last option"
                        );
                        return false;
                    }
                }

                if (token.Arguments.Count <= 0)
                    continue;

                Logger.LogInfo(
                    $"Validating argument types for option {token.Name}. Expected type: {def.ValueDefinition.TypeName}"
                );

                if (token.Arguments.All(x => x.TypeName == def.ValueDefinition.TypeName))
                    continue;

                Logger.LogError(
                    $"Option {token.Name} argument types do not match expected type {def.ValueDefinition.TypeName}"
                );

                foreach (var arg in token.Arguments)
                    Logger.LogError(
                        $"Found argument of type {arg.TypeName} with value {arg.Value}"
                    );

                return false;
            }

            Logger.LogInfo($"Successfully parsed all options for command {command.Name}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Exception during ParseCommandOptions: {ex.Message}");
            return false;
        }
    }

    public static bool ParseCommandArguments(Command command, TokenizerResult result)
    {
        Logger.LogInfo($"Parsing command arguments for: {command.Name}");
        Logger.LogInfo(
            $"Command has {command.Arguments.Count} defined arguments, processing {result.Token.Arguments.Count} provided arguments"
        );

        try
        {
            switch (command.Arguments.Count)
            {
                case > 0 when result.Token.Arguments.Count == 0:
                    Logger.LogError(
                        $"Command {command.Name} requires arguments but none were provided"
                    );
                    return false;
                case 0 when result.Token.Arguments.Count > 0:
                    Logger.LogError(
                        $"Command {command.Name} does not accept any arguments but {result.Token.Arguments.Count} were provided"
                    );
                    return false;
            }

            int requiredArgLength = command.Arguments.Count(x => x.IsRequired);
            Logger.LogInfo(
                $"Required argument count: {requiredArgLength}, Provided: {result.Token.Arguments.Count}"
            );

            if (result.Token.Arguments.Count < requiredArgLength)
            {
                Logger.LogError(
                    $"Command {command.Name} requires {requiredArgLength} arguments, but only {result.Token.Arguments.Count} provided"
                );
                return false;
            }

            for (int i = 0; i < result.Token.Arguments.Count; i++)
            {
                Logger.LogInfo($"Validating argument at position {i}");
                bool hasDefinition =
                    command.Arguments.Count > 1
                        ? command.Arguments.TryGetValue(i, out ArgumentDefinition def)
                        : command.Arguments.TryGetValue(0, out def);

                Logger.LogInfo($"Argument {i} definition found: {hasDefinition}");

                if (!hasDefinition)
                {
                    Logger.LogError(
                        $"Argument {i} is not a valid argument for command {command.Name}"
                    );
                    return false;
                }

                ArgumentToken token = result.Token.Arguments[i];
                Logger.LogInfo(
                    $"Validating argument {i} type. Expected: {def.TypeName}, Found: {token.TypeName}"
                );

                if (def.TypeName != token.TypeName)
                {
                    Logger.LogError(
                        $"Argument {i} type mismatch. Expected {def.TypeName}, found {token.TypeName}"
                    );
                    Logger.LogError($"Argument value was: {token.Value}");
                    return false;
                }
            }

            Logger.LogInfo($"Successfully parsed all arguments for command {command.Name}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Exception during ParseCommandArguments: {ex.Message}");
            return false;
        }
    }
}
