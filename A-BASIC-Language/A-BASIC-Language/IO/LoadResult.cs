#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

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
}
