using System;
using A_BASIC_Language.TerminalMatrix;

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
            case Resolution.Pixels640X200Characters80X25:
                control.WriteLine("                             ABL, A BASIC Language");
                control.WriteLine("                  ***** Altair Commodore BASIC Emulator *****");
                control.WriteLine("                    (C)  Tomas Hakansson & Anders Hesselbom");
                break;
            case Resolution.Pixels480X200Characters60X25:
                control.WriteLine("                   ABL, A BASIC Language");
                control.WriteLine("         **** Altair Commodore BASIC Emulator ****");
                control.WriteLine("          (C)  Tomas Hakansson & Anders Hesselbom");
                break;
            case Resolution.Pixels320X200Characters40X25:
                control.WriteLine("            A BASIC Language");
                control.WriteLine("    *** An Altair BASIC Emulator ***");
                control.WriteLine(" (C) Tomas Hakansson & Anders Hesselbom");
                break;
            case Resolution.Pixels160X200Characters20X25:
                control.WriteLine("  A BASIC Language");
                control.WriteLine("**Altair BASIC Emu**");
                control.WriteLine("T.Hakansson & A.Hbom");
                break;
            default:
                throw new SystemException("Resolution not supported.");
        }

        control.WriteLine("");
        control.WriteLine("Ready.");
    }
}
