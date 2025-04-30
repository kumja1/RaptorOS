using System;

namespace RaptorOS.Utils.Tokenizer.Tokens;

public readonly struct OptionToken(string? name, EquatableArray<ArgumentToken> arguments)
    : IEquatable<OptionToken>
{
    public string? Name { get; } = name;
    public EquatableArray<ArgumentToken> Arguments { get; } = arguments;

    public bool Equals(OptionToken other) =>
        Name == other.Name && Arguments.Equals(other.Arguments);

    public override bool Equals(object? obj) => obj is OptionToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Name, Arguments);

    public static bool operator ==(OptionToken left, OptionToken right) => left.Equals(right);

    public static bool operator !=(OptionToken left, OptionToken right) => !(left == right);
}
