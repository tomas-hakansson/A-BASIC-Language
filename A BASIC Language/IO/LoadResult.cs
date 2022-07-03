namespace A_BASIC_Language.IO;

public class LoadResult
{
    public bool Result { get; }
    public string Data { get; }

    public LoadResult(bool result, string data)
    {
        Result = result;
        Data = data;
    }

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Data);

    public List<string> DataAsList() =>
        Data.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

    public static LoadResult Fail() =>
        new(false, "");

    public static LoadResult Success(string source) =>
        new(true, source);
}