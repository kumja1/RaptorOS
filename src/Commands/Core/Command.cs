using System.Collections.Generic;
using RaptorOS.Commands.Core.Models;
using RaptorOS.Utils;

namespace RaptorOS.Commands.Core;

public class Command
{
    public string Name { get; protected set; }

    protected string Usage { get; set; }

    protected string Description { get; init; }

    public readonly Dictionary<string, OptionDefinition> Options = [];

    public readonly List<ArgumentDefinition> Arguments = [];

    protected virtual void Execute(
        List<object> args,
        Dictionary<string, IEnumerable<object?>> options
    ) { }

    public void ExecuteCore(List<object> args, Dictionary<string, IEnumerable<object>> options)
    {
        if (options.ContainsKey("--help"))
        {
            ShowHelp();
            return;
        }

        Execute(args, options);
    }

    private void ShowHelp()
    {
        Logger.Log($"Description: \n\t{Description} ");
        Logger.Log($"Usage: \n\t{Usage}");
        Logger.Log($"Options:");
        foreach (OptionDefinition option in Options.Values)
            Logger.Log(option.ToString());
    }

    protected Command()
    {
        Options["help"] = new OptionDefinition
        {
            IsRequired = false,
            Description = "Shows help message",
            Aliases = ["h"],
        };

        Name ??= GetType().Name.Replace("Command", string.Empty).ToLowerInvariant();
        Usage ??= $"{Name} [options] {string.Join(" ", Arguments)}";
    }
}
