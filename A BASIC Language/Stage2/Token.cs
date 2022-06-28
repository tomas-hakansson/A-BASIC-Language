// See https://aka.ms/new-console-template for more information


namespace StageTwo
{
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
        UserDefinedFunction,//syntax seems to be FNx where x is user optional.
        Number,
        Other,//Ponder: By now maybe the only thing this could be are variables.
    }
}