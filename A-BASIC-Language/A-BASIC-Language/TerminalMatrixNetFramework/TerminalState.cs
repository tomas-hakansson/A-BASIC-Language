namespace TerminalMatrixNetFramework;

public class TerminalState
{
    public bool DirectMode { get; set; }
    public bool InputMode { get; set; }
    public int InputStartX { get; set; }
    public int InputStartY { get; set; }

    public TerminalState()
    {
        DirectMode = true;
        InputMode = false;
        InputStartX = 0;
    }
}
