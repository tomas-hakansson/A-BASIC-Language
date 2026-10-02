#nullable enable
using System.Collections.Generic;

namespace A_BASIC_Language;

public class ProgramCacheDictionary : Dictionary<string, string>
{
    public BasicProgram GetValueOrDefault(string name)
    {
        if (TryGetValue(name, out var sourceCode))
        {
            return new BasicProgram
            {
                SourceCode = sourceCode,
                Filename = name
            };
        }

        return BasicProgram.Empty();
    }
}
