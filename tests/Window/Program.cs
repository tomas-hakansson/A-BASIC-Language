using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using A_BASIC_Language;
using A_BASIC_Language.Language;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var status = new ToolStripStatusLabel();
        using var font = new Font(FontFamily.GenericMonospace, 10);
        var log = new Log(status, font);
        var checks = 0;
        foreach (var runtime in new[] { false, true })
        {
            foreach (var test in new[] {
                ("WINDOW", 0), ("window 10", 15), ("WINDOW 2", 23),
                ("WINDOW 25", 0), ("A=5:WINDOW A*2", 15),
                ("WINDOW:PRINT 1", 0),
                ("IF 1 THEN WINDOW ELSE WINDOW 2", 0),
                ("IF 0 THEN WINDOW ELSE WINDOW 2", 23)
            })
            {
                var terminal = Run(test.Item1, runtime);
                Require(terminal.Limits.SequenceEqual(new[] { test.Item2 }) && terminal.ClearCount == 1, test.Item1);
                Require(!terminal.Lines.Any(s => s.StartsWith("?")), string.Join("|", terminal.Lines));
                checks++;
            }
            foreach (var argument in new[] { "0", "1", "26", "-1", "2.5", "\"10\"", "1E100" })
            {
                var terminal = Run("WINDOW " + argument + ":PRINT \"WRONG\"", runtime);
                Require(terminal.ClearCount == 0 && terminal.Limits.Count == 0 && terminal.Lines.Any(s => s.StartsWith("?")) &&
                    !terminal.Lines.Contains("WRONG"), "Invalid argument accepted: " + argument);
                checks++;
            }
            var reset = Run("PRINT " + "\"OLD\";" + ":WINDOW 10:WINDOW", runtime);
            Require(reset.ClearCount == 2 && reset.Limits.SequenceEqual(new[] { 15, 0 }), "Reset failed");
        }
        // Exercise the session path used by direct commands and RUN.
        var session = new BasicSession();
        var target = new FakeTerminal();
        session.Execute("WINDOW 10", log, target, () => "", (_, __) => {}, () => {});
        session.Execute("RUN", log, target, () => "10 WINDOW", (_, __) => {}, () => {});
        Require(target.ClearCount == 3 && target.Limits.SequenceEqual(new[] { 15, 0, 0 }), "Session routing failed");

        foreach (var command in new[] { "NEW", "RUN" })
        {
            var resetSession = new BasicSession();
            var resetTarget = new FakeTerminal();
            var programCleared = false;
            resetSession.Execute("A=7:WINDOW 2", log, resetTarget, () => "", (_, __) => {}, () => {});
            resetSession.Execute(command, log, resetTarget, () =>
            {
                Require(resetTarget.Limits.Last() == 0 && resetTarget.ClearCount == 2,
                    "RUN did not reset the window before loading the program");
                return "10 PRINT A";
            }, (_, __) => {}, () => programCleared = true);
            Require(resetTarget.Limits.SequenceEqual(new[] { 23, 0 }) && resetTarget.ClearCount == 2,
                command + " did not reset the window");
            Require(programCleared == (command == "NEW"), "Unexpected program clearing");
            if (command == "NEW")
                resetSession.Execute("PRINT A", log, resetTarget, () => "", (_, __) => {}, () => {});
            Require(resetTarget.Lines.Contains(" 0 "), command + " did not clear variables");
        }
        Console.WriteLine($"WINDOW checks passed ({checks} cases plus reset and session routing).");

        FakeTerminal Run(string source, bool runtime)
        {
            var terminal = new FakeTerminal();
            new Interpreter(runtime ? "10 " + source : source, runtime, log, new RuntimeState()).Run(terminal);
            return terminal;
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private sealed class FakeTerminal : IBasicTerminal
    {
        public bool QuitFlag => false;
        public int OutputColumn => 0;
        public List<int> Limits { get; } = new();
        public List<string> Lines { get; } = new();
        private bool _cleared;
        public int ClearCount { get; private set; }
        public void ClearScreen() { _cleared = true; ClearCount++; _line = ""; }
        public void SetTextRenderLimit(int limit)
        {
            Require(_cleared, "WINDOW changed the limit before clearing the screen");
            _cleared = false;
            Limits.Add(limit);
        }
        private string _line = "";
        public void Write(string text) => _line += text;
        public void WriteLine(string text) { Lines.Add(_line + text); _line = ""; }
        public string InputString(string prompt) => throw new Exception("Unexpected INPUT");
        public void PumpEvents() { }
    }
}
