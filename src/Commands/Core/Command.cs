using System;
using System.Collections.Generic;
using RaptorOS.Commands.Core.Models;
using RaptorOS.Utils;

namespace RaptorOS.Commands.Core;

public class Command
{
    public string Name { get; protected set; }

    public string Usage { get; protected set; }

    public string Description { get; protected set; }

    public readonly Dictionary<string, OptionDefinition> Options = [];

    public readonly List<ArgumentDefinition> Arguments = [];

    public virtual void Execute(
        List<object> args,
        Dictionary<string, IEnumerable<object?>> options
    ) { }

    public void ExecuteCore(List<object> args, Dictionary<string, IEnumerable<object?>> options)
    {
        if (options.ContainsKey("--help"))
        {
            ShowHelp();
            return;
        }

        Execute(args, options);
    }

    public virtual void ShowHelp()
    {
        Logger.Log($"Description: \n\t{Description} ");
        Logger.Log($"Usage: \n\t{Usage}");
        Logger.Log($"Options:");
        foreach (OptionDefinition option in Options.Values)
            Logger.Log(option.ToString());
    }

    protected Command()
    {
        Options["--help"] = new OptionDefinition
        {
            IsRequired = false,
            Description = "Shows help message",
            Aliases = ["-h"],
        };

        Name ??= GetType().Name.Replace("Command", string.Empty).ToLowerInvariant();
        Usage ??= $"{Name} [options] {string.Join(" ", Arguments)}";
    }
}
