using System;
using System.Collections.Concurrent;
#if DEBUG
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

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

        if (TryGetPlaceholderCode(character, out var code))
            return code;

        throw new InvalidOperationException("Illegal function call: ASC does not support this character.");
    }

    internal static bool TryGetPlaceholderCode(char character, out byte code)
    {
        var offset = character - PlaceholderStart;
        if (offset >= 0 && offset <= byte.MaxValue && Placeholders.ContainsKey((byte)offset))
        {
            code = (byte)offset;
            return true;
        }

        code = 0;
        return false;
    }

    public static char Decode(byte code)
    {
        var index = code - FirstCode;
        if (index >= 0 && index < Characters.Length)
            return Characters[index];

        return Placeholders.GetOrAdd(code, value => (char)(PlaceholderStart + value));
    }
}

#if DEBUG
[TestClass]
public class PetsciiTests
{
    [TestMethod]
    public void UsesExpectedCodesForMappedCharacters()
    {
        Assert.AreEqual(32, Petscii.Encode(' '));
        Assert.AreEqual(65, Petscii.Encode('A'));
        Assert.AreEqual('Z', Petscii.Decode(90));
    }

    [TestMethod]
    public void AllByteValuesRoundTripThroughCharacters()
    {
        for (var code = 0; code <= byte.MaxValue; code++)
            Assert.AreEqual(code, Petscii.Encode(Petscii.Decode((byte)code)), $"PETSCII code {code}");
    }
}
#endif
