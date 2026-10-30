using System.Collections.Generic;
using A_BASIC_Language.TerminalMatrix.TerminalColor;

namespace A_BASIC_Language.Gui.Multimedia;

public enum ColorScheme
{
    Default,
    C64,
    C128,
    Vic20,
    C16,
}

public struct ColorSchemeConfiguration
{
    public string Name { get; }
    public ColorScheme Id { get; }
    public ColorName Background { get; }
    public ColorName Text { get; }
    public ColorName Border { get; }

    public ColorSchemeConfiguration(string name, ColorScheme id, ColorName background, ColorName text, ColorName border)
    {
        Name = name;
        Id = id;
        Background = background;
        Text = text;
        Border = border;
    }

    public override string ToString() =>
        Name;
}

public class ColorSchemeConfigurationList : List<ColorSchemeConfiguration>
{
    public ColorSchemeConfigurationList()
    {
        Add(new ColorSchemeConfiguration("DEFAULT", ColorScheme.Default, ColorName.Black, ColorName.Green, ColorName.Black));
        Add(new ColorSchemeConfiguration("COMMODORE 64", ColorScheme.C64, ColorName.Blue, ColorName.LightBlue, ColorName.LightBlue));
        Add(new ColorSchemeConfiguration("COMMODORE 128", ColorScheme.C128, ColorName.DarkGrey, ColorName.LightGreen, ColorName.LightGreen));
        Add(new ColorSchemeConfiguration("VIC-20", ColorScheme.Vic20, ColorName.White, ColorName.Blue, ColorName.Cyan));
        Add(new ColorSchemeConfiguration("COMMODORE 16", ColorScheme.C16, ColorName.Violet, ColorName.Black, ColorName.White));
    }

    public ColorSchemeConfiguration Get(ColorScheme scheme) =>
        this[(int)scheme];
}
