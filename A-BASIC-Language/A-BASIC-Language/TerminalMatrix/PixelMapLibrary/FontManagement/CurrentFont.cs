using System.Collections.Generic;

namespace A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement;

public enum CurrentFont
{
    Pet,
    C64
}

public readonly struct FontConfiguration
{
    public string Name { get; }
    public string ShortName { get; }
    public CurrentFont Id { get; }
    
    public FontConfiguration(string name, string shortName, CurrentFont id)
    {
        Name = name;
        ShortName = shortName;
        Id = id;
    }

    public override string ToString() =>
        Name;
}

public class FontConfigurationList : List<FontConfiguration>
{
    public FontConfigurationList()
    {
        Add(new FontConfiguration("Commodore PET", "PET", CurrentFont.Pet));
        Add(new FontConfiguration("Commodore 64", "C64", CurrentFont.C64));
    }

    public FontConfiguration Get(FontConfiguration font) =>
        this[(int)font.Id];

    public FontConfiguration Get(CurrentFont font) =>
        this[(int)font];
}
