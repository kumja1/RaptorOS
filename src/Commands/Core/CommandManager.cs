using System;
using System.Collections.Generic;
using System.Linq;
using RaptorOS.Commands.Core.Models;
using RaptorOS.Utils;
using RaptorOS.Utils.Extensions;
using RaptorOS.Utils.Tokenizer;
using RaptorOS.Utils.Tokenizer.Tokens;

namespace RaptorOS.Commands.Core;

public class CommandManager
{
    private static CommandManager? _instance;
    public static CommandManager Instance => _instance ??= new CommandManager();

    public Dictionary<string, Command> _commands = [];

    public void RegisterCommand(Command command) => _commands.Add(command.Name, command);

    public void RemoveCommand(string name) => _commands.Remove(name);

    public void RemoveCommand(Command command) => RemoveCommand(command.Name);

    public void Tick()
    {
        string? input = Console.ReadLine();
        if (string.IsNullOrEmpty(input))
            return;

        Span<string> parts = input.TrimStart().TrimEnd().Split(' ');
        if (parts.Length == 0 || string.IsNullOrEmpty(parts[0]))
            return;

        string cmdName = parts[0];
        if (!_commands.TryGetValue(cmdName, out Command? command))
            Logger.LogError($"Command {cmdName} could not found");

        TokenizerResult result = CommandLineTokenizer.Tokenize(parts);
        if (!ValidateCommand(command!, result))
            return;

        Dictionary<string, IEnumerable<object?>> options = result.Token.Options.ToDictionary(
            x => x.Name,
            x => x.Arguments.Select(y => y.Value)
        );

        List<object> arguments = [.. result.Token.Arguments.Select(x => x.Value)];
        command!.ExecuteCore(arguments, options);
    }

    private bool ValidateCommand(Command command, TokenizerResult result)
    {
        if (result.Issues.Count > 0)
        {
            Logger.LogError("Error while parsing command arguments:");
            foreach (string issue in result.Issues)
                Logger.LogError(issue);

            return false;
        }

        if (command.Arguments.Count > 0 && result.Token.Arguments.Length == 0)
        {
            Logger.LogError($"Command {command.Name} requires arguments");
            return false;
        }
        else if (command.Arguments.Count == 0 && result.Token.Arguments.Length > 0)
        {
            Logger.LogError($"Command {command.Name} does not accept arguments");
            return false;
        }

        int requiredArgLength = command.Arguments.Count(x => x.IsRequired);
        if (result.Token.Arguments.Length < requiredArgLength)
        {
            Logger.LogError($"Command {command.Name} requires {requiredArgLength} arguments");
            return false;
        }

        for (int i = 0; i < result.Token.Arguments.Length; i++)
        {
            bool hasDefinition = command.Arguments.TryGetValue(i, out ArgumentDefinition def);
            if (!hasDefinition)
            {
                Logger.LogError($"Argument {i} is not a valid argument for command {command.Name}");
                return false;
            }

            ArgumentToken token = result.Token.Arguments[i];
            if (def.TypeName != token.TypeName)
            {
                Logger.LogError($"Argument {i} is not of type {def.TypeName}");
                return false;
            }
        }

        int requiredOptionLength = command.Options.Count(x => x.Value.IsRequired);
        if (result.Token.Options.Length < requiredOptionLength)
        {
            Logger.LogError($"Command {command.Name} requires {requiredOptionLength} options");
            return false;
        }

        foreach (OptionToken token in result.Token.Options)
        {
            bool hasDefinition =
                command.Options.TryGetValue(token.Name, out OptionDefinition def)
                || command.Options.Values.Any(x => x.Aliases.Contains(token.Name), out def);

            if (!hasDefinition)
            {
                Logger.LogError(
                    $"Option {token.Name} is not a valid option for command {command.Name}"
                );
                return false;
            }

            if (def.ValueDefinition.IsRequired && token.Arguments.Length == 0)
            {
                Logger.LogError($"Option {token.Name} requires a value");
                return false;
            }

            if (
                token.Arguments.Length > 0
                && !token.Arguments.All(x => x.TypeName == def.ValueDefinition.TypeName)
            )
            {
                Logger.LogError(
                    $"Option {token.Name} is not of type {def.ValueDefinition.TypeName}"
                );
                return false;
            }
        }

        return true;
    }
}
