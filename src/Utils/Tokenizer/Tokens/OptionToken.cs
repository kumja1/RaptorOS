using System;

namespace RaptorOS.Utils.Tokenizer.Tokens;

public readonly struct OptionToken : IEquatable<OptionToken>
{
    public OptionToken(string? name, EquatableArray<ArgumentToken> arguments)
    {
        Name = name;
        Arguments = arguments;
    }

    public string? Name { get; } = null;
    public EquatableArray<ArgumentToken> Arguments { get; } = [];

    public bool Equals(OptionToken other) => Name == other.Name && Arguments.Equals(other.Arguments);
    public override bool Equals(object? obj) =>  obj is OptionToken other && Equals(other);
    
    public override int GetHashCode() => HashCode.Combine(Name, Arguments);
}