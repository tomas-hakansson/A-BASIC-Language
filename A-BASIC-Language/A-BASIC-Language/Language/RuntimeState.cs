using System;
using System.Collections.Generic;
using A_BASIC_Language.Language.ValueTypes;

namespace A_BASIC_Language.Language;

/// <summary>Session memory persists when RUN or NEW clears variables and other program state.</summary>
public sealed class RuntimeState
{
    public Dictionary<string, ValueBase> Variables { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Dimension> Arrays { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, AblFunctionDefinition> Functions { get; } = new(StringComparer.OrdinalIgnoreCase);
    public int DataPointer { get; set; }
    internal byte[] Memory { get; } = new byte[ushort.MaxValue + 1];

    public void Clear() { Variables.Clear(); Arrays.Clear(); Functions.Clear(); DataPointer = 0; }
}
