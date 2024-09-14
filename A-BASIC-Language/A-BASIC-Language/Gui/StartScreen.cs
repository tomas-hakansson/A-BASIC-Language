using TerminalMatrix;

namespace A_BASIC_Language.Gui;

public class StartScreen
{
    private readonly Resolution _resolution;

    public StartScreen(Resolution resolution)
    {
        _resolution = resolution;
    }

    public void Write(TerminalMatrixControl control)
    {
        control.Clear();

        //TODO: Dafuq is this?!?
        while (control.CursorPosition.Y < 1)
            control.WriteLine("");

        switch (_resolution)
        {
            case Resolution.Pixels640x200Characters80x25:
                control.WriteLine("                             ABL, A BASIC Language");
                control.WriteLine("                  ***** Altair Commodore BASIC Emulator *****");
                control.WriteLine("                    (C)  Tomas Hakansson & Anders Hesselbom");
                control.WriteLine("");
                control.WriteLine("Ready.");
                break;
            case Resolution.Pixels480x200Characters60x25:
                control.WriteLine("                   ABL, A BASIC Language");
                control.WriteLine("         **** Altair Commodore BASIC Emulator ****");
                control.WriteLine("");
                control.WriteLine("Ready.");
                break;
            case Resolution.Pixels320x200Characters40x25:
                control.WriteLine("            A BASIC Language");
                control.WriteLine("    *** An Altair BASIC Emulator ***");
                control.WriteLine("");
                control.WriteLine("Ready.");
                break;
            case Resolution.Pixels160x200Characters20x25:
                control.WriteLine("  A BASIC Language");
                control.WriteLine("**Altair BASIC Emu**");
                control.WriteLine("");
                control.WriteLine("Ready.");
                break;
            default:
                throw new SystemException("Resolution not supported.");
        }
    }
}