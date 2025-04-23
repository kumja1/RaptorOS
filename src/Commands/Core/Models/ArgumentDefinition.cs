namespace RaptorOS.Commands.Core.Models;

public struct ArgumentDefinition
{
    public bool IsRequired;
    public string? TypeName;

    public override readonly string ToString() => IsRequired ? $"<{TypeName}>" : $"[{TypeName}]";
}
