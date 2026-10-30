using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using A_BASIC_Language;
using A_BASIC_Language.Language;
using A_BASIC_Language.TerminalMatrix;
using TerminalMatrixNetFramework;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var status = new ToolStripStatusLabel();
        using var font = new Font(FontFamily.GenericMonospace, 10);
        var log = new Log(status, font);
        // Exercise the application command using the actual window and terminal.
        var settingsType = typeof(MainWindow).Assembly.GetType("A_BASIC_Language.Properties.Settings");
        var settings = settingsType.GetProperty("Default").GetValue(null);
        var themeProperty = settingsType.GetProperty("ColorScheme");
        A_BASIC_Language.Gui.Multimedia.ColorScheme GetTheme() =>
            (A_BASIC_Language.Gui.Multimedia.ColorScheme)themeProperty.GetValue(settings);
        var originalTheme = GetTheme();
        try
        {
            using var window = new MainWindow();
            var themeTerminal = (TerminalMatrixControl)typeof(MainWindow)
                .GetField("terminalMatrixControl1", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
            themeTerminal.SetResolution(Resolution.Pixels320x200Characters40x25);
            var commands = new A_BASIC_Language.MainWindowControllers.PreProcessorController(themeTerminal, window);
            foreach (var theme in new A_BASIC_Language.Gui.Multimedia.ColorSchemeConfigurationList())
            {
                commands.Run("!colortheme " + theme.ShortName.ToLowerInvariant(), out var quit);
                Require(!quit && GetTheme() == theme.Id &&
                    themeTerminal.BackgroundColor == (byte)theme.Background &&
                    themeTerminal.CurrentCursorColor == (byte)theme.Text &&
                    themeTerminal.BorderColor == (byte)theme.Border, "Theme command: " + theme.ShortName);
                themeTerminal.Clear();
                commands.Run("!COLORTHEME", out quit);
                var text = (byte[,])typeof(TerminalMatrixControl)
                    .GetField("_characterMap", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(themeTerminal);
                var rows = Enumerable.Range(0, themeTerminal.CharacterHeight).Select(y =>
                    new string(Enumerable.Range(0, themeTerminal.CharacterWidth).Select(x => (char)text[x, y]).ToArray()));
                Require(rows.Any(row => row.StartsWith(theme.ShortName)), "Theme query: " + theme.ShortName);
            }
            var currentTheme = GetTheme();
            var currentColor = themeTerminal.CurrentCursorColor;
            commands.Run("!COLORTHEME UNKNOWN", out _);
            commands.Run("!COLORTHEME C64 EXTRA", out _);
            Require(GetTheme() == currentTheme &&
                themeTerminal.CurrentCursorColor == currentColor, "Invalid theme changed settings or colors");
        }
        finally
        {
            themeProperty.SetValue(settings, originalTheme);
        }
        var checks = 0;
        foreach (var runtime in new[] { false, true })
        {
            for (var color = 1; color <= 16; color++)
            {
                var terminal = Run("COLOR 1," + color, runtime);
                Require(terminal.Colors.SequenceEqual(new[] { color }) && !terminal.Output.Contains("?"), "Palette " + color);
                checks++;
            }
            foreach (var targetId in new[] { 0, 2 })
                for (var color = 1; color <= 16; color++)
                {
                    var terminal = Run("COLOR " + targetId + "," + color, runtime);
                    Require((targetId == 0 ? terminal.Background : terminal.Border) == color &&
                        terminal.Color == 1 && !terminal.Output.Contains("?"), "Background/border palette");
                    checks++;
                }
            foreach (var source in new[] {
                "color 1,2", "A=1:B=3:COLOR A,B*2-4",
                "IF 1 THEN COLOR 1,2 ELSE COLOR 1,5",
                "IF 0 THEN COLOR 1,5 ELSE COLOR 1,2",
                "DIM A(1):A(0)=2:COLOR 1,A(0)",
                "DEF FNA(X)=X+1:COLOR 1,FNA(1)"
            })
            {
                var terminal = Run(source, runtime);
                Require(terminal.Colors.SequenceEqual(new[] { 2 }) && !terminal.Output.Contains("?"), source);
                checks++;
            }
            foreach (var arguments in new[] {
                "3,2", "0.05,2", "1.05,2", "-1,2", "1.5,2", "1E1000,2", "SQR(-1),2",
                "1,-1", "1,0", "1,17", "1,2.05", "1,2.5", "1,1E1000", "1,SQR(-1)",
                "\"1\",2", "1,\"2\"", "1,1/0",
                "", "1", "1,", ",2", "1;2", "1,2,3", "1,2 3"
            })
            {
                var terminal = Run("COLOR " + arguments + ":PRINT \"WRONG\"", runtime);
                Require(terminal.Colors.Count == 0 && terminal.Output.Contains("?") &&
                    !terminal.Output.Contains("WRONG"), "Invalid arguments accepted: " + arguments);
                checks++;
            }
            var mixed = Run("COLOR 1,2:PRINT \"A\";:COLOR 1,5:PRINT \"B\"", runtime);
            Require(mixed.Output.StartsWith("AB") && mixed.Characters.Take(2).SequenceEqual(new[] { ("A", 2), ("B", 5) }),
                "Color change interrupted PRINT or recolored old output");
            var input = Run("COLOR 1,7:INPUT A$:PRINT A$", runtime);
            Require(input.InputColor == 7 && input.Output.Contains("ANSWER"), "INPUT color");
            var invalid = Run("COLOR 1,2:COLOR 1,17", runtime);
            Require(invalid.Color == 2 && invalid.Colors.Count == 1, "Invalid COLOR changed the existing color");
        }

        var session = new BasicSession();
        var target = new FakeTerminal();
        foreach (var command in new[] { "COLOR 1,5", "PRINT 1", "RUN", "NEW" })
            session.Execute(command, log, target, () => "10 PRINT 2", (_, __) => { }, () => { });
        Require(target.Color == 5 && target.Colors.SequenceEqual(new[] { 5 }), "Session did not retain the color");

        // Verify the real adapter stores individual character colors through wrapping and scrolling.
        using var control = new TerminalMatrixControl();
        control.SetResolution(Resolution.Pixels320x200Characters40x25);
        var adapter = new TerminalAdapter(control);
        new Interpreter("COLOR 1,2:PRINT \"A\";:COLOR 1,5:PRINT \"B\";", false, log, new RuntimeState()).Run(adapter);
        // Direct mode appends Ready; the first two characters must still retain their colors.
        var map = (byte[,])typeof(TerminalMatrixControl).GetField("_characterColorMap", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(control);
        var chars = (byte[,])typeof(TerminalMatrixControl).GetField("_characterMap", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(control);
        var found = false;
        for (var y = 0; y < control.CharacterHeight; y++)
            if (chars[0, y] == 'A' && chars[1, y] == 'B')
            {
                Require(map[0, y] == 1 && map[1, y] == 4, "Real terminal lost per-character colors");
                found = true;
            }
        Require(found, "Real terminal output missing");
        adapter.ClearScreen();
        for (var row = 0; row < 30; row++)
        {
            adapter.SetTextColor(row % 16 + 1);
            adapter.WriteLine(new string('X', 41));
        }
        map = (byte[,])typeof(TerminalMatrixControl).GetField("_characterColorMap", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(control);
        chars = (byte[,])typeof(TerminalMatrixControl).GetField("_characterMap", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(control);
        Require(chars[0, 22] == 'X' && chars[0, 23] == 'X' && map[0, 22] == 13 && map[0, 23] == 13,
            "Wrapping or scrolling lost the color");
        control.BorderWidth = 8;
        control.BorderHeight = 8;
        control.SetResolution(Resolution.Pixels320x200Characters40x25);
        new Interpreter("COLOR 0,7:COLOR 2,3:COLOR 1,16:PRINT \"X\";", false, log, new RuntimeState()).Run(adapter);
        control.UpdateBitmap();
        var bitmap = (Bitmap)typeof(TerminalMatrixControl).GetField("_fastBitmap", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(control);
        Require(control.BackgroundColor == 6 && control.BorderColor == 2 && control.CurrentCursorColor == 15,
            "Adapter did not convert all three colors to palette indices");
        Require(bitmap.GetPixel(0, 0).ToArgb() == System.Drawing.Color.FromArgb(136, 0, 0).ToArgb(), "Border pixels");
        Require(bitmap.GetPixel(16, 16).ToArgb() == System.Drawing.Color.FromArgb(0, 0, 170).ToArgb(), "Background pixels");
        adapter.ClearScreen();
        control.SetResolution(Resolution.Pixels320x200Characters40x25);
        Require(control.BackgroundColor == 6 && control.BorderColor == 2, "Clear/resolution reset colors");
        Console.WriteLine($"COLOR checks passed ({checks} cases plus output, INPUT, session and real terminal checks).");

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
        public int Color { get; private set; } = 1;
        public int InputColor { get; private set; }
        public List<int> Colors { get; } = new();
        public List<(string, int)> Characters { get; } = new();
        public string Output { get; private set; } = "";
        public int Background { get; private set; } = 1;
        public int Border { get; private set; } = 1;
        public void SetBackgroundColor(int color) { Background = color; Colors.Add(color); }
        public void SetBorderColor(int color) { Border = color; Colors.Add(color); }
        public void SetTextColor(int color) { Color = color; Colors.Add(color); }
        public void ClearScreen() { }
        public void SetTextRenderLimit(int limit) { }
        public void Write(string text)
        {
            Output += text;
            foreach (var character in text) Characters.Add((character.ToString(), Color));
        }
        public void WriteLine(string text) { Write(text); Write("\n"); }
        public string InputString(string prompt) { InputColor = Color; return "ANSWER"; }
        public void PumpEvents() { }
    }
}
