using System;
using System.Collections.Generic;
using A_BASIC_Language.Language.ValueTypes;
using A_BASIC_Language.SpecificExecutors;

namespace A_BASIC_Language.Language.SpecificExecutors;

public class FlatVariableExecutor : VariableExecutor
{
    private readonly Dictionary<string, ValueBase> _variables;

    public FlatVariableExecutor(Stack<ValueBase> data, Action<string> end, Dictionary<string, ValueBase> variables) : base(data, end)
    {
        _variables = variables;
    }

    public void Create(ABL_Variable v)
    {
        if (_variables.TryGetValue(v.Symbol, out var value))
        {
            value ??= ValueBase.GetDefaultValueFor(v.Symbol);
            Data.Push(value);
        }
        else
        {
            // Variable is probably not declared, get the default value for the variable.
            Data.Push(ValueBase.GetDefaultValueFor(v.Symbol));
        }
    }

    public void Write(ABL_Assignment a)
    {
        if (Data.Count > 0)
        {
            var value = Data.Pop();
            var symbol = a.Symbol;

            value = value.ForVariable(symbol);

            _variables[symbol] = value;
        }
        else
        {
            End("The stack is empty");
        }
    }
}
