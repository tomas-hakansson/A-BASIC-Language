using System;
using System.Text;
using System.Windows.Forms;
using A_BASIC_Language.TerminalMatrix;

namespace A_BASIC_Language.Language;

public sealed class TerminalAdapter(TerminalMatrixControl terminal) : IBasicTerminal
{
    private readonly StringBuilder _line = new();
    public int OutputColumn => _line.Length;
    public bool QuitFlag => terminal.QuitFlag;

    public void ClearScreen()
    {
        terminal.Clear();
        _line.Clear();
        terminal.SetStartPosition(0, terminal.CursorPosition.Y);
    }

    public void SetTextRenderLimit(int limit) => terminal.SetTextRenderLimit(limit);
    public void SetTextColor(int color) => terminal.CurrentCursorColor = (byte)(color - 1);
    public void SetBackgroundColor(int color) => terminal.SetBackgroundColor((byte)(color - 1));
    public void SetBorderColor(int color) => terminal.SetBorderColor((byte)(color - 1));
    public void PumpEvents() => Application.DoEvents();

    public void Write(string text)
    {
        text = text.Replace("\r\n", "\n").Replace('\r', '\n');
        var index = 0;
        while (index < text.Length)
        {
            if (text[index] == '\n')
            {
                WriteLine("");
                index++;
                continue;
            }

            if (_line.Length == terminal.CharacterWidth)
                WriteLine("");

            var newline = text.IndexOf('\n', index);
            var count = Math.Min(terminal.CharacterWidth - _line.Length,
                (newline < 0 ? text.Length : newline) - index);
            var chunk = text.Substring(index, count);
            terminal.Write(_line.Length, chunk);
            _line.Append(chunk);
            index += count;
            terminal.SetStartPosition(Math.Min(_line.Length, terminal.CharacterWidth - 1), terminal.CursorPosition.Y);
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
        if (_line.Length >= terminal.CharacterWidth - 2)
            WriteLine("");

        var value = terminal.InputString($"{_line}{prompt}");
        _line.Clear();
        return value;
    }
}
