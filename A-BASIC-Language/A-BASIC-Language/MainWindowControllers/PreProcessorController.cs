using A_BASIC_Language.Gui.PreProcessor;
using A_BASIC_Language.StringManipulation;
using TerminalMatrix;

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
        preProcessor = preProcessor[1..].Trim();

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
        var response = preProcessorParser.Parse(out var message);
        _parent.WriteLine(message);

        switch (response)
        {
            case PreProcessorParserResult.Unknown:
                _parent.WriteLine("Unknown command.");
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
            case PreProcessorParserResult.Quit:
#if !DEBUG
                    _promptQuit = false;
#endif
                quitFlag = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}