using System.Configuration;

namespace A_BASIC_Language;

public class Configuration
{
    public static List<string> ConfigurationMessages { get; }
    public static int ColumnCount { get; private set; }
    public static Color PrimaryVectorGraphicsColor { get; private set; }

    static Configuration()
    {
        ConfigurationMessages = new List<string>();
        LoadConfiguration();
    }

    private static void LoadConfiguration()
    {
        ColumnCount = ParseInt("columnCount", 60);

        if (ColumnCount != 40 && ColumnCount != 60 && ColumnCount != 80)
        {
            ConfigurationMessages.Add(@"Setting ""columnCount"" should be 60, 70 or 80.");
            ColumnCount = 60;
        }

        PrimaryVectorGraphicsColor = ParseColor("primaryVectorGraphicsColor", "#a0a000");
    }

    private static int ParseInt(string settingName, int defaultValue)
    {
        try
        {
            var result = int.Parse(ConfigurationManager.AppSettings[settingName] ?? "");
            return result;
        }
        catch
        {
            ConfigurationMessages.Add($@"Failed to parse setting ""{settingName}"", defaults to {defaultValue}.");
            return defaultValue;
        }
    }

    private static Color ParseColor(string settingName, string defaultValue)
    {
        var color = ConfigurationManager.AppSettings[settingName];

        if (color == null)
        {
            ConfigurationMessages.Add($@"Setting ""{settingName}"" is missing.");
            color = defaultValue;
        }

        if (color.Length == 7 && color.StartsWith("#"))
            color = $"#ff{color[1..]}";

        try
        {
            var c = ColorTranslator.FromHtml(color);
            return c;
        }
        catch
        {
            ConfigurationMessages.Add($@"Failed to parse color setting ""{settingName}"".");
            return Color.FromArgb(160, 160, 0);
        }
    }
}