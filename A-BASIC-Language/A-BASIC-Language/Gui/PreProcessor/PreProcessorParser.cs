using System.Text.RegularExpressions;
using A_BASIC_Language.StringManipulation;

namespace A_BASIC_Language.Gui.PreProcessor;

public class PreProcessorParser
{
    private readonly string _command;

    public PreProcessorParser(string? command)
    {
        _command = (command ?? "").Trim().ToUpper();
    }

    public PreProcessorParserResult Parse(out string message)
    {
        message = "Did not understand that.";
        var c = Regex.Replace(_command, @"\s+", " ");
        var parts = c.Split(' ');

        switch (parts.Length)
        {
            case 1:
                switch (parts[0])
                {
                    case "FULLSCREEN":
                        message = "Toggle fullscreen.";
                        return PreProcessorParserResult.ToggleFullscreen;
                    case "LOG":
                        message = "Toggle debug output.";
                        return PreProcessorParserResult.ToggleLog;
                    case "QUIT":
                        message = "Quit.";
                        return PreProcessorParserResult.Quit;
                    case "RES":
                        message = "Toggle resolution.";
                        return PreProcessorParserResult.ToggleResolution;
                    case "LIST":
                        message = "List source code.";
                        return PreProcessorParserResult.List;
                    default:
                        return PreProcessorParserResult.Unknown;
                }
            case 2:
                switch (parts[0])
                {
                    case "FULLSCREEN":
                        if (parts[1].IsTrue())
                        {
                            message = "Switch to fullscreen.";
                            return PreProcessorParserResult.FullscreenOn;
                        }
                        if (parts[1].IsFalse())
                        {
                            message = "Switch to windowed mode.";
                            return PreProcessorParserResult.FullscreenOff;
                        }
                        return PreProcessorParserResult.Unknown;
                    case "LOG":
                        if (parts[1].IsTrue())
                        {
                            message = "Turn on debug output.";
                            return PreProcessorParserResult.LogOn;
                        }
                        if (parts[1].IsFalse())
                        {
                            message = "Turn off debug output.";
                            return PreProcessorParserResult.LogOff;
                        }
                        return PreProcessorParserResult.Unknown;
                    case "RES":
                        switch (parts[1])
                        {
                            case "1":
                                return PreProcessorParserResult.Resolution1;
                            case "2":
                                return PreProcessorParserResult.Resolution2;
                            case "3":
                                return PreProcessorParserResult.Resolution3;
                            case "4":
                                return PreProcessorParserResult.Resolution4;
                            default:
                                message = "Did not understand that.";
                                return PreProcessorParserResult.Unknown;
                        }
                    default:
                        message = "Did not understand that.";
                        return PreProcessorParserResult.Unknown;
                }
            default:
                message = "Did not understand that.";
                return PreProcessorParserResult.Unknown;
        }
    }
}