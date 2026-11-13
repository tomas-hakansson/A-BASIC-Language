using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using A_BASIC_Language.TerminalMatrix;

namespace A_BASIC_Language.Language;

public sealed class TerminalAdapter : IBasicTerminal
{
    private readonly MainWindow _owner;
    private readonly StringBuilder _line = new();
    private readonly TerminalMatrixControl _terminal;

    public TerminalAdapter(MainWindow owner, TerminalMatrixControl terminal)
    {
        _terminal = terminal;
        _owner = owner;
    }

    public int OutputColumn => _line.Length;
    public bool QuitFlag => _terminal.QuitFlag;

    public void ClearScreen()
    {
        _terminal.Clear();
        _line.Clear();
        _terminal.SetStartPosition(0, _terminal.CursorPosition.Y);
    }

    public void SetTextRenderLimit(int limit) =>
        _terminal.SetTextRenderLimit(limit);

    internal void HomeCursor() =>
        _terminal.SetStartPosition(0, 0);

    internal void Scroll(int mode) =>
        _terminal.ScrollText(mode);

    public void SetTextColor(int color) =>
        _terminal.CurrentCursorColor = (byte)(color - 1);

    public void SetBackgroundColor(int color) =>
        _terminal.SetBackgroundColor((byte)(color - 1));

    public void SetBorderColor(int color) =>
        _terminal.SetBorderColor((byte)(color - 1));

    public void MainWindowLoadProgram(string source) =>
        _owner.LoadProgram(source, showReady: false);

    public bool MainWindowSaveProgram(string source) =>
        _owner.SaveProgram(source);

    public void MainWindowSessionSetFolders(FileInfo basicFileInfo) =>
        _owner.SessionSetFolders(basicFileInfo);

    public string SourceCode =>
        _terminal.ProgramLines.ToString();

    public void PumpEvents() =>
        Application.DoEvents();

    internal void EnsureGlyph(char character) =>
        _terminal.EnsureGlyph(character);

    internal void AttachFontMemory(byte[] memory) =>
        _terminal.AttachFontMemory(memory);

    internal void UpdateFontMemory(int address, byte value) =>
        _terminal.UpdateFontMemory(address, value);

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

            if (_line.Length == _terminal.CharacterWidth)
                WriteLine("");

            var newline = text.IndexOf('\n', index);
            var count = Math.Min(_terminal.CharacterWidth - _line.Length, (newline < 0 ? text.Length : newline) - index);
            var chunk = text.Substring(index, count);
            _terminal.Write(_line.Length, chunk);
            _line.Append(chunk);
            index += count;
            _terminal.SetStartPosition(Math.Min(_line.Length, _terminal.CharacterWidth - 1), _terminal.CursorPosition.Y);
        }
    }

    public void WriteLine(string text)
    {
        Write(text);
        _terminal.WriteLine("");
        _line.Clear();
    }

    public void WriteLine(bool blankLineFirst, string text)
    {
        if (blankLineFirst)
            WriteLine("");

        WriteLine(text);
    }

    public string InputString(string prompt)
    {
        if (_line.Length >= _terminal.CharacterWidth - 2)
            WriteLine("");

        var value = _terminal.InputString($"{_line}{prompt}");
        _line.Clear();
        return value;
    }
}
