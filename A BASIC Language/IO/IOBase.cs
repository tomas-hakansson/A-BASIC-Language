namespace A_BASIC_Language.IO;

public abstract class IOBase
{
    protected string Filename { get; }

    protected IOBase(string filename)
    {
        Filename = filename;
    }

    public abstract LoadResult Load();
}