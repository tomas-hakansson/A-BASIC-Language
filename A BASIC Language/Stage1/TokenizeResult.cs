﻿namespace A_BASIC_Language.Stage1;

public class TokenizeResult
{
    public List<string> TokenValues { get; private set; } = new List<string>();
    public List<TokenType> TokenTypes { get; private set; } = new List<TokenType>();

    public void Add(string value, TokenType type)
    {
        TokenValues.Add(value);
        TokenTypes.Add(type);
    }

    public void AddMany(List<string> values, List<TokenType> types)
    {
        if (values.Count != types.Count)
            throw new ArgumentException("Values and types must have the same length");
        TokenValues.AddRange(values);
        TokenTypes.AddRange(types);
    }
}