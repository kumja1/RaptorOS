namespace RaptorOS.Utils.Tokenizer.Tokens;

public struct CommandToken(string? name)
{
    public CommandToken()
        : this(null) { }

    public string? Name = name;
    public EquatableArray<OptionToken> Options = [];
    public EquatableArray<ArgumentToken> Arguments = [];
}
