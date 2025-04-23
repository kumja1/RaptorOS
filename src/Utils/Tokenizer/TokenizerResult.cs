using RaptorOS.Utils.Tokenizer.Tokens;

namespace RaptorOS.Utils.Tokenizer;

public struct TokenizerResult(CommandToken token)
{
    public CommandToken Token = token;
    public EquatableArray<string> Issues = [];
}
