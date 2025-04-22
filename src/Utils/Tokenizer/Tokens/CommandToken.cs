using System.Collections.Generic;

namespace RaptorOS.Utils.Tokenizer.Tokens;


public class CommandToken : Token {
    public string Name;
    public List<OptionToken> Options = [];
    public List<CommandToken> SubCommands = [];
}


