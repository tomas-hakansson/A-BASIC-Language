using System.Net;

namespace A_BASIC_Language.IO;

public class NetIO : IOBase
{
    public NetIO(string filename) : base(filename)
    {
    }

    public override LoadResult Load()
    {
        try
        {
            using var wc = new WebClient(); // todo
            var source = wc.DownloadString(Filename);
            return LoadResult.Success(source);
        }
        catch
        {
            return LoadResult.Fail();
        }
    }
}