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

    public override LoadResult Load()
    {
        try
        {
            var httpClient = new HttpClient();
            var token = httpClient.GetStringAsync(Filename);
            while (!token.IsCompleted)
            {
                Thread.Sleep(500);
                Application.DoEvents();
            }

            if (token.IsCompletedSuccessfully)
            {
                var response = token.Result;
                
                return string.IsNullOrWhiteSpace(response)
                    ? LoadResult.Fail()
                    : LoadResult.Success(response ?? "");
            }
            else
            {
                return LoadResult.Fail();
            }
        }
        catch
        {
            return LoadResult.Fail();
        }
    }
}