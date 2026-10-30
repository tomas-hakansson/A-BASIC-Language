using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using A_BASIC_Language;
using A_BASIC_Language.Language;
using A_BASIC_Language.TerminalMatrix;
using TerminalMatrixNetFramework;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        try { Run(args); }
        catch (Exception ex) { Console.Error.WriteLine(ex); Environment.ExitCode = 1; }
    }

    private static void Run(string[] args)
    {
        using var status = new ToolStripStatusLabel();
        using var strip = new StatusStrip();
        strip.Items.Add(status);
        using var font = new Font(FontFamily.GenericMonospace, 10);
        var log = new Log(status, font);
        Console.WriteLine("Checking output batching...");
        CheckOutputBatching();
        var breakTerminal = new BenchmarkTerminal("");
        var endless = new Interpreter("10 GOTO 10", true, log, new RuntimeState());
        breakTerminal.OnPump = () =>
        {
            if (breakTerminal.Pumps >= 3) endless.UserBreak = true;
        };
        endless.Run(breakTerminal);
        if (!breakTerminal.Output.ToString().Contains("User break."))
            throw new Exception("Event pumping did not deliver the user break.");
        var source = File.ReadAllText(args[0]);
        foreach (var size in new[] { "10,10", "20,15" })
        {
            var terminal = new BenchmarkTerminal(size);
            var watch = Stopwatch.StartNew();
            var interpreter = new Interpreter(source, true, log, new RuntimeState());
            var parseTime = watch.ElapsedMilliseconds;
            interpreter.Run(terminal);
            if (!terminal.Output.ToString().Contains("Program has run through.") || terminal.Output.ToString().Contains("?Error"))
                throw new Exception("AMAZING failed: " + terminal.Output);
            Console.WriteLine($"AMAZING {size}: {watch.ElapsedMilliseconds} ms total, {parseTime} ms parsing, {terminal.Pumps} event pumps, {terminal.Output.Length} output characters");
        }
        Console.WriteLine("Output batching and user break checks passed.");
    }

    private static void CheckOutputBatching()
    {
        using var actual = new TerminalMatrixControl();
        using var expected = new TerminalMatrixControl();
        actual.SetResolution(Resolution.Pixels320x200Characters40x25);
        expected.SetResolution(Resolution.Pixels320x200Characters40x25);
        var adapter = new TerminalAdapter(actual);
        var line = new StringBuilder();
        var random = new Random(1234);
        var mapField = typeof(TerminalMatrixControl).GetField("_characterMap", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var text in new[] { "", "ABC", "\r\n", new string('X', 40), "\n", new string('Y', 83), "\rZ\n\r\n" })
            Compare(text);
        for (var n = 0; n < 10; n++)
        {
            var text = new StringBuilder();
            for (var i = random.Next(50); i > 0; i--)
                text.Append("ABC \r\n"[random.Next(6)]);
            Compare(text.ToString());
        }

        void Compare(string text)
        {
            adapter.Write(text);
            // Reference: the original character-at-a-time output, including wrapping and scrolling.
            foreach (var c in text.Replace("\r\n", "\n").Replace('\r', '\n'))
            {
                if (c == '\n' || line.Length == expected.CharacterWidth)
                {
                    expected.WriteLine("");
                    line.Clear();
                    if (c == '\n') continue;
                }
                expected.Write(line.Length, c.ToString());
                line.Append(c);
                expected.SetStartPosition(Math.Min(line.Length, expected.CharacterWidth - 1), expected.CursorPosition.Y);
            }
            if (adapter.OutputColumn != line.Length || actual.CursorPosition.X != expected.CursorPosition.X || actual.CursorPosition.Y != expected.CursorPosition.Y)
                throw new Exception("Output cursor or column changed.");
            var a = (byte[,])mapField.GetValue(actual);
            var b = (byte[,])mapField.GetValue(expected);
            for (var y = 0; y < actual.CharacterHeight; y++)
                for (var x = 0; x < actual.CharacterWidth; x++)
                    if (a[x, y] != b[x, y]) throw new Exception("Output content changed.");
        }
    }

    private sealed class BenchmarkTerminal : IBasicTerminal
    {
        private readonly Queue<string> _inputs;
        public readonly StringBuilder Output = new StringBuilder();
        public int Pumps;
        public Action OnPump;
        public BenchmarkTerminal(string size) { _inputs = new Queue<string>(size.Split(',')); }
        public bool QuitFlag => false;
        public int OutputColumn { get; private set; }
        public void Write(string text) { Output.Append(text); OutputColumn += text.Length; }
        public void WriteLine(string text) { Output.AppendLine(text); OutputColumn = 0; }
        public string InputString(string prompt) { return _inputs.Dequeue(); }
        public void PumpEvents() { Pumps++; Application.DoEvents(); OnPump?.Invoke(); }
        public void ClearScreen() { }
        public void SetBackgroundColor(int color) { }
        public void SetBorderColor(int color) { }
        public void SetTextColor(int color) { }
        public void SetTextRenderLimit(int limit) { }
    }
}
