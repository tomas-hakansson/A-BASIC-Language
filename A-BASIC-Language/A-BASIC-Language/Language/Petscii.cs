using System;
using System.Collections.Concurrent;

namespace A_BASIC_Language.Language;

internal static class Petscii
{
    private const int FirstCode = 32;
    private const string Characters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const char PlaceholderStart = '\uE000';
    private static readonly ConcurrentDictionary<byte, char> Placeholders = new();

    public static int Encode(char character)
    {
        var index = Characters.IndexOf(character);
        if (index >= 0)
            return FirstCode + index;

        var code = character - PlaceholderStart;
        if (code >= 0 && code <= byte.MaxValue && Placeholders.ContainsKey((byte)code))
            return code;

        throw new InvalidOperationException("Illegal function call: ASC does not support this character.");
    }

    public static char Decode(byte code)
    {
        var index = code - FirstCode;
        if (index >= 0 && index < Characters.Length)
            return Characters[index];

        return Placeholders.GetOrAdd(code, value => (char)(PlaceholderStart + value));
    }
}
