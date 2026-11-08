using System;

namespace A_BASIC_Language.Language;

internal static class Petscii
{
    private const int FirstCode = 32;
    private const string Characters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static int Encode(char character)
    {
        var index = Characters.IndexOf(character);
        if (index < 0)
            throw new InvalidOperationException("Illegal function call: ASC does not support this character.");

        return FirstCode + index;
    }

    public static char Decode(int code)
    {
        var index = code - FirstCode;
        if (index < 0 || index >= Characters.Length)
            throw new InvalidOperationException($"Illegal function call: CHR$ does not support PETSCII code {code}.");

        return Characters[index];
    }
}
