#if DEBUG
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using A_BASIC_Language.TerminalMatrix;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_BASIC_Language.Language;

[TestClass]
public class ContinuationTests
{
    [TestMethod]
    public void IntegerDivisionTruncatesAndHasItsOwnPrecedence()
    {
        var host = new Host("");
        foreach (var (expression, expected) in new (string, double)[]
        {
            (@"7\2", 3), (@"-7\2", -3), (@"7\-2", -3), (@"-7\-2", 3),
            (@"7.9\2.9", 3), (@"-7.9\2.9", -3), (@"1\2", 0),
            (@"20\3\2", 3), (@"20\3*2", 3), (@"20\4/2", 10),
            (@"2+9\2", 6), (@"9\2+2", 6), (@"2^3\3", 2),
            (@"(20\3)*2", 12), (@"NOT 7\2", -4), (@"7\2=3", -1),
            (@"2147483647\1", 2147483647), (@"(-2147483648)\1", -2147483648d),
            (@"7/2", 3.5)
        })
        {
            host.Execute("RESULT=" + expression);
            Assert.AreEqual(expected, host.Number("RESULT"), expression);
        }
        Assert.IsFalse(host.Terminal.Output.ToString().Contains("?"));
    }

    [TestMethod]
    public void IntegerDivisionWorksInProgramsFunctionsAndArraySubscripts()
    {
        var host = new Host("10 DIM A(3)\n20 DEF FNF(X)=X\\2\n30 A(7\\2)=FNF(9)\n40 IF A(3)=4 THEN B=1\n50 PRINT \"A\\B\"");
        host.Execute("RUN");
        Assert.AreEqual(4, host.Session.GetRuntimeState().Arrays["A"].Get(3));
        Assert.AreEqual(1d, host.Number("B"));
        StringAssert.Contains(host.Terminal.Output.ToString(), @"A\B");
        Assert.IsFalse(host.Terminal.Output.ToString().Contains("?Error"));
    }

    [TestMethod]
    public void IntegerDivisionRejectsZeroOverflowAndInvalidOperands()
    {
        foreach (var expression in new[] { @"1\0", @"1\0.9", @"1\-0.9", @"2147483648\1", @"1\2147483648", @"(-2147483648)\-1", @"SQR(-1)\1", "\"7\"\\2", @"7\", @"\2" })
        {
            var host = new Host("");
            host.Execute("RESULT=" + expression);
            StringAssert.Contains(host.Terminal.Output.ToString(), "?", expression);
            Assert.IsFalse(host.Session.GetRuntimeState().Variables.ContainsKey("RESULT"), expression);
        }
    }

    [TestMethod]
    public void NotAndExclamationComplementIntegersWithBasicPrecedence()
    {
        var host = new Host("");
        foreach (var (expression, expected) in new (string, double)[]
        {
            ("NOT 0", -1), ("!0", -1), ("NOT -1", 0), ("!-1", 0),
            ("NOT 1", -2), ("!5", -6), ("NOT 1.9", -2), ("NOT -1.9", 0),
            ("NOT NOT 5", 5), ("!!5", 5), ("NOT !5", 5),
            ("NOT 2=1", -1), ("!2=1", -1), ("NOT 1=1", 0),
            ("NOT 1 AND 2", 2), ("!1 AND 2", 2), ("NOT 1 OR 2", -2),
            ("NOT (1 AND 2)", -1), ("NOT 1+2*3", -8),
            ("1!=2", -1), ("1!=1", 0), ("1<>2", -1),
            ("NOT 2147483647", -2147483648d), ("NOT (-2147483648)", 2147483647),
            ("not 0", -1), ("NOT0", -1), ("NOT123", -124),
            ("not0", -1), ("NOT0.5", -1), ("NOT1E2", -101), ("NOT0=1", -1)
        })
        {
            host.Execute("RESULT=" + expression);
            Assert.AreEqual(expected, host.Number("RESULT"), expression);
        }
        host.Execute("NOTABLE=7:RESULT=NOTABLE");
        Assert.AreEqual(7d, host.Number("RESULT"));
        host.Execute("NOTABLE4EVER=42:RESULT=NOTABLE4EVER");
        Assert.AreEqual(42d, host.Number("RESULT"));
        host.Execute("A1=9:RESULT=A1");
        Assert.AreEqual(9d, host.Number("RESULT"));
        Assert.IsFalse(host.Terminal.Output.ToString().Contains("?"));
    }

    [TestMethod]
    public void NotWorksInConditionsFunctionsAndNumberedPrograms()
    {
        var host = new Host("10 DEF FNF(X)=!X\n20 IF NOT 2=1 THEN A=FNF(0)\n30 IF !(3=3) THEN B=1 ELSE B=2\n40 C=NOT (\"A\"=\"B\")");
        host.Execute("RUN");
        Assert.AreEqual(-1d, host.Number("A"));
        Assert.AreEqual(2d, host.Number("B"));
        Assert.AreEqual(-1d, host.Number("C"));
        Assert.IsFalse(host.Terminal.Output.ToString().Contains("?Error"));
    }

    [TestMethod]
    public void NotRejectsInvalidOperandsAndMissingExpressions()
    {
        foreach (var expression in new[] { "NOT \"1\"", "!\"text\"", "NOT 2147483648", "NOT (-2147483649)", "NOT SQR(-1)", "NOT", "!", "!!", "!=1" })
        {
            var host = new Host("");
            host.Execute("RESULT=" + expression);
            StringAssert.Contains(host.Terminal.Output.ToString(), "?", expression);
            Assert.IsFalse(host.Session.GetRuntimeState().Variables.ContainsKey("RESULT"), expression);
        }
    }

    [TestMethod]
    public void StopResumesWithinLineAndPreservesLoopsSubroutinesAndData()
    {
        var host = new Host("10 DATA 7,8\n20 FOR I=1 TO 2\n30 GOSUB 100\n40 NEXT I\n50 END\n100 READ A:PRINT A:STOP:PRINT I:RETURN");
        host.Execute("RUN");
        Assert.IsTrue(host.Session.CanContinue);
        host.Execute("PRINT A");
        host.Execute("A=A+1");
        host.Execute("CONT");
        Assert.IsTrue(host.Session.CanContinue);
        Assert.AreEqual(8d, host.Number("A"));
        host.Execute("CONT");
        Assert.IsFalse(host.Session.CanContinue);
        Assert.AreEqual(3d, host.Number("I"));
        Assert.AreEqual(2, host.Session.GetRuntimeState().DataPointer);
        Assert.IsFalse(host.Terminal.Output.ToString().Contains("?Error"));
    }

    [TestMethod]
    public void StopDoesNotRepeatEarlierStatementsAndCanPauseAtEndOfProgram()
    {
        var host = new Host("10 A=A+1:STOP:A=A+10:STOP");
        host.Execute("RUN");
        Assert.AreEqual(1d, host.Number("A"));
        host.Execute("CONT");
        Assert.AreEqual(11d, host.Number("A"));
        Assert.IsTrue(host.Session.CanContinue);
        host.Execute("CONT");
        Assert.IsFalse(host.Session.CanContinue);
        Assert.AreEqual(11d, host.Number("A"));
    }

    [TestMethod]
    public void BreakDuringSleepResumesAtFollowingInstruction()
    {
        var host = new Host("10 A=A+1:SLEEP 10000:A=A+10");
        host.Terminal.OnPump = () => host.Session.Break();
        // The first two pumps happen before instruction execution.
        host.Terminal.SkipPumps = 2;
        host.Execute("RUN");
        Assert.IsTrue(host.Session.CanContinue);
        Assert.AreEqual(1d, host.Number("A"));
        host.Terminal.OnPump = null;
        host.Execute("CONT");
        Assert.AreEqual(11d, host.Number("A"));
        Assert.IsFalse(host.Session.CanContinue);
    }

    [TestMethod]
    public void InterruptedInputRestartsPromptAndPreservesArrayAndLoopState()
    {
        var host = new Host("10 DIM A(2)\n20 FOR I=1 TO 2\n30 INPUT \"VALUE\";A(I),B$\n40 NEXT I\n50 END");
        host.Terminal.Inputs.Enqueue("3"); // Partial input, then break at the continuation prompt.
        host.Terminal.OnInput = () => host.Session.Break();
        host.Execute("RUN");
        Assert.IsTrue(host.Session.CanContinue);
        Assert.AreEqual(1d, host.Number("I"));
        host.Terminal.OnInput = null;
        host.Terminal.Inputs.Enqueue("7,first");
        host.Terminal.Inputs.Enqueue("8,second");
        host.Execute("CONT");
        Assert.AreEqual(7, host.Session.GetRuntimeState().Arrays["A"].Get(1));
        Assert.AreEqual(8, host.Session.GetRuntimeState().Arrays["A"].Get(2));
        Assert.IsFalse(host.Session.CanContinue);
        Assert.AreEqual(3, host.Terminal.Output.ToString().Split(new[] { "VALUE" }, StringSplitOptions.None).Length - 1);
        Assert.IsFalse(host.Terminal.Output.ToString().Contains("?Error"));
    }

    [TestMethod]
    public void BreakDuringFunctionEvaluationRetainsPendingOperandsAndCalls()
    {
        var body = string.Join("+", Enumerable.Repeat("1", 2000));
        var host = new Host("10 DEF FNF(X)=X+" + body + "\n20 FOR I=1 TO 100\n30 A=A+FNF(I)\n40 NEXT I\n50 END");
        host.Terminal.SkipPumps = 2;
        host.Terminal.OnPump = () => host.Session.Break();
        host.Execute("RUN");
        Assert.IsTrue(host.Session.CanContinue);
        host.Terminal.OnPump = null;
        host.Execute("CONT");
        Assert.IsFalse(host.Session.CanContinue);
        Assert.AreEqual(205050d, host.Number("A"));
        Assert.IsFalse(host.Terminal.Output.ToString().Contains("?Error"));
    }

    [TestMethod]
    public void EditingAndRestoringTheSameLineStillInvalidatesContinuation()
    {
        var host = new Host("10 STOP");
        var lines = new ProgramLineDictionary();
        lines.Changed += (_, _) => host.Session.InvalidateContinuation();
        host.Execute("RUN");
        Assert.IsTrue(host.Session.CanContinue);
        lines.InsertProgramLine(new ProgramLine("10 END", 10, "END"));
        lines.InsertProgramLine(new ProgramLine("10 STOP", 10, "STOP"));
        Assert.IsFalse(host.Session.CanContinue);
    }

    [TestMethod]
    public void EndErrorsAndNormalCompletionCannotContinue()
    {
        foreach (var source in new[] { "10 END", "10 A=1", "10 A=1/0" })
        {
            var host = new Host(source);
            host.Execute("RUN");
            host.Execute("CONT");
            Assert.IsFalse(host.Session.CanContinue);
            StringAssert.Contains(host.Terminal.Output.ToString(), "?Can't continue.");
        }
    }

    [TestMethod]
    public void EditingNewRunAndResetInvalidateContinuation()
    {
        var host = new Host("10 STOP:A=99");
        host.Execute("RUN");
        host.Source = "10 A=3";
        host.Execute("CONT");
        StringAssert.Contains(host.Terminal.Output.ToString(), "?Can't continue.");
        foreach (var command in new[] { "NEW", "RESET", "RESET HARD" })
        {
            host.Source = "10 STOP:A=99";
            host.Execute("RUN");
            host.Execute(command);
            Assert.IsFalse(host.Session.CanContinue, command);
        }
        host.Source = "10 A=A+1:STOP";
        host.Execute("RUN");
        host.Execute("RUN");
        Assert.AreEqual(1d, host.Number("A"));
        host.Session.InvalidateContinuation();
        Assert.IsFalse(host.Session.CanContinue);
    }

    private sealed class Host
    {
        public readonly BasicSession Session = new();
        public readonly TestTerminal Terminal = new();
        private readonly Log _log = new(new ToolStripStatusLabel(), SystemFonts.DefaultFont);
        public string Source;
        public Host(string source) => Source = source;
        public void Execute(string command) => Session.Execute(command, _log, Terminal, () => Source,
            (_, _) => { }, () => Source = "", source => Source = source);
        public double Number(string name) => (double)Session.GetRuntimeState().Variables[name]
            .GetValueAsType<ValueTypes.FloatValue>();
    }

    private sealed class TestTerminal : IBasicTerminal
    {
        public readonly StringBuilder Output = new();
        public readonly Queue<string> Inputs = new();
        public Action? OnPump;
        public Action? OnInput;
        public int SkipPumps;
        public bool QuitFlag => false;
        public int OutputColumn => 0;
        public Resolution ScreenResolution => Resolution.Pixels640X200Characters80X25;
        public void SetScreenResolution(Resolution resolution, bool preserveContents = false) { }
        public void Write(string text) => Output.Append(text);
        public void WriteLine(string text) => Output.AppendLine(text);
        public void WriteLine(bool blankLineFirst, string text) => WriteLine(text);
        public string InputString(string prompt)
        {
            if (Inputs.Count > 0)
                return Inputs.Dequeue();
            if (OnInput == null)
                throw new InvalidOperationException("Test input exhausted.");
            OnInput();
            return "";
        }
        public void PumpEvents()
        {
            if (SkipPumps > 0) SkipPumps--;
            else OnPump?.Invoke();
        }
        public void ClearScreen() { }
        public void SetTextRenderLimit(int limit) { }
        public void SetCursorVisible(bool visible) { }
        public void ResetAppearance() { }
        public void SetTextColor(int color) { }
        public void SetBackgroundColor(int color) { }
        public void SetBorderColor(int color) { }
        public void MainWindowLoadProgram(string source) { }
        public bool MainWindowSaveProgram(string source) => false;
        public void MainWindowSessionSetFolders(FileInfo basicFileInfo) { }
        public void WriteCharacterAt(int x, int y, char character, int? color = null) { }
        public void CreateBlockSprite(int number, int x, int y, char character, int color) { }
        public void MoveBlockSprite(int number, int x, int y, char? character) { }
        public void MoveBlockSpriteRelative(int number, int relativeX, int relativeY) { }
        public void DeleteBlockSprite(int? number) { }
        public (int X, int Y)? GetBlockSpritePosition(int number) => null;
    }
}
#endif
