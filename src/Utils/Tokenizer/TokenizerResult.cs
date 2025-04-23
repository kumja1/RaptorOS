using System.Collections.Generic;
using RaptorOS.Utils.Tokenizer.Tokens;

namespace RaptorOS.Utils.Tokenizer;

public class TokenizerResult(CommandToken token)
{
    public CommandToken Token = token;
    public List<string> Issues = [];
}
