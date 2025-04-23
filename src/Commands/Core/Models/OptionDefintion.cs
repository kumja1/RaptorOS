using System.Collections.Generic;

namespace RaptorOS.Commands.Core.Models;

public struct OptionDefinition
{
    public bool IsRequired;
    public List<string> Aliases;
    public string Description;
    public ArgumentDefinition ValueDefinition;

    public override readonly string ToString() =>
        $"{string.Join(", ", Aliases)}{(!string.IsNullOrEmpty(ValueDefinition.TypeName) ? " " + ValueDefinition.ToString() : "")}: {Description}";
}
