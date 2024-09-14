namespace A_BASIC_Language.IO;

public class Dispatcher
{
    public IoBase GetIo(string filename) =>
        filename.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || filename.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)
            ? new NetIo(filename)
            : new FileIo(filename);
}