using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language.SpecificExecutors;

public abstract class VariableExecutor
{
    protected readonly Stack<ValueBase> Data;
    protected readonly Action<string> End;

    protected VariableExecutor(Stack<ValueBase> data, Action<string> end)
    {
        Data = data;
        End = end;
    }
}