using A_BASIC_Language.Stage1;

namespace A_BASIC_Language.StageN;

public class Parser
{
    readonly List<string> _tokenValues = new();
    readonly List<TokenType> _tokenTypes = new();
    int _index = 0;
    string _currentTokenValue = string.Empty;
    TokenType _currentTokenType = default;

    public Parser(List<string> values, List<Stage1.TokenType> types)//todo: tokenizer return type?
    {
        //Todo: This logic should be in a tokenized object created by the tokenizer.
        if (values.Count != types.Count)
            throw new ArgumentException("Values and types must have the same length");
        /* todo: initialisation:
         *  rpn program
         */
        ;

        //Note: Initialisation:
        Next();

        //Note: The parsing begins:
        try
        {
            AProgram();
        }
        catch (Exception ex)
        {
            //this try catch is for development but might find use in final program.
            var message = ex.Message;
        }
    }

    private void AProgram()
    {
        /* A program consists of zero or more lines of code.
         * Each line begins with a numeric label.
         * It's then followed by a statement or (in the case of let) a user defined name.
         * The statement can optionally be followed by:
         *  Control symbols (e.g. ',', ';' and maybe others).
         *  Other portions of the statement in the cases where it consists of multiple parts:
         *   If is followed by then (which itself can be followed by else) or in some versions of BASIC by goto.
         *   Def is followed by fn.
         *   On is followed by gosub or goto.
         *   For is followed by to (which itself can be followed by step).
         *   There might be others.
         *  Mathematical and logical operations and user defined functions.
         *  If can be followed by other statements.
         *  Finally it can be followed by ':' which is then followed by another statement.
         * 
         * It follows from this that a valid line looks something like this:
         *  aLabel aStatementWithParts (: aStatementWithParts)* //where * means zero or more.
         *  We therefore need a parser that first parses the number and then the statement with
         *   its parts.
         *  The statement parser itself identifies the current statement and calls its parser 
         *   (each statement with parts has its own parser)
         *  After the statement parser is done, the line parser (which contains all this) 
         *   checks if ':' follows and if so calls the statement parser again.
         *  This is repeated until the next label is found then the line parser returns and the
         *   program parser loops
         *  The line parser returns true if it finds a valid program line, false otherwise.
         *   
         */

        //I'm gonna start with input, let, print and goto. In that order.
        bool more;
        do
        {
            more = ALine();
        }
        while (more);
    }

    bool ALine()
    {
        //aLabel aStatementWithParts (: aStatementWithParts)*
        bool result = default;
        if (!ALabel())
            return false;

        AStatementPlusParts();

        while (MightMatch(TokenType.Comma))
        {
            AStatementPlusParts();
        }
        //Ponder: should these checks be done in AStatementPlusParts?
        if (_currentTokenType == TokenType.Label)
            result = true;
        if (_currentTokenType == TokenType.EOF)
            result = false;
        return result;
    }

    private bool ALabel()
    {
        if (_currentTokenType != TokenType.Label)
            return false;
        var currentValue = _currentTokenValue;
        //todo: set a parsed result with this value.
        SkipWhiteSpace();
        return true;
    }

    void AStatementPlusParts()
    {
        //This is the tricky part.

    }

    private void SkipWhiteSpace()
    {
        while (_currentTokenType != TokenType.Space)
            Next();
    }

    void MustMatch(TokenType tokenType)
    {
        if (tokenType == _currentTokenType)
        {
            Next();
        }
        else
            throw new Exception("placeholder");//todo: proper error handling.
    }

    bool MightMatch(TokenType tokenType)
    {
        if (tokenType == _currentTokenType)
        {
            Next();
            return true;
        }
        return false;
    }

    void Next()
    {
        if (_currentTokenType == TokenType.EOF)
            return;
        //if (_index >= _tokenValues.Count)//Note: End of file.
        //    return;
        _currentTokenType = _tokenTypes[_index];
        _currentTokenValue = _tokenValues[_index];
        _index++;
    }
}
