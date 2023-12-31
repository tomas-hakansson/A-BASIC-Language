using System.Text;

namespace A_BASIC_Language.IO;

public class FileIo : IoBase
{
    public FileIo(string filename) : base(filename)
    {
    }

    public override string GetNameOnly()
    {
        string nameOnly;

        try
        {
            var fi = new FileInfo(Filename);
            nameOnly = fi.Name;
        }
        catch
        {
            nameOnly = Filename;
        }

        return nameOnly;
    }

    public override LoadResult Load()
    {
        try
        {
            using var x = new StreamReader(Filename, Encoding.UTF8);
            var source = x.ReadToEnd();
            x.Close();
            return LoadResult.Success(source);
        }
        catch
        {
            return LoadResult.Fail();
        }
    }
}