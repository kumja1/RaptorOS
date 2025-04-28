using RaptorOS.Utils;

namespace RaptorOS.Commands.Core.Models;

public struct OptionDefinition
{
    public bool IsRequired;
    public EquatableArray<string> Aliases;
    public string Description;
    public ArgumentDefinition ValueDefinition;
    public int ArgumentLength;

    public override readonly string ToString() =>
        $"{string.Join(", ", Aliases)}{(!string.IsNullOrEmpty(ValueDefinition.TypeName) ? " " + ValueDefinition.ToString() : "")}: {Description}";
}
