using System;
using System.Collections.Generic;
using A_BASIC_Language.Language.ValueTypes;

namespace A_BASIC_Language.Language;

/// <summary>Variables persist between direct commands; RUN and NEW start fresh.</summary>
public sealed class RuntimeState
{
    public Dictionary<string, ValueBase> Variables { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Dimension> Arrays { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, AblFunctionDefinition> Functions { get; } = new(StringComparer.OrdinalIgnoreCase);
    public void Clear() { Variables.Clear(); Arrays.Clear(); Functions.Clear(); }
}
