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
            Require(terminal.Lines.Contains("Ready."), "Normal execution did not finish.");
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
