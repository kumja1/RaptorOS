using System.Collections.Generic;
using RaptorOS.Commands.Core;
using RaptorOS.Commands.Core.Models;
using RaptorOS.Utils;

namespace RaptorOS.Commands;

public class EchoCommand : Command
{
    public EchoCommand()
    {
        Description = "Echos input";
        Arguments.Add(new ArgumentDefinition { IsRequired = true, TypeName = "string" });
    }

    public override void Execute(
        List<object> arguments,
        Dictionary<string, IEnumerable<object?>> options
    ) => Logger.Log(string.Join(" ", arguments));
}
