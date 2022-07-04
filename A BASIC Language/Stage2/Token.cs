namespace A_BASIC_Language.Stage2;

internal enum Token
{
    Label,
    OpeningParenthesis,
    ClosingParenthesis,
    Statement,
    EqualityOrAssignment,
    Comma,
    Colon,
    Semicolon,
    Operator,
    StandardFunction,
    UserDefinedFunction,
    Number,
    Other,//Ponder: By now maybe the only thing this could be are variables.
}