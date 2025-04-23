namespace RaptorOS.Utils.Tokenizer.Tokens;

public readonly record struct OptionToken(
    string Name,
    bool IsShortHand,
    EquatableArray<ArgumentToken> Arguments
);
