using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language.Language;

/// <summary>Variables persist between direct commands; RUN and NEW start fresh.</summary>
public sealed class RuntimeState
{
    public Dictionary<string, ValueBase?> Variables { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Dimension> Arrays { get; } = new(StringComparer.OrdinalIgnoreCase);
    public void Clear() { Variables.Clear(); Arrays.Clear(); }
}
