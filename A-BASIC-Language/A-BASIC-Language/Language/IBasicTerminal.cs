namespace A_BASIC_Language.Language;

/// <summary>Text operations needed by BASIC; Write appends at the current output position.</summary>
public interface IBasicTerminal
{
    bool QuitFlag { get; }
    int OutputColumn { get; }
    void Write(string text);
    void WriteLine(string text);
    string InputString(string prompt);
    void PumpEvents();
}
