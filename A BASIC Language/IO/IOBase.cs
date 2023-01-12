namespace A_BASIC_Language.IO;

public abstract class IoBase
{
    protected string Filename { get; }

    protected IoBase(string filename)
    {
        Filename = filename;
    }

    public abstract string GetNameOnly();

    public abstract Task<LoadResult> Load();
}