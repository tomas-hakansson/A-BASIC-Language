﻿using A_BASIC_Language.Stage1;

namespace A_BASIC_Language;

internal class Parser
{
    public ParsedProgram Program { get; }

    public Parser(List<string> lines)
    {
        Tokenizer t1 = new(lines);
        Stage2.Tokenizer t2 = new(t1.TokenizedLines);
        Stage3.Parser p1 = new(t2.Result);
        Program = p1.Program;
    }
}