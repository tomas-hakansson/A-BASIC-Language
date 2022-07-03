namespace A_BASIC_Language.IO;

public class NetIO : IOBase
{
    public NetIO(string filename) : base(filename)
    {
    }

    public override LoadResult Load()
    {
        return new LoadResult(false, "");
    }
}