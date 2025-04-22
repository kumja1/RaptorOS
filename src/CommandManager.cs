using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RaptorOS.Utils;

namespace RaptorOS.Commands;

public class CommandManager
{

    private static CommandManager _instance;
    public static CommandManager Instance => _instance ??= new CommandManager();

    public Dictionary<string, Command> _commands = [];

    public void RegisterCommand(Command command) => _commands.Add(command.Name, command);

    public void RemoveCommand(string name) => _commands.Remove(name);

    public void RemoveCommand(Command command) => RemoveCommand(command.Name);

    public void Tick()
    {
        string input = Console.ReadLine();
        if (string.IsNullOrEmpty(input))
            return;

        Span<string> parts = Regex.Split(input, @"\b\w+\b(?:\s+\b\w+\b)?");
        string cmdName = parts[0];

        if (!_commands.TryGetValue(cmdName, out Command command))
            Console.WriteLine($"Command {cmdName} could not found");

        foreach (string arg in parts[1..])
        {
            Span<string> argParts = arg.Split(' ');
            string name = argParts[0];

            object result = null;
            if (argParts.Length > 1)
                _ = TypeParser.TryParse(argParts[1], out result);
            
            
            Command.ArgumentDefinition definition = command.Arguments[name];
            command.ValidateArgument(name, result, definition);
        }
    }

}