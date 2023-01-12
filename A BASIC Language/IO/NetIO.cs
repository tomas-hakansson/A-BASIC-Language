namespace A_BASIC_Language.IO;

public class NetIo : IoBase
{
    public NetIo(string filename) : base(filename)
    {
    }

    public override string GetNameOnly() =>
        Filename.Contains('/')
            ? Filename.Split("/").LastOrDefault() ?? ""
            : Filename;

    public override async Task<LoadResult> Load()
    {
        try
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(Filename);

            return string.IsNullOrWhiteSpace(response)
                ? LoadResult.Fail() 
                : LoadResult.Success(response ?? "");
        }
        catch
        {
            return LoadResult.Fail();
        }
    }
}