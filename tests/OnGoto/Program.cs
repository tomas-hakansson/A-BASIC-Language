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

        FakeTerminal Run(string source, bool runtime = true)
        {
            var terminal = new FakeTerminal();
            new Interpreter(source, runtime, log, new RuntimeState()).Run(terminal);
            checks++;
            return terminal;
        }

        // Selection, numeric expressions, arrays, built-ins and user functions.
        foreach (var test in new[]
        {
            ("1", "FIRST"), ("2", "SECOND"), ("3", "THIRD"),
            ("2.9", "SECOND"), ("A+1", "SECOND"), ("A%+1", "SECOND"),
            ("ABS(-2)", "SECOND"), ("FNA(1)", "SECOND"),
            ("B(1)", "SECOND"), ("(1+2)*2-4", "SECOND"),
            ("1E0+1", "SECOND"), ("2^1", "SECOND")
        })
        {
            var result = Run("10 A=1:A%=1:DIM B(2):B(1)=2:DEF FNA(Z)=Z+1\n" +
                $"20 ON {test.Item1} GOTO 100,200,300:PRINT \"WRONG\":END\n" +
                "100 PRINT \"FIRST\":END\n200 PRINT \"SECOND\":END\n300 PRINT \"THIRD\":END");
            RequireOutput(result, test.Item2);
        }

        // Compact selectors must stop before the jump keyword, including long names.
        foreach (var selector in new[] { "I", "INDEX", "I+0", "A(1)" })
        {
            RequireOutput(Run("10 I=2:INDEX=2:DIM A(1):A(1)=2\n" +
                $"20 ON{selector}GOTO100,200\n" +
                "100 PRINT \"WRONG\":END\n200 PRINT \"OK\":END"), "OK");
        }
        foreach (var keyword in new[] { "GOSUB", "GO SUB", "gosub" })
            RequireOutput(Run("10 I=1\n" + $"20 ONI{keyword}100:PRINT \"BACK\":END\n" +
                "100 PRINT \"SUB\":RETURN"), "SUB", "BACK");
        Require(new Parser("2140 ONIGOTO2300,1980,4000,4260,4700,5530,5690,7290,6270").Result.Success,
            "Super Star Trek line 2140 did not parse.");
        Require(new Parser("10 ON LEN(\"GOTO2300\")GOTO100,200").Result.Success,
            "Jump text inside a selector string was treated as a keyword.");

        // Falling through executes the next statement on the same line and consumes the selector.
        foreach (var index in new[] { "0", "0.9", "4", "256", "1E100" })
            RequireOutput(Run($"10 ON {index} GOTO 999,998,997:PRINT \"NEXT\"\n20 PRINT 7"), "NEXT", " 7 ");
        RequireOutput(Run("10 ON 0 GOTO 999\n20 PRINT \"NEXT\""), "NEXT");
        RequireOutput(Run("10 ON 1 GOTO 30,999\n20 PRINT \"WRONG\":END\n30 PRINT \"OK\""), "OK");
        RequireOutput(Run("10 on 2 go to 90,40\n40 PRINT \"OK\":END\n90 PRINT \"WRONG\""), "OK");
        RequireOutput(Run("10 ON 1 GOTO 0\n0 PRINT \"OK\":END"), "OK");
        RequireOutput(Run("10 IF 1 THEN ON 2 GOTO 100,200 ELSE PRINT \"WRONG\"\n" +
            "100 PRINT \"WRONG\":END\n200 PRINT \"OK\""), "OK");
        RequireOutput(Run("10 IF 1 THEN ON 0 GOTO 999:PRINT \"OK\" ELSE PRINT \"WRONG\""), "OK");
        RequireOutput(Run("10 FOR I=1 TO 3\n20 ON I GOTO 100,200,300\n" +
            "100 PRINT \"FIRST\":GOTO 400\n200 PRINT \"SECOND\":GOTO 400\n" +
            "300 PRINT \"THIRD\"\n400 NEXT I"), "FIRST", "SECOND", "THIRD");

        foreach (var test in new[]
        {
            ("-1", "out of range"), ("-0.1", "out of range"),
            ("\"2\"", "Type mismatch"), ("1", "Undefined line 999")
        })
        {
            var result = Run($"10 ON {test.Item1} GOTO 999:PRINT \"WRONG\"");
            Require(result.Lines.Any(s => s.StartsWith("?Error in line 10:") && s.Contains(test.Item2)),
                "Missing runtime error for " + test.Item1);
            Require(!result.Lines.Contains("WRONG"), "Execution continued after an error.");
        }

        foreach (var source in new[]
        {
            "10 ON 1", "10 ON GOTO 100", "10 ON 1 GOTO",
            "10 ON 1 GOTO ,100", "10 ON 1 GOTO 100,", "10 ON 1 GOTO 100,,200",
            "10 ON 1 GOTO A", "10 ON 1 GOTO 10+20", "10 ON 1 GOTO 1.5",
            "10 ON 1 GOTO -10", "10 ON 1 GOTO 1E2", "10 ON 1 GOTO 2147483648",
            "10 ON 1 GOTO 100 200", "ON 1 GOTO 100"
        })
        {
            Require(!new Parser(source).Result.Success, "Invalid syntax accepted: " + source);
            checks++;
        }

        foreach (var source in new[] { "ON 1 GOTO 100", "PRINT 1:ON 1 GOTO 100", "IF 1 THEN ON 1 GOTO 100" })
        {
            var parse = new Parser(source, direct: true).Result;
            Require(!parse.Success && parse.Errors.Any(s => s.Contains("only allowed in numbered program lines")),
                "Direct ON GOTO accepted: " + source);
            Require(Run(source, runtime: false).Lines.Any(s => s.StartsWith("?Syntax error:")),
                "Missing direct-mode syntax error.");
        }

        Console.WriteLine($"All ON GOTO regression checks passed ({checks} cases).");
    }

    private static void RequireOutput(FakeTerminal terminal, params string[] expected)
    {
        var actual = terminal.Lines.ToList();
        Require(actual.Count >= 2 && actual[actual.Count - 1] == "Program has run through.",
            "Program did not finish normally: " + string.Join("|", actual));
        actual.RemoveRange(actual.Count - 2, 2); // The interpreter's final blank line and status.
        Require(actual.SequenceEqual(expected), "Unexpected output: " + string.Join("|", terminal.Lines));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private sealed class FakeTerminal : IBasicTerminal
    {
        private string _line = "";
        private int _ticks;
        public void ClearScreen() { }
        public void SetTextRenderLimit(int limit) { }
        public bool QuitFlag => false;
        public int OutputColumn => _line.Length;
        public List<string> Lines { get; } = new List<string>();
        public void Write(string text) { _line += text; }
        public void WriteLine(string text) { Lines.Add(_line + text); _line = ""; }
        public string InputString(string prompt) => throw new Exception("Unexpected INPUT.");
        public void PumpEvents()
        {
            if (++_ticks > 10000) throw new Exception("Possible infinite loop.");
        }
    }
}
