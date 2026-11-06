#nullable enable
using System;
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

    public static DirectoryInfo GetUserProgramsRootPath()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var programsRoot = Path.Combine(folder, "AblPrograms");

        if (!Directory.Exists(programsRoot))
            Directory.CreateDirectory(programsRoot);

        return new DirectoryInfo(programsRoot);
    }

    public static DirectoryInfo? GetUser101GamesPath()
    {
        DirectoryInfo? result;

        try
        {
            var path = Path.Combine(GetUserProgramsRootPath().FullName, "101Games");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            result = new DirectoryInfo(path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

        return result;
    }

    public static DirectoryInfo? GetUserProgramsPath()
    {
        try
        {
            var path = Path.Combine(GetUserProgramsRootPath().FullName, "UserPrograms");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return new DirectoryInfo(path);
        }
        catch
        {
            return null;
        }
    }

    public static DirectoryInfo GetProgramDataPath(FileInfo programFile)
    {
        var directory = programFile.Directory ?? GetUserProgramsPath();
        var name = programFile.Name.ToLower();

        if (name.EndsWith(".bas"))
            name = name.Substring(0, name.Length - 4);

        var path = Path.Combine(directory.FullName, name);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return new DirectoryInfo(path);
    }

    public static DirectoryInfo? GetGamesSourceDirectory()
    {
        var dir = GetProgramBasePath();

        if (dir == null)
            return null;

        try
        {
            return new DirectoryInfo(Path.Combine(dir.FullName, @"Games"));
        }
        catch
        {
            return null;
        }
    }
}
