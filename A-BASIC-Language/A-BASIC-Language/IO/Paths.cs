#nullable enable
using System.IO;
using System.Reflection;

namespace A_BASIC_Language.IO;

public static class Paths
{
    public static DirectoryInfo? GetProgramBasePath()
    {
        try
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly == null)
                return null;

            var location = assembly.Location;
            var file = new FileInfo(location);
            return file.Directory;
        }
        catch
        {
            return null;
        }
    }

    public static DirectoryInfo? GetGamesSourceDirectory()
    {
        var dir = GetProgramBasePath();

        if (dir == null)
            return null;

        try
        {
            return new DirectoryInfo(Path.Combine(dir.FullName, @"Games\Source\"));
        }
        catch
        {
            return null;
        }
    }
}
