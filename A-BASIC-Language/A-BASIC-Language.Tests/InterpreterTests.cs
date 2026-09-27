using System.Text;
using A_BASIC_Language.Language;
using A_BASIC_Language.Language.Parsing;
using A_BASIC_Language.ValueTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_BASIC_Language.Tests;

[TestClass]
public class InterpreterTests
{
    private sealed class Terminal : IBasicTerminal
    {
        public StringBuilder Output { get; } = new();
        public int OutputColumn => Output.Length - Output.ToString().LastIndexOf('\n') - 1;
        public bool QuitFlag { get; set; }
        public Action? Pump { get; set; }
        public Queue<string> Inputs { get; } = new();
        public void Write(string text) => Output.Append(text);
        public void WriteLine(string text) => Output.Append(text).Append('\n');
        public string InputString(string prompt) => Inputs.Dequeue();
        public void PumpEvents() => Pump?.Invoke();
    }

    private static string Run(string source, bool runtime = true)
    {
        var terminal = new Terminal();
        new Interpreter(source, runtime).Run(terminal);
        return terminal.Output.ToString();
    }

    [DataTestMethod]
    [DataRow("HEJ", "?Syntax error:")]
    [DataRow("LET A 123", "?Syntax error:")]
    [DataRow("IF 1 THEN", "?Syntax error:")]
    [DataRow("PRINT 1/0", "?Error:")]
    [DataRow("PRINT \"A\"-1", "?Error:")]
    public void DirectErrorsDoNotExposeSyntheticLineNumbers(string source, string prefix)
    {
        var output = Run(source, runtime: false);
        StringAssert.Contains(output, prefix);
        Assert.IsFalse(output.Contains("line ", StringComparison.OrdinalIgnoreCase), output);
        Assert.IsFalse(output.Contains("column ", StringComparison.OrdinalIgnoreCase), output);
    }

    [DataTestMethod]
    [DataRow("HEJ", "Line 37,")]
    [DataRow("IF 1 THEN", "Line 37,")]
    [DataRow("PRINT 1/0", "in line 37:")]
    [DataRow("PRINT \"A\"-1", "in line 37:")]
    public void StoredProgramErrorsRetainTheirLineNumber(string statement, string location)
    {
        var output = Run("37 " + statement);
        StringAssert.Contains(output, location);
        Assert.IsFalse(output.Contains("Line 0"), output);
    }

    [DataTestMethod]
    [DataRow("10 PRINT 123", true, "123\n")]
    [DataRow("PRINT 123", false, "123\n")]
    [DataRow("10 PRINT \"HELLO\";\"WORLD\"", true, "HELLOWORLD\n")]
    [DataRow("10 PRINT -1", true, "-1\n")]
    [DataRow("10 PRINT -2^2", true, "-4\n")]
    [DataRow("10 PRINT 2^-2", true, "0.25\n")]
    [DataRow("10 PRINT 2*(3+4)", true, "14\n")]
    [DataRow("10 LET A=3\n20 PRINT A", true, "3\n")]
    [DataRow("10 LET A=3\r\n20 PRINT A", true, "3\n")]
    [DataRow("10 LET A=3\r20 PRINT A", true, "3\n")]
    [DataRow("10 IF 1 THEN PRINT \"YES\" ELSE PRINT \"NO\"", true, "YES\n")]
    [DataRow("10 IF 0 THEN PRINT \"YES\" ELSE PRINT \"NO\"", true, "NO\n")]
    [DataRow("10 GOTO 30\n20 PRINT \"NO\"\n30 PRINT \"YES\"", true, "YES\n")]
    [DataRow("10 FOR I=1 TO 3\n20 PRINT I;\n30 NEXT I", true, "123")]
    [DataRow("10 FOR I=3 TO 1 STEP -1\n20 PRINT I;\n30 NEXT I", true, "321")]
    [DataRow("10 FOR I=1 TO 2:FOR J=1 TO 2:PRINT I;J;:NEXT J,I", true, "11122122")]
    [DataRow("10 FOR I=4 TO 1:PRINT \"NO\":NEXT I\n20 PRINT \"YES\"", true, "YES\n")]
    [DataRow("10 DIM A(1,2):A(1,2)=7:PRINT A(1,2)", true, "7\n")]
    public void ExecutesValidPrograms(string source, bool runtime, string expected)
    {
        var output = Run(source, runtime);
        Assert.IsTrue(output.StartsWith(expected), output);
        Assert.IsFalse(output.Contains("?Error") || output.Contains("?Syntax"), output);
    }

    [DataTestMethod]
    [DataRow("10 PRINT \"HELLO")]
    [DataRow("10 PRINT 1+")]
    [DataRow("10 IF 1 THEN")]
    [DataRow("10 UNKNOWN")]
    [DataRow("10 PRINT (1")]
    [Timeout(2000)]
    public void InvalidSyntaxReportsAnErrorWithoutHanging(string source)
    {
        var result = new BasicParser(source).Result;
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.Count > 0);
        StringAssert.Contains(Run(source), "?Syntax error");
    }

    [DataTestMethod]
    [DataRow("10 A=\"HELLO\"\n20 PRINT \"SHOULD NOT RUN\"", "Type mismatch")]
    [DataRow("10 DIM A(1,1):A(0,2)=99\n20 PRINT \"SHOULD NOT RUN\"", "subscript")]
    [DataRow("10 PRINT 1/0\n20 PRINT \"SHOULD NOT RUN\"", "zero")]
    [DataRow("10 GOTO 999", "Undefined line 999")]
    [DataRow("10 NEXT I", "NEXT")]
    [DataRow("10 FOR I=1 TO 2", "FOR")]
    [DataRow("10 FOR I=1 TO 2 STEP 0:NEXT I", "STEP")]
    [DataRow("10 FOR I=1 TO 2:NEXT J", "NEXT")]
    public void RuntimeErrorsStopTheProgram(string source, string message)
    {
        var output = Run(source);
        StringAssert.Contains(output, "?Error");
        StringAssert.Contains(output, message);
        Assert.IsFalse(output.Contains("SHOULD NOT RUN"), output);
    }

    [TestMethod]
    public void TypeErrorDoesNotOverwriteVariable()
    {
        var state = new RuntimeState();
        state.Variables["A"] = new IntValue(42);
        new Interpreter("A=\"HELLO\"", false, state).Run(new Terminal());
        Assert.AreEqual("42", state.Variables["A"]!.ToString());
    }

    [TestMethod]
    public void ArrayRejectsInvalidShapeAndEachSubscript()
    {
        var array = new Dimension(new[] { 1, 1 });
        array.Add(7, 1, 0);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.Add(99, 0, 2));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => array.Add(99, -1, 1));
        Assert.ThrowsException<ArgumentException>(() => array.Get(0));
        Assert.ThrowsException<ArgumentException>(() => array.Get(0, 0, 0));
        Assert.ThrowsException<ArgumentException>(() => new Dimension(new[] { -1 }));
        Assert.AreEqual(7, array.Get(1, 0));
    }

    [TestMethod]
    public void SessionRoutesListRunAndNewAndKeepsDirectVariables()
    {
        var session = new BasicSession();
        var terminal = new Terminal();
        var source = "10 PRINT 7";
        var listed = false;
        void Execute(string command) => session.Execute(command, terminal, () => source,
            () => listed = true, () => source = "");
        Execute("LIST"); Assert.IsTrue(listed);
        Execute("A=42"); terminal.Output.Clear();
        Execute("PRINT A"); StringAssert.StartsWith(terminal.Output.ToString(), "42\n");
        terminal.Output.Clear(); Execute("run");
        StringAssert.StartsWith(terminal.Output.ToString(), "7\n");
        terminal.Output.Clear(); Execute("PRINT A");
        StringAssert.StartsWith(terminal.Output.ToString(), "0\n");
        Execute("NEW"); Assert.AreEqual("", source);
    }

    [TestMethod]
    public void BreakStopsLoopAndSessionRejectsReentrantCommands()
    {
        var session = new BasicSession();
        var terminal = new Terminal();
        var pumps = 0;
        terminal.Pump = () =>
        {
            session.Execute("PRINT 999", terminal, () => "", () => {}, () => {});
            if (++pumps == 20) session.Break();
        };
        session.Execute("RUN", terminal, () => "10 GOTO 10", () => {}, () => {});
        StringAssert.Contains(terminal.Output.ToString(), "User break");
        Assert.IsFalse(terminal.Output.ToString().Contains("999"));
        Assert.IsFalse(session.IsRunning);
    }

    [TestMethod]
    public void InputPreservesNumericLookingStrings()
    {
        var terminal = new Terminal();
        terminal.Inputs.Enqueue("00123");
        new Interpreter("10 INPUT A$\n20 PRINT A$", true).Run(terminal);
        StringAssert.Contains(terminal.Output.ToString(), "00123");
    }
}
