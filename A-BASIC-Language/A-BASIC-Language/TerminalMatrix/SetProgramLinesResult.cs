namespace TerminalMatrixNetFramework;

public class SetProgramLinesResult
{
    public int AffectedRows { get; }
    public int TotalRows { get; }
    public bool Success { get; }
    public string ErrorMessage { get; }

    internal SetProgramLinesResult(int affectedRows, int totalRows, bool success, string errorMessage)
    {
        AffectedRows = affectedRows;
        TotalRows = totalRows;
        Success = success;
        ErrorMessage = errorMessage;
    }

    internal static SetProgramLinesResult CreateSuccess(int affectedRows, int totalRows) =>
        new(affectedRows, totalRows, true, "");

    internal static SetProgramLinesResult CreateFail(int affectedRows, int totalRows, string errorMessage) =>
        new(affectedRows, totalRows, false, errorMessage);
}
