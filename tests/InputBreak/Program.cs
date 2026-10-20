using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using A_BASIC_Language;
using A_BASIC_Language.Language;

// Build with MSBuild tests/InputBreak/InputBreak.csproj, then run bin/Debug/InputBreak.exe.
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var status = new ToolStripStatusLabel();
        using var font = new Font(FontFamily.GenericMonospace, 10);
        var log = new Log(status, font);
        foreach (var test in new[]
        {
            ("10 DEF FNA(Z)=30*EXP(-Z*Z/100)\n20 PRINT FNA(0)", " 30 "),
            ("10 DEF FN A(Z)=Z*Z\n20 PRINT FN A(2+3)", " 25 "),
            ("10 Z=99\n20 DEF FNA(Z)=Z+1\n30 PRINT FNA(4);Z", " 5  99 "),
            ("10 K=2\n20 DEF FNA(Z)=Z*K\n30 K=3\n40 PRINT FNA(4)", " 12 "),
            ("10 DEF FNA(Z)=Z+1\n20 DEF FNB(Z)=FNA(Z*2)+Z\n30 PRINT 10+FNB(3)*2", " 30 "),
            ("10 DEF FNA(Z)=Z+1\n20 PRINT FNA(FNA(2))", " 4 "),
            ("10 PRINT FNA(1)", "Undefined function FN A."),
            ("10 DEF FNA(Z)=Z\n20 PRINT FNA(\"X\")", "Type mismatch"),
            ("10 DEF FNA(Z)=\"X\"\n20 PRINT FNA(1)", "Type mismatch"),
            ("10 DEF FNA(Z)=FNA(Z)\n20 PRINT FNA(1)", "nesting limit"),
            ("10 DEF FNA(Z)=Z+1\n20 DEF FNA(Z)=Z+2\n30 PRINT FNA(1)", " 3 ")
        })
        {
            var terminal = new FakeTerminal(() => throw new Exception("FN requested input."));
            new Interpreter(test.Item1, true, log, new RuntimeState()).Run(terminal);
            Require(terminal.Lines.Any(line => line.Contains(test.Item2)), "FN failed: " + test.Item1 + " => " + string.Join("|", terminal.Lines));
        }
        foreach (var source in new[] { "10 DEF FNA(Z)=", "10 DEF FNA(Z$)=Z", "10 DEF A(Z)=Z", "10 PRINT FNA(1,2)" })
            Require(!new Parser(source).Result.Success, "Invalid FN syntax accepted: " + source);
        {
            var state = new RuntimeState();
            var terminal = new FakeTerminal(() => throw new Exception("FN requested input."));
            new Interpreter("DEF FN a(z)=1/z", false, log, state).Run(terminal);
            Require(!terminal.Lines.Any(line => line.StartsWith("?")), "DEF evaluated its body prematurely.");
            new Interpreter("PRINT FN A(2)", false, log, state).Run(terminal);
            Require(terminal.Lines.Contains(" 0.5 "), "Direct FN definition did not persist.");
            state.Clear();
            Require(state.Functions.Count == 0, "Session clear retained FN definitions.");
        }
        {
            var terminal = new FakeTerminal(() => throw new Exception("3dplot requested input."));
            var source = System.IO.File.ReadAllText("A-BASIC-Language/A-BASIC-Language/Games/Source/3dplot.bas");
            new Interpreter(source, true, log, new RuntimeState()).Run(terminal);
            Require(!terminal.Lines.Any(line => line.StartsWith("?")), "3dplot failed: " + string.Join("|", terminal.Lines));
            Require(terminal.Lines.Count(line => line.Contains("*")) == 41, "3dplot did not produce all plot rows.");
        }
        Console.WriteLine("All DEF FN checks passed, including 3dplot.bas.");
        foreach (var test in new[]
        {
            ("A%=42", "A%", "text 42 text"),
            ("A%=-42", "A%", "text-42 text"),
            ("A%=0", "A%", "text 0 text"),
            ("A=2.5", "A", "text 2.5 text"),
            ("A=-2.5", "A", "text-2.5 text"),
            ("A$=\"42\"", "A$", "text42text")
        })
        {
            var terminal = new FakeTerminal(() => throw new Exception("PRINT requested input."));
            new Interpreter($"10 LET {test.Item1}\n20 PRINT \"text\";{test.Item2};\"text\"", true, log, new RuntimeState()).Run(terminal);
            Require(terminal.Lines.Contains(test.Item3), "Wrong PRINT formatting for " + test.Item1);
        }
        {
            var terminal = new FakeTerminal(() => throw new Exception("PRINT requested input."));
            new Interpreter("10 PRINT 4;2;\n20 PRINT \"X\"\n30 PRINT 1,2", true, log, new RuntimeState()).Run(terminal);
            Require(terminal.Lines.Contains(" 4  2 X"), "Wrong PRINT spacing or trailing semicolon.");
            Require(terminal.Lines.Contains(" 1 " + new string(' ', 11) + " 2 "), "Wrong PRINT comma zone.");
        }
        Console.WriteLine("All PRINT formatting regression checks passed (8 cases).");
        foreach (var test in new[]
        {
            ("10 PRINT TAB(5);\"*\";TAB(10);\"*\"", new[] { "    *    *" }),
            ("10 PRINT \"AB\";\n20 PRINT TAB(5);\"*\"", new[] { "AB  *" }),
            ("10 PRINT \"AB\";TAB(3);\"*\"", new[] { "AB*" }),
            ("10 PRINT \"AB\";TAB(1);\"*\"", new[] { "AB", "*" })
        })
        {
            var terminal = new FakeTerminal(() => throw new Exception("TAB requested input."));
            new Interpreter(test.Item1, true, log, new RuntimeState()).Run(terminal);
            Require(terminal.Lines.Take(test.Item2.Length).SequenceEqual(test.Item2), "Wrong absolute TAB positioning.");
        }
        {
            var terminal = new FakeTerminal(() => throw new Exception("3D plot requested input."));
            var source = System.IO.File.ReadAllText("A-BASIC-Language/A-BASIC-Language/Games/Source/3dplot.bas");
            new Interpreter(source, true, log, new RuntimeState()).Run(terminal);
            var plot = terminal.Lines.Where(line => line.Contains("*")).ToArray();
            Require(plot.Length == 41, "3D plot should have 41 rows.");
            Require(plot.All(line => line.Length <= 55), "3D plot exceeds its expected width.");
        }
        Console.WriteLine("All TAB and 3D plot regression checks passed.");
        foreach (var runtime in new[] { false, true })
        {
            var terminal = new FakeTerminal(() => throw new Exception("Completion requested input."));
            new Interpreter(runtime ? "10 PRINT \"DONE\"" : "PRINT \"DONE\"", runtime, log, new RuntimeState()).Run(terminal);
            Require(terminal.Lines.Contains("DONE"), "Program did not execute.");
            Require(terminal.Lines.Count(s => s == "Program has run through.") == (runtime ? 1 : 0), "Wrong completion message.");
            Require(terminal.Lines.Count(s => s == "Ready.") == (runtime ? 0 : 1), "Wrong completion prompt.");
        }
        Console.WriteLine("All completion message regression checks passed (2 cases).");
        var listCases = new[]
        {
            ("LIST", (int?)null, (int?)null), ("LIST 10", (int?)10, (int?)10),
            ("LIST 100-", (int?)100, (int?)null), ("LIST -100", (int?)null, (int?)100),
            ("LIST - 100", (int?)null, (int?)100), ("LIST 100-200", (int?)100, (int?)200),
            ("  list  100 - 200  ", (int?)100, (int?)200),
            ("LIST\t100\t-\t", (int?)100, (int?)null),
            ("LIST50", (int?)50, (int?)50), ("LIST50-100", (int?)50, (int?)100),
            ("LIST50-", (int?)50, (int?)null), ("LIST-100", (int?)null, (int?)100)
        };
        foreach (var test in listCases)
        {
            var terminal = new FakeTerminal(() => throw new Exception("LIST requested input."));
            var calls = 0;
            new BasicSession().Execute(test.Item1, log, terminal,
                () => throw new Exception("LIST tried to run the program."),
                (first, last) =>
                {
                    calls++;
                    Require(first == test.Item2 && last == test.Item3, "Wrong LIST bounds.");
                }, () => throw new Exception("LIST cleared the program."));
            Require(calls == 1, "LIST did not call listing exactly once.");
            Require(terminal.Lines.SequenceEqual(new[] { "", "Ready." }), "Wrong LIST completion.");
        }
        foreach (var command in new[] { "LIST -", "LIST 200-100", "LIST 1-2-3", "LIST abc", "LIST 999999999999", "LIST 1 00" })
        {
            var terminal = new FakeTerminal(() => throw new Exception("Invalid LIST requested input."));
            new BasicSession().Execute(command, log, terminal, () => "",
                (_, _) => throw new Exception("Invalid LIST was accepted."), () => { });
            Require(terminal.Lines.SequenceEqual(new[] { "?Invalid LIST range", "", "Ready." }), "Missing LIST error.");
        }
        Console.WriteLine("All LIST regression checks passed (12 valid cases, 6 invalid cases).");
        foreach (var variable in new[] { "A%", "A", "A$" })
        {
            foreach (var returnedInput in new[] { "", "123" })
            {
                var state = new RuntimeState();
                var interpreter = new Interpreter($"10 INPUT {variable}\n20 PRINT \"AFTER\"", true, log, state);
                var terminal = new FakeTerminal(() =>
                {
                    interpreter.UserBreak = true;
                    return returnedInput;
                });
                interpreter.Run(terminal);
                Require(terminal.InputCalls == 1, "Cancelled input was retried.");
                Require(!interpreter.UserBreak, "Break flag was not reset.");
                Require(terminal.Lines.Count(s => s == "User break.") == 1, "Missing break message.");
                Require(terminal.Lines.Count(s => s == "Ready.") == 1, "Missing ready message.");
                Require(!terminal.Lines.Any(s => s.Contains("AFTER") || s.Contains("Redo")), "Execution continued after break.");
                Require(state.Variables.Count == 0, "Cancelled input assigned a variable.");
            }
        }

        // Invalid numbers must still retry, and valid input must continue execution.
        foreach (var variable in new[] { "A%", "A" })
        {
            var values = new Queue<string>(new[] { "invalid", "42" });
            var terminal = new FakeTerminal(() => values.Dequeue());
            new Interpreter($"10 INPUT {variable}\n20 PRINT \"AFTER\"", true, log, new RuntimeState()).Run(terminal);
            Require(terminal.InputCalls == 2, "Invalid numeric input did not retry.");
            Require(terminal.Lines.Any(s => s.Contains("Redo")), "Missing retry message.");
            Require(terminal.Lines.Any(s => s.Contains("AFTER")), "Valid input did not continue execution.");
            Require(terminal.Lines.Contains("Program has run through."), "Normal execution did not finish.");
            Require(!terminal.Lines.Contains("Ready."), "Normal execution printed an extra ready message.");
        }
        Console.WriteLine("All input break regression checks passed (6 break cases, 2 retry cases).");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private sealed class FakeTerminal : IBasicTerminal
    {
        private readonly Func<string> _input;
        private string _line = "";
        public FakeTerminal(Func<string> input) { _input = input; }
        public void ClearScreen() { }
        public void SetTextRenderLimit(int limit) { }
        public bool QuitFlag => false;
        public int OutputColumn => _line.Length;
        public int InputCalls { get; private set; }
        public List<string> Lines { get; } = new List<string>();
        public void Write(string text) { _line += text; }
        public void WriteLine(string text) { Lines.Add(_line + text); _line = ""; }
        public string InputString(string prompt)
        {
            InputCalls++;
            _line = "";
            return _input();
        }
        public void PumpEvents() { }
    }
}
