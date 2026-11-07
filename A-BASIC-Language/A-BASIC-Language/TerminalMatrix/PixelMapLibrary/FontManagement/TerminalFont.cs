#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement;

public abstract class TerminalFont : IReadOnlyDictionary<char, CharacterPixelMatrix>
{
    protected abstract Dictionary<char, CharacterPixelMatrix> Data { get; set; }

    public IEnumerator<KeyValuePair<char, CharacterPixelMatrix>> GetEnumerator() =>
        Data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        Data.GetEnumerator();

    public int Count =>
        Data.Count;

    public bool ContainsKey(char key) =>
        Data.ContainsKey(key);

#pragma warning disable CS8601 // Possible null reference assignment.
    public bool TryGetValue(char key, out CharacterPixelMatrix value) =>
        Data.TryGetValue(key, out value);
#pragma warning restore CS8601 // Possible null reference assignment.

    public CharacterPixelMatrix this[int index] =>
        Data.ElementAt(index).Value;

    public CharacterPixelMatrix this[char key] =>
        Data[key];

    public IEnumerable<char> Keys =>
        Data.Keys;

    public IEnumerable<CharacterPixelMatrix> Values =>
        Data.Values;
}
