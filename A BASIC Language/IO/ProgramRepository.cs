namespace A_BASIC_Language.IO;

public class ProgramRepository
{
    private static Dictionary<string, string> ProgramCache { get; }

    static ProgramRepository()
    {
        ProgramCache = new Dictionary<string, string>();
    }

    public string GetProgram(IWin32Window owner, string pathAndName, out string name)
    {
        var ioDispatcher = new Dispatcher();
        var io = ioDispatcher.GetIo(pathAndName);
        name = io.GetNameOnly();

        if (ProgramCache.ContainsKey(name))
            return ProgramCache.GetValueOrDefault(name) ?? "";

        var source = io.Load();

        if (!source.Result)
        {
            MessageBox.Show(owner, @"Failed to load source code.", @"Run failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }

        if (source.IsEmpty)
        {
            MessageBox.Show(@"Empty file loaded.", @"Run failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }

        return source.Data;
    }
}