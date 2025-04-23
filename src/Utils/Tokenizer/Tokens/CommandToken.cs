namespace RaptorOS.Utils.Tokenizer.Tokens;

public class CommandToken
{
    public string? Name;
    public EquatableArray<OptionToken> Options = [];
    public EquatableArray<ArgumentToken> Arguments = [];
}
