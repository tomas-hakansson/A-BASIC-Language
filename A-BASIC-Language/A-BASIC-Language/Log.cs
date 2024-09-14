using A_BASIC_Language.StringManipulation;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace A_BASIC_Language;

public class Log
{
    private const int LogWidth = 300;
    private const int LogHeight = 160;
    private const int LogMargin = 10;
    private readonly ToolStripStatusLabel _statusLabel;
    private readonly Font _logFont;
    public int StartPointer { get; set; }
    public string[] LogRows { get; }

    public Log(ToolStripStatusLabel statusLabel, Font logFont)
    {
        LogRows = new string[10];
        StartPointer = 0;
        _statusLabel = statusLabel;
        _logFont = logFont;
    }

    public void Clear()
    {
        for (var i = 0; i < LogRows.Length; i++)
            LogRows[i] = "";

        StartPointer = 0;
    }

    public void Write(string text)
    {
        _statusLabel.Text = text.MaxLength(10, 30);
        LogRows[StartPointer] = text;
        StartPointer++;

        if (StartPointer >= LogRows.Length)
            StartPointer = 0;
    }

    public string GetStringNumber(int number)
    {
        var pointer = (StartPointer + number) % LogRows.Length;
        return LogRows[pointer];
    }

    public void Paint(Graphics g, Control terminalControl)
    {
        var x = terminalControl.Width - (LogWidth + LogMargin);
        var y = terminalControl.Height - (LogHeight + LogMargin);
        g.CompositingMode = CompositingMode.SourceOver;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.SetClip(new RectangleF(x, y, LogWidth, LogHeight));

        for (var i = 0; i < 10; i++)
        {
            var text = GetStringNumber(i);
            var textY = y + (i * 16);
            g.DrawString(text, _logFont, Brushes.White, x, textY);
        }

        g.ResetClip();
        g.DrawRectangle(Pens.White, x, y, LogWidth, LogHeight);
    }
}