namespace A_BASIC_Language.Language;

/// <summary>Routes direct commands and owns the variables shared by the interactive session.</summary>
public sealed class BasicSession
{
    private readonly RuntimeState _state = new();
    private Interpreter? _interpreter;
    public bool IsRunning { get; private set; }
    public void Break() { if (_interpreter != null) _interpreter.UserBreak = true; }

    public void Execute(string command, IBasicTerminal terminal, Func<string> getProgram,
        Action listProgram, Action clearProgram)
    {
        // WinForms message pumping can deliver another Enter while a program is running.
        if (IsRunning || string.IsNullOrWhiteSpace(command)) return;
        command = command.Trim();
        if (command.Equals("LIST", StringComparison.OrdinalIgnoreCase))
        {
            listProgram();
            return;
        }
        if (command.Equals("NEW", StringComparison.OrdinalIgnoreCase))
        {
            clearProgram();
            _state.Clear();
            terminal.WriteLine("Ready.");
            return;
        }
        var runtime = command.Equals("RUN", StringComparison.OrdinalIgnoreCase);
        if (runtime) _state.Clear();
        IsRunning = true;
        try
        {
            _interpreter = new Interpreter(runtime ? getProgram() : command, runtime, _state);
            _interpreter.Run(terminal);
        }
        finally
        {
            _interpreter = null;
            IsRunning = false;
        }
    }
}
