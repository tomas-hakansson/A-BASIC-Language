#nullable enable
using System;
using System.IO;
using System.Text.RegularExpressions;
using A_BASIC_Language.IO;

namespace A_BASIC_Language.Language;

/// <summary>Routes direct commands and owns the variables shared by the interactive session.</summary>
public sealed class BasicSession
{
    private readonly RuntimeState _state = new();
    private Interpreter? _interpreter;
    public bool IsRunning { get; private set; }
    public DirectoryInfo? DataDirectory;
    public FileInfo? ProgramFile;

    public void CreateDataDirectories(Action<string> writeLine)
    {
        var games101Path = Paths.GetUser101GamesPath();

        if (games101Path == null)
        {
            writeLine("Could not create 101 Games directory.");
        }
        else
        {
            try
            {
                var games101SourcePath = Paths.GetGamesSourceDirectory();

                if (games101SourcePath is not { Exists: true })
                {
                    writeLine("Could not find 101 Games source directory.");
                }
                else
                {
                    foreach (var file in games101SourcePath.GetFiles("*.bas"))
                    {
                        var destFile = new FileInfo(Path.Combine(games101Path.FullName, file.Name));
                        
                        if (!destFile.Exists)
                            file.CopyTo(destFile.FullName);
                    }
                }
            }
            catch
            {
                writeLine("Failed to create 101 Games directory.");
            }
        }

        try
        {
            var userPrograms = Paths.GetUserProgramsPath();

            if (userPrograms is not { Exists: true })
                writeLine("Could not create user programs directory.");
        }
        catch
        {
            writeLine("Failed to create user programs directory.");
        }
    }

    public void Break() =>
        _interpreter?.UserBreak = true;

    public void Execute(string command, Log log, IBasicTerminal terminal, Func<string> getProgram, Action<int?, int?> listProgram, Action clearProgram)
    {
        // WinForms message pumping can deliver another Enter while a program is running.
        if (IsRunning || string.IsNullOrWhiteSpace(command))
            return;

        command = command.Trim();
        
        if (command.Equals("LIST", StringComparison.OrdinalIgnoreCase) || (command.StartsWith("LIST", StringComparison.OrdinalIgnoreCase) && command.Length > 4 && (char.IsWhiteSpace(command[4]) || (command[4] >= '0' && command[4] <= '9') || command[4] == '-')))
        {
            if (TryParseListRange(command.Substring(4), out var first, out var last))
                listProgram(first, last);
            else
                terminal.WriteLine("?Invalid LIST range");

            terminal.WriteLine("");
            terminal.WriteLine("Ready.");
            return;
        }

        if (command.Equals("NEW", StringComparison.OrdinalIgnoreCase))
        {
            clearProgram();
            Reset(terminal);
            DataDirectory = null;
            ProgramFile = null;
            terminal.WriteLine("");
            terminal.WriteLine("Ready.");
            return;
        }

        var runtime = command.Equals("RUN", StringComparison.OrdinalIgnoreCase);
        
        if (runtime)
            Reset(terminal);
        
        IsRunning = true;
        
        try
        {
            _interpreter = new Interpreter(runtime ? getProgram() : command, runtime, log, _state);
            _interpreter.Run(terminal);
        }
        finally
        {
            _interpreter = null;
            IsRunning = false;
        }
    }

    private void Reset(IBasicTerminal terminal)
    {
        _state.Clear();
        terminal.ClearScreen();
        terminal.SetTextRenderLimit(0);
    }

    public bool SetFolders(FileInfo basicFileInfo)
    {
        DataDirectory = Paths.GetProgramDataPath(basicFileInfo);
        ProgramFile = basicFileInfo;

        try
        {
            var programDirectory = ProgramFile.Directory;

            if (programDirectory == null)
                return false;

            var filename = basicFileInfo.Name.ToLower();

            if (!DataDirectory.Exists)
                DataDirectory.Create();
        }
        catch
        {
            return false;
        }

        return true;
    }

    private static bool TryParseListRange(string argument, out int? first, out int? last)
    {
        first = last = null;
        argument = argument.Trim();
        
        if (argument.Length == 0)
            return true;

        var match = Regex.Match(argument, @"\A(?:([0-9]+)|([0-9]*)\s*-\s*([0-9]*))\z");

        if (!match.Success)
            return false;

        if (match.Groups[1].Success)
        {
            if (!int.TryParse(match.Groups[1].Value, out var line))
                return false;

            first = last = line;
            return true;
        }

        var firstText = match.Groups[2].Value;
        var lastText = match.Groups[3].Value;

        if (firstText.Length == 0 && lastText.Length == 0)
            return false;

        if (firstText.Length > 0)
        {
            if (!int.TryParse(firstText, out var line))
                return false;
            first = line;
        }

        if (lastText.Length > 0)
        {
            if (!int.TryParse(lastText, out var line))
                return false;
            last = line;
        }

        return !first.HasValue || !last.HasValue || first.Value <= last.Value;
    }

    public RuntimeState GetRuntimeState() =>
        _state;
}
