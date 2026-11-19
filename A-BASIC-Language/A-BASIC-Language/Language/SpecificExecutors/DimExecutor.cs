using System;
using System.Collections.Generic;
using System.Linq;
using A_BASIC_Language.Language.ValueTypes;

namespace A_BASIC_Language.Language.SpecificExecutors;

public class DimExecutor : VariableExecutor
{
    private readonly Dictionary<string, Dimension> _dimVariables;

    public DimExecutor(Stack<ValueBase> data, Action<string> end, Dictionary<string, Dimension> dimVariables) : base(data, end)
    {
        _dimVariables = dimVariables;
    }

    public void Create(AblDimCreation dc)
    {
        if (Data.Count > 1)
        {
            var indexCount = (int)Data.Pop().GetValueAsType<IntValue>();
            List<int> index = [];

            for (var inner = 0; inner < indexCount; inner++)
            {
                var indexDigit = (int)Data.Pop().GetValueAsType<IntValue>();
                index.Add(indexDigit);
            }

            var defaultValue = ValueBase.GetDefaultValueFor(dc.Symbol);
            Dimension newDim = new(index, defaultValue);
            _dimVariables[dc.Symbol] = newDim;
        }
        else
        {
            End("Insufficient items on the stack");
        }
    }

    public void Read(AblDimVariable dv)
    {
        if (Data.Count > 1)
        {
            //Note: Removal of index data from the stack must be done in all successful cases.
            var indexCount = (int)Data.Pop().GetValueAsType<IntValue>();
            List<int> index = [];
            
            for (var inner = 0; inner < indexCount; inner++)
            {
                var indexDigit = (int)Data.Pop().GetValueAsType<IntValue>();
                index.Add(indexDigit);
            }

            Data.Push(GetOrCreateImplicitArray(dv.Symbol, indexCount).Get(index));
        }
        else
        {
            End("Insufficient items on the stack");
        }
    }

    public void Write(AblDimAssignment da)
    {
        if (Data.Count > 2)
        {
            var value = Data.Pop();
            var symbol = da.Symbol;

            value = value.ForVariable(symbol);

            var indexCount = (int)Data.Pop().GetValueAsType<IntValue>();
            List<int> index = [];

            for (var inner = 0; inner < indexCount; inner++)
            {
                var indexDigit = (int)Data.Pop().GetValueAsType<IntValue>();
                index.Add(indexDigit);
            }

            var dim = GetOrCreateImplicitArray(symbol, indexCount);
            dim.Add(value, index);
        }
        else
        {
            End("Insufficient items on the stack");
        }
    }

    private Dimension GetOrCreateImplicitArray(string symbol, int indexCount)
    {
        if (!_dimVariables.TryGetValue(symbol, out var dim))
        {
            // Arrays used without DIM have fixed bounds 0..10 in each dimension.
            dim = new Dimension(Enumerable.Repeat(10, indexCount), ValueBase.GetDefaultValueFor(symbol));
            _dimVariables[symbol] = dim;
        }

        return dim;
    }
}
