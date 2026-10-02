using System;
using System.Text;
using System.Windows.Forms;
using TerminalMatrixNetFramework;
using TerminalMatrixNetFramework.Definitions;

namespace A_BASIC_Language.Language;

public sealed class TerminalAdapter(TerminalMatrixControl terminal) : IBasicTerminal
{
    private readonly StringBuilder _line = new();
    public int OutputColumn => _line.Length;
    public bool QuitFlag => terminal.QuitFlag;
    public void PumpEvents() => Application.DoEvents();

    public void Write(string text)
    {
        foreach (var c in text.Replace("\r\n", "\n").Replace('\r', '\n'))
        {
            if (c == '\n')
            {
                WriteLine("");
                continue;
            }

            if (_line.Length == CharacterMatrixDefinition.Width)
                WriteLine("");
            
            terminal.Write(_line.Length, c.ToString());
            _line.Append(c);
            terminal.SetStartPosition(Math.Min(_line.Length, CharacterMatrixDefinition.Width - 1), terminal.CursorPosition.Y);
        }
    }

    public void WriteLine(string text)
    {
        Write(text);
        terminal.WriteLine("");
        _line.Clear();
    }

    public string InputString(string prompt)
    {
        if (_line.Length >= CharacterMatrixDefinition.Width - 2)
            WriteLine("");

        var value = terminal.InputString($"{_line}{prompt}");
        _line.Clear();
        return value;
    }
}
