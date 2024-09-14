namespace A_BASIC_Language.IO;

public class ProgramRepository
{
    private static Dictionary<string, string> ProgramCache { get; }

    static ProgramRepository()
    {
        ProgramCache = new Dictionary<string, string>();
    }

    public BasicProgram GetProgram(IWin32Window owner, string pathAndName)
    {
        var ioDispatcher = new Dispatcher();
        var io = ioDispatcher.GetIo(pathAndName);
        var name = io.GetNameOnly();

        if (ProgramCache.ContainsKey(name))
            return new BasicProgram(ProgramCache.GetValueOrDefault(name) ?? "", name);

        var source = io.Load();

        if (!source.Result)
        {
            MessageBox.Show(owner, @"Failed to load source code.", @"Run failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return BasicProgram.Empty();
        }

        if (source.IsEmpty)
        {
            MessageBox.Show(@"Empty file loaded.", @"Run failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return BasicProgram.Empty();
        }

        return new BasicProgram(source.Data, name);
    }
}