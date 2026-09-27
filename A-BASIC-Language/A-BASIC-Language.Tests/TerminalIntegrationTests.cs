using System.Reflection;
using System.Runtime.ExceptionServices;
using A_BASIC_Language.Language;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminalMatrix;
using TerminalMatrix.Definitions;

namespace A_BASIC_Language.Tests;

[TestClass]
[DoNotParallelize]
public class TerminalIntegrationTests
{
    private sealed class TestTerminal : TerminalMatrixControl
    {
        public void Type(string text)
        {
            foreach (var c in text) OnKeyPress(new KeyPressEventArgs(c));
        }
        public void Submit() => OnKeyDown(new KeyEventArgs(Keys.Enter));
    }

    private static void WithTerminal(Action<TestTerminal> test)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var terminal = new TestTerminal();
                terminal.SetResolution(Resolution.Pixels320x200Characters40x25);
                try { test(terminal); }
                finally { terminal.Quit(); }
            }
            catch (Exception ex) { failure = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private static string Row(TerminalMatrixControl terminal, int y)
    {
        var map = (byte[,])typeof(TerminalMatrixControl)
            .GetField("_characterMap", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(terminal)!;
        return new string(Enumerable.Range(0, CharacterMatrixDefinition.Width)
            .Select(x => (char)map[x, y]).ToArray()).TrimEnd();
    }

    [TestMethod]
    public void PrintAppendsUsingTheInstalledTerminalPackage()
    {
        WithTerminal(terminal =>
        {
            new Interpreter("10 PRINT \"HELLO\";\"WORLD\"", true).Run(terminal);
            Assert.AreEqual("HELLOWORLD", Row(terminal, 0));
        });
    }

    [TestMethod]
    public void OutputWrapsAndNewlineDoesNotOverwritePreviousText()
    {
        WithTerminal(terminal =>
        {
            var output = new TerminalAdapter(terminal);
            output.Write(new string('A', 40));
            output.Write("BC");
            output.WriteLine("D");
            output.Write("NEXT");
            Assert.AreEqual(new string('A', 40), Row(terminal, 0));
            Assert.AreEqual("BCD", Row(terminal, 1));
            Assert.AreEqual("NEXT", Row(terminal, 2));
        });
    }

    [TestMethod]
    public void TypingNumberedLineListAndRunUsesTheRealEventPath()
    {
        WithTerminal(terminal =>
        {
            var session = new BasicSession();
            terminal.AutoProgramManagement = true;
            terminal.TypedLine += (_, e) => session.Execute(e.InputValue, new TerminalAdapter(terminal),
                terminal.GetProgramAsString, terminal.List, terminal.New);
            terminal.Type("10 PRINT 123"); terminal.Submit();
            Assert.AreEqual(1, terminal.ProgramLines.Count);
            terminal.Type("LIST"); terminal.Submit();
            Assert.AreEqual("10 PRINT 123", Row(terminal, 2));
            terminal.Type("RUN"); terminal.Submit();
            Assert.AreEqual("123", Row(terminal, 4));
        });
    }
}
