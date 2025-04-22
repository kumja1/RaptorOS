using System.Collections.Generic;

namespace RaptorOS.Commands;

public class Command
{
    public struct ArgumentDefinition
    {
        public bool IsRequired;
    }

    public string Name { get; protected set; }

    public string Usage { get; protected set; }

    public string Description { get; protected set; }

    public readonly Dictionary<string, ArgumentDefinition> Arguments = [];

    public virtual void ValidateArgument(string argName, object value, ArgumentDefinition definition)
    { }

    public virtual void Execute(){}

    protected Command()
    {
        Arguments["--help"] = new ArgumentDefinition
        {
            IsRequired = true,
            
        };
    }
}
