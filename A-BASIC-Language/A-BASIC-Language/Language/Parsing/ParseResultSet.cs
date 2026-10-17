namespace A_BASIC_Language.Language.Parsing;

public class ParseResultSet
{
    public bool Success { get; }
    public string FullName { get; }
    public string TypeSpecifier { get; }
    public bool DimVariable { get; }

    public ParseResultSet(bool success, string fullName, string typeSpecifier, bool dimVariable)
    {
        Success = success;
        FullName = fullName;
        TypeSpecifier = typeSpecifier;
        DimVariable = dimVariable;
    }
}
