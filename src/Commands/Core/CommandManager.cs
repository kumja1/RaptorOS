using System;
using System.Collections.Generic;
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

    private Dictionary<string, Command> _commands = [];

    public void RegisterCommand(Command command) => _commands.Add(command.Name, command);

    public void RemoveCommand(string name) => _commands.Remove(name);

    public void RemoveCommand(Command command) => RemoveCommand(command.Name);

    public void Tick()
    {
        string? input = Console.ReadLine();
        if (string.IsNullOrEmpty(input))
            return;

        Logger.LogInfo($"Executing command: {input}");
        try
        {
            string[] parts = input
                .TrimStart()
                .TrimEnd()
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            Logger.LogInfo($"Processing command: {parts[0]}");
            Logger.LogInfo($"Command arguments: {string.Join(",", parts)}");

            string cmdName = parts[0];
            if (!_commands.TryGetValue(cmdName, out Command? command))
            {
                Logger.LogError($"Command {cmdName} does not exist");
                return;
            }

            Logger.LogInfo($"Found command {cmdName}");
            TokenizerResult result = CommandLineTokenizer.Tokenize(parts);
            Logger.LogInfo("Received tokenizer result");
            if (!CommandLineParser.ParseCommand(command, result))
                return;

            Logger.LogInfo($"Command {cmdName} is valid");
            var options = result.Token.Options.ToDictionary(
                x => x.Name,
                x => x.Arguments.Select(y => y.Value) as IEnumerable<object>
            );

            List<object> arguments = [.. result.Token.Arguments.Select(x => x.Value)];
            command.ExecuteCore(arguments, options);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error processing command: {ex}");
        }
    }
}
