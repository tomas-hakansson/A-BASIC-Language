#nullable enable
using A_BASIC_Language.Gui.PreProcessor;
using A_BASIC_Language.StringManipulation;
using A_BASIC_Language.TerminalMatrix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using A_BASIC_Language.IO;
using TerminalMatrixNetFramework;

namespace A_BASIC_Language.MainWindowControllers;

public class PreProcessorController
{
    private readonly TerminalMatrixControl _parent;
    private readonly MainWindow _owner;

    public PreProcessorController(TerminalMatrixControl parent, MainWindow owner)
    {
        _parent = parent;
        _owner = owner;
    }

    public void Run(string preProcessor, out bool quitFlag)
    {
        quitFlag = false;
        preProcessor = preProcessor.Substring(1).Trim();

        if (preProcessor.IsEmpty())
        {
            if (_parent.Resolution == Resolution.Pixels160x200Characters20x25)
            {
                _parent.WriteLine("Don't know what to");
                _parent.WriteLine("do with that.");
            }
            else
            {
                _parent.WriteLine("Don't know what to do with that.");
            }

            return;
        }

        var preProcessorParser = new PreProcessorParser(preProcessor);
        var response = preProcessorParser.Parse(out var message, out var argument);
        _parent.WriteLine(message);

        switch (response)
        {
            case PreProcessorParserResult.Unknown:
                _parent.WriteLine("Unknown command.");
                _parent.WriteLine("Help: https://abl.winsoft.se/");
                break;
            case PreProcessorParserResult.QuietError:
                _parent.WriteLine("Help: https://abl.winsoft.se/");
                break;
            case PreProcessorParserResult.ToggleFullscreen:
                _owner.ToggleFullscreen(false);
                break;
            case PreProcessorParserResult.FullscreenOn:
                if (_owner.fullscreenToolStripMenuItem.Checked)
                    _parent.WriteLine("No action required.");
                else
                    _owner.ToggleFullscreen(false);
                break;
            case PreProcessorParserResult.FullscreenOff:
                if (!_owner.fullscreenToolStripMenuItem.Checked)
                    _parent.WriteLine("No action required.");
                else
                    _owner.ToggleFullscreen(false);
                break;
            case PreProcessorParserResult.HighQualityRenderToggle:
                _owner.highQualityRenderingToolStripMenuItem_Click(this, EventArgs.Empty);
                break;
            case PreProcessorParserResult.HighQualityRenderOn:
                if (_owner.highQualityRenderingToolStripMenuItem.Checked)
                    _parent.WriteLine("No action required.");
                else
                    _owner.highQualityRenderingToolStripMenuItem_Click(this, EventArgs.Empty);
                break;
            case PreProcessorParserResult.HighQualityRenderOff:
                if (!_owner.highQualityRenderingToolStripMenuItem.Checked)
                    _parent.WriteLine("No action required.");
                else
                    _owner.highQualityRenderingToolStripMenuItem_Click(this, EventArgs.Empty);
                break;
            case PreProcessorParserResult.ToggleLog:
                _owner.debugOutputToolStripMenuItem_Click(this, EventArgs.Empty);
                break;
            case PreProcessorParserResult.LogOn:
                if (_owner.debugOutputToolStripMenuItem.Checked)
                    _parent.WriteLine("No action required.");
                else
                    _owner.debugOutputToolStripMenuItem_Click(this, EventArgs.Empty);
                break;
            case PreProcessorParserResult.LogOff:
                if (!_owner.debugOutputToolStripMenuItem.Checked)
                    _parent.WriteLine("No action required.");
                else
                    _owner.debugOutputToolStripMenuItem_Click(this, EventArgs.Empty);
                break;
            case PreProcessorParserResult.ToggleResolution:
                switch (_parent.Resolution)
                {
                    case Resolution.Pixels640x200Characters80x25:
                        _parent.SetResolution(Resolution.Pixels480x200Characters60x25);
                        _parent.WriteLine("Resolution changed. Is now:");
                        _parent.WriteLine("480 * 200 pixels (60 * 25 characters)");
                        break;
                    case Resolution.Pixels480x200Characters60x25:
                        _parent.SetResolution(Resolution.Pixels320x200Characters40x25);
                        _parent.WriteLine("Resolution changed. Is now:");
                        _parent.WriteLine("320 * 200 pixels (40 * 25 characters)");
                        break;
                    case Resolution.Pixels320x200Characters40x25:
                        _parent.SetResolution(Resolution.Pixels160x200Characters20x25);
                        _parent.WriteLine("Resolution changed. Is now:");
                        _parent.WriteLine("160 * 200 pixels");
                        _parent.WriteLine("(40 * 25 characters)");
                        break;
                    case Resolution.Pixels160x200Characters20x25:
                        _parent.SetResolution(Resolution.Pixels640x200Characters80x25);
                        _parent.WriteLine("Resolution changed. Is now:");
                        _parent.WriteLine("640 * 200 pixels (80 * 25 characters)");
                        break;
                    case Resolution.LogPixels640x80Characters80x10: // Not in the cycle loop.
                        _parent.SetResolution(Resolution.Pixels640x200Characters80x25);
                        _parent.WriteLine("Resolution changed. Is now:");
                        _parent.WriteLine("640 * 200 pixels (80 * 25 characters)");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _owner.CheckResolutionBox();
                break;
            case PreProcessorParserResult.Resolution1:
                _parent.SetResolution(Resolution.Pixels640x200Characters80x25);
                _parent.WriteLine("Resolution changed. Is now:");
                _parent.WriteLine("640 * 200 pixels (80 * 25 characters)");
                break;
            case PreProcessorParserResult.Resolution2:
                _parent.SetResolution(Resolution.Pixels480x200Characters60x25);
                _parent.WriteLine("Resolution changed. Is now:");
                _parent.WriteLine("480 * 200 pixels (60 * 25 characters)");
                break;
            case PreProcessorParserResult.Resolution3:
                _parent.SetResolution(Resolution.Pixels320x200Characters40x25);
                _parent.WriteLine("Resolution changed. Is now:");
                _parent.WriteLine("320 * 200 pixels (40 * 25 characters)");
                break;
            case PreProcessorParserResult.Resolution4:
                _parent.SetResolution(Resolution.Pixels160x200Characters20x25);
                _parent.WriteLine("Resolution changed. Is now:");
                _parent.WriteLine("160 * 200 pixels");
                _parent.WriteLine("(40 * 25 characters)");
                break;
            case PreProcessorParserResult.List:
                _parent.List();
                break;
            case PreProcessorParserResult.Show101Games:
                Show101Games();
                break;
            case PreProcessorParserResult.Load101Games:
                Load101Games(argument ?? "");
                break;
            case PreProcessorParserResult.Quit:
                _owner.DisableQuitPrompt();
                quitFlag = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Show101Games()
    {
        try
        {
            var dir = Paths.GetGamesSourceDirectory();

            if (dir == null)
            {
                _parent.WriteLine("?Failed to show 101 games");
                return;
            }

            var games = dir.GetFiles();

            var basFiles = (
                    from fileInfo in games
                    where fileInfo.Extension.Equals(".bas", StringComparison.OrdinalIgnoreCase)
                    select fileInfo.Name.Substring(0, fileInfo.Name.Length - 4)
                ).ToList();

            if (basFiles.Count <= 0)
            {
                _parent.WriteLine("?No games found.");
                return;
            }

            var s = new StringBuilder();

            foreach (var basFile in basFiles)
                s.AppendLine(basFile == basFiles.Last() ? basFile : $"{basFile},");

            var result = s.ToString();
            var parts = new List<string>();
            var consoleWidth = _parent.GetCharacterMapSize().Width;

            for (var i = 0; i < result.Length; i += consoleWidth)
                parts.Add(i + consoleWidth > result.Length ? result.Substring(i) : result.Substring(i, consoleWidth));

            foreach (var part in parts)
                _parent.WriteLine(part);
        }
        catch (Exception e)
        {
            _parent.WriteLine("?Failed to show 101 games");
        }
    }

    private void Load101Games(string gameName)
    {
        try
        {
            var gamesDirectory = Paths.GetGamesSourceDirectory();

            if (gamesDirectory == null)
            {
                _parent.WriteLine($"?Failed to load game: {gameName}");
                return;
            }

            var fullPath = Path.Combine(gamesDirectory.FullName, $"{gameName}.bas");
            var source = File.ReadAllText(fullPath);
            _owner.LoadProgram(source);
        }
        catch
        {
            _parent.WriteLine($"?Failed to load game: {gameName}");
        }
    }
}
