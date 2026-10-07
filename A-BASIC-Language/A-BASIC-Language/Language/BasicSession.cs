#nullable enable
using System;
using System.Text.RegularExpressions;

namespace A_BASIC_Language.Language;

/// <summary>Routes direct commands and owns the variables shared by the interactive session.</summary>
public sealed class BasicSession
{
    private readonly RuntimeState _state = new();
    private Interpreter? _interpreter;
    public bool IsRunning { get; private set; }

    public void Break() =>
        _interpreter?.UserBreak = true;

    public void Execute(string command, Log log, IBasicTerminal terminal, Func<string> getProgram, Action<int?, int?> listProgram, Action clearProgram)
    {
        // WinForms message pumping can deliver another Enter while a program is running.
        if (IsRunning || string.IsNullOrWhiteSpace(command))
            return;

        command = command.Trim();
        
        if (command.Equals("LIST", StringComparison.OrdinalIgnoreCase) ||
            (command.StartsWith("LIST", StringComparison.OrdinalIgnoreCase) &&
             command.Length > 4 && (char.IsWhiteSpace(command[4]) ||
                                    (command[4] >= '0' && command[4] <= '9') || command[4] == '-')))
        {
            if (TryParseListRange(command.Substring(4), out var first, out var last))
                listProgram(first, last);
            else
                terminal.WriteLine("?Invalid LIST range");
            terminal.WriteLine("");
            terminal.WriteLine("Ready.");
            return;
        }

        if (command.Equals("NEW", StringComparison.OrdinalIgnoreCase))
        {
            clearProgram();
            _state.Clear();
            terminal.WriteLine("");
            terminal.WriteLine("Ready.");
            return;
        }
        var runtime = command.Equals("RUN", StringComparison.OrdinalIgnoreCase);
        
        if (runtime)
            _state.Clear();
        
        IsRunning = true;
        
        try
        {
            _interpreter = new Interpreter(runtime ? getProgram() : command, runtime, log, _state);
            _interpreter.Run(terminal);
        }
        finally
        {
            _interpreter = null;
            IsRunning = false;
        }
    }

    private static bool TryParseListRange(string argument, out int? first, out int? last)
    {
        first = last = null;
        argument = argument.Trim();
        if (argument.Length == 0)
            return true;

        var match = Regex.Match(argument, @"\A(?:([0-9]+)|([0-9]*)\s*-\s*([0-9]*))\z");
        if (!match.Success)
            return false;

        if (match.Groups[1].Success)
        {
            if (!int.TryParse(match.Groups[1].Value, out var line))
                return false;
            first = last = line;
            return true;
        }

        var firstText = match.Groups[2].Value;
        var lastText = match.Groups[3].Value;
        if (firstText.Length == 0 && lastText.Length == 0)
            return false;

        if (firstText.Length > 0)
        {
            if (!int.TryParse(firstText, out var line))
                return false;
            first = line;
        }
        if (lastText.Length > 0)
        {
            if (!int.TryParse(lastText, out var line))
                return false;
            last = line;
        }
        return !first.HasValue || !last.HasValue || first.Value <= last.Value;
    }
}
