using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_BASIC_Language.Language;

internal static class Petscii
{
    private const int FirstCode = 32;
    private const string Characters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    internal const byte SwedishFirstCode = 128;
    internal const string SwedishCharacters = "ÅÄÖåäö";
    private const char PlaceholderStart = '\uE000';
    private static readonly ConcurrentDictionary<byte, char> Placeholders = new();

    public static int Encode(char character)
    {
        if (TryGetSwedishCode(character, out var swedishCode))
            return swedishCode;

        if (character >= 'a' && character <= 'z')
            character = char.ToUpperInvariant(character);

        var index = Characters.IndexOf(character);

        if (index >= 0)
            return FirstCode + index;

        if (TryGetPlaceholderCode(character, out var code))
            return code;

        throw new InvalidOperationException("Illegal function call: ASC does not support this character.");
    }

    internal static bool TryGetSwedishCode(char character, out byte code)
    {
        var index = SwedishCharacters.IndexOf(character);
        code = index >= 0 ? (byte)(SwedishFirstCode + index) : (byte)0;
        return index >= 0;
    }

    internal static bool TryGetPlaceholderCode(char character, out byte code)
    {
        var offset = character - PlaceholderStart;

        if (offset.IsByte() && Placeholders.ContainsKey((byte)offset))
        {
            code = (byte)offset;
            return true;
        }

        code = 0;
        return false;
    }

    public static char Decode(byte code)
    {
        var swedishIndex = code - SwedishFirstCode;
        if (swedishIndex >= 0 && swedishIndex < SwedishCharacters.Length)
            return SwedishCharacters[swedishIndex];

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
    public void SwedishCharactersHaveDistinctExtensionCodes()
    {
        var codePage = new TerminalMatrix.PixelMapLibrary.FontManagement.TerminalCodePage();
        for (var index = 0; index < Petscii.SwedishCharacters.Length; index++)
        {
            var character = Petscii.SwedishCharacters[index];
            var code = (byte)(128 + index);
            Assert.AreEqual((int)code, Petscii.Encode(character));
            Assert.AreEqual(character, Petscii.Decode(code));
            Assert.AreEqual(code, codePage.Asc[character]);
            Assert.AreEqual(character, codePage.Chr[code]);
        }
        Assert.AreEqual('\\', codePage.Chr[92]);
    }

    [TestMethod]
    public void UsesExpectedCodesForMappedCharacters()
    {
        Assert.AreEqual(32, Petscii.Encode(' '));
        Assert.AreEqual(65, Petscii.Encode('A'));
        Assert.AreEqual('Z', Petscii.Decode(90));
    }

    [TestMethod]
    public void LowercaseLettersUseTheSameCodesAsUppercaseLetters()
    {
        for (var offset = 0; offset < 26; offset++)
            Assert.AreEqual(65 + offset, Petscii.Encode((char)('a' + offset)));
    }

    [TestMethod]
    public void AllByteValuesRoundTripThroughCharacters()
    {
        for (var code = 0; code <= byte.MaxValue; code++)
            Assert.AreEqual(code, Petscii.Encode(Petscii.Decode((byte)code)), $"PETSCII code {code}");
    }
}
#endif
