using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using A_BASIC_Language;
using A_BASIC_Language.Language;
using A_BASIC_Language.Language.LanguageParts;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var status = new ToolStripStatusLabel();
        using var font = new Font(FontFamily.GenericMonospace, 10);
        var log = new Log(status, font);
        var checks = 0;
        foreach (var jump in new[] { "GOSUB 100", "GO SUB 100", "go sub 50+50",
            "ON 2 GOSUB 90,100", "ON 2 GO SUB 90,100", "ON 2.9 GOSUB 90,100" })
        {
            var source = "10 " + jump + ":PRINT \"WRONG\":END\n90 PRINT \"WRONG\":END\n100 PRINT \"OK\":END";
            var result = Run(source);
            Require(result.Terminal.Lines.Contains("OK") && !result.Terminal.Lines.Contains("WRONG"), jump);
            var frame = result.Calls.Single();
            var parsed = new Parser(source).Result.EvalValues;
            Require(frame.LineNumber == 10 && parsed[frame.Index] is AblString, "Incorrect return point: " + jump);
            checks++;
        }
        var nested = Run("10 GOSUB 100:END\n100 GOSUB 200:END\n200 PRINT \"OK\":END");
        Require(nested.Calls.Select(f => f.LineNumber).SequenceEqual(new[] { 100, 10 }), "Nested call order");
        foreach (var jump in new[] { "GOTO 100", "GO TO 100", "ON 1 GOTO 100",
            "ON 0 GOSUB 999", "ON 3 GOSUB 999,998" })
        {
            var result = Run("10 " + jump + ":END\n100 END");
            Require(result.Calls.Count == 0 && !result.Terminal.Lines.Any(s => s.StartsWith("?")), jump);
            checks++;
        }
        foreach (var jump in new[] { "GOSUB 999", "ON 1 GOSUB 999", "GOSUB \"100\"",
            "GOSUB -1", "ON -1 GOSUB 100", "ON \"1\" GOSUB 100" })
        {
            var result = Run("10 " + jump + ":PRINT \"WRONG\"\n100 END");
            Require(result.Calls.Count == 0 && result.Terminal.Lines.Any(s => s.StartsWith("?Error in line 10:")) &&
                !result.Terminal.Lines.Contains("WRONG"), jump);
            checks++;
        }
        foreach (var source in new[] { "10 GOSUB", "10 GO SUB", "10 ON 1 GOSUB",
            "10 ON 1 GOSUB 100,", "10 ON 1 GO SUB A", "10 GO WRONG 100" })
        {
            Require(!new Parser(source).Result.Success, "Invalid syntax: " + source);
            checks++;
        }
        Require(Run("10 IF 1 THEN GO SUB 100 ELSE END\n100 END").Calls.Count == 1, "IF GOSUB");
        Require(!new Parser("ON 1 GOSUB 100", direct: true).Result.Success, "Direct ON restriction");
        foreach (var jump in new[] { "GOSUB 100", "GO SUB 100", "ON 2 GOSUB 90,100", "ON 2 GO SUB 90,100" })
        {
            CheckReturn("10 " + jump + ":PRINT \"BACK\":END\n90 END\n100 PRINT \"SUB\":RETURN",
                "SUB", "BACK");
            checks++;
        }
        CheckReturn("10 GOSUB 100\n20 PRINT \"BACK\":END\n100 RETURN", "BACK");
        CheckReturn("10 GOSUB 100:PRINT \"OUTER\":END\n100 GOSUB 200:PRINT \"INNER\":RETURN\n200 RETURN",
            "INNER", "OUTER");
        CheckReturn("10 FOR I=1 TO 3:GOSUB 100:NEXT I:END\n100 PRINT I:RETURN", " 1 ", " 2 ", " 3 ");
        CheckReturn("10 A=3:GOSUB 100:PRINT \"DONE\":END\n100 IF A=0 THEN RETURN\n110 PRINT A:A=A-1:GOSUB 100:RETURN",
            " 3 ", " 2 ", " 1 ", "DONE");
        CheckReturn("10 GOSUB 100:PRINT \"BACK\":END\n100 IF 0 THEN END ELSE RETURN", "BACK");
        CheckReturn("10 DEF FNA(X)=X+1:GOSUB 100:PRINT FNA(2):END\n100 PRINT FNA(1):RETURN", " 2 ", " 3 ");
        foreach (var source in new[] { "10 RETURN:PRINT \"WRONG\"",
            "10 GOSUB 100:RETURN\n100 RETURN",
            "10 ON 0 GOSUB 100:RETURN\n100 RETURN" })
        {
            var result = Run(source);
            Require(result.Terminal.Lines.Any(s => s.StartsWith("?Error in line 10:") && s.Contains("RETURN without GOSUB")) &&
                !result.Terminal.Lines.Contains("WRONG"), "Missing RETURN error: " + source);
            checks++;
        }
        var direct = new FakeTerminal();
        new Interpreter("RETURN", false, log).Run(direct);
        Require(direct.Lines.Any(s => s.Contains("?Error: RETURN without GOSUB")), "Direct RETURN error");
        Require(!new Parser("10 RETURN 100").Result.Success, "RETURN accepted an argument");

        void CheckReturn(string source, params string[] expected)
        {
            var result = Run(source);
            Require(result.Calls.Count == 0, "Unconsumed return frames");
            Require(result.Terminal.Lines.Take(expected.Length).SequenceEqual(expected) &&
                !result.Terminal.Lines.Any(s => s.StartsWith("?")), "Unexpected RETURN output: " +
                string.Join("|", result.Terminal.Lines));
        }
        Console.WriteLine($"GOSUB/RETURN checks passed ({checks} cases plus nesting and IF).");

        (FakeTerminal Terminal, Stack<(int Index, int LineNumber)> Calls) Run(string source)
        {
            var interpreter = new Interpreter(source, true, log);
            var terminal = new FakeTerminal();
            interpreter.Run(terminal);
            var calls = (Stack<(int Index, int LineNumber)>)typeof(Interpreter)
                .GetField("_subroutineCalls", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(interpreter);
            return (terminal, calls);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private sealed class FakeTerminal : IBasicTerminal
    {
        private string _line = "";
        public bool QuitFlag => false;
        public int OutputColumn => _line.Length;
        public List<string> Lines { get; } = new();
        public void Write(string text) => _line += text;
        public void WriteLine(string text) { Lines.Add(_line + text); _line = ""; }
        public void ClearScreen() => _line = "";
        public void SetTextColor(int color) { }
        public void SetTextRenderLimit(int limit) { }
        public string InputString(string prompt) => throw new Exception("Unexpected INPUT");
        public void PumpEvents() { }
    }
}
