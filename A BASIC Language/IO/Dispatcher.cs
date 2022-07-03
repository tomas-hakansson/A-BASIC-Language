namespace A_BASIC_Language.IO;

public class Dispatcher
{
    public IOBase GetIo(string filename) =>
        filename.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || filename.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)
            ? new NetIO(filename)
            : new FileIO(filename);
}