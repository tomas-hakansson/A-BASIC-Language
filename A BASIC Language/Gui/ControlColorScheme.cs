using ConsoleControlLibrary;

namespace A_BASIC_Language.Gui;

public class ControlColorScheme : IControlColorScheme
{
    public Color BackColor => Color.FromArgb(30, 30, 120);
    public Color ForeColor => Color.FromArgb(40, 200, 200);
    public Color InputControlBackColor => Color.FromArgb(10, 10, 90);
    public Color ActiveControlForeColor => Color.FromArgb(40, 200, 200);
    public Color DisabledForeColor => Color.FromArgb(30, 150, 150);
}