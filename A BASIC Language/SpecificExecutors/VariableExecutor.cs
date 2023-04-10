using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language.SpecificExecutors;

public abstract class VariableExecutor
{
    protected readonly Stack<ValueBase> Data;
    protected readonly Func<string, Task> End;

    protected VariableExecutor(Stack<ValueBase> data, Func<string, Task> end)
    {
        Data = data;
        End = end;
    }
}