using System.Configuration;

namespace A_BASIC_Language;

public class Configuration
{
    public static List<string> ConfigurationMessages { get; }
    public static int ColumnCount { get; private set; }

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
}