using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language.SpecificExecutors;

public class SubtractExecutor : ISubExecutor
{
    private readonly Stack<ValueBase> _data;

    public SubtractExecutor(Stack<ValueBase> data)
    {
        _data = data;
    }

    public void Run(int lineNumber)
    {
        if (_data.Count >= 2)
        {
            dynamic x = _data.Pop();
            dynamic y = _data.Pop();
            _data.Push(Subtract(lineNumber, y, x));
        }
        else
            throw new SystemException($"Line {lineNumber}: Insufficient items on the stack.");

    }

    // Float rules

    static ValueBase Subtract(int lineNumber, FloatValue x, FloatValue y) =>
        new FloatValue(x.Value - y.Value);

    static ValueBase Subtract(int lineNumber, FloatValue x, IntValue y) =>
        new FloatValue(x.Value - y.Value);

    static ValueBase Subtract(int lineNumber, FloatValue x, StringValue y) =>
        throw new SystemException($"Line {lineNumber}: Subtract in string scope.");

    // Int rules

    static ValueBase Subtract(int lineNumber, IntValue x, IntValue y) =>
        new FloatValue(x.Value - y.Value);

    static ValueBase Subtract(int lineNumber, IntValue x, FloatValue y) =>
        new FloatValue(x.Value - y.Value);

    static ValueBase Subtract(int lineNumber, IntValue x, StringValue y) =>
        throw new SystemException($"Line {lineNumber}: Subtract in string scope.");

    // String rules

    static ValueBase Subtract(int lineNumber, StringValue x, StringValue y) =>
        throw new SystemException($"Line {lineNumber}: Subtract in string scope.");

    static ValueBase Subtract(int lineNumber, StringValue x, FloatValue y) =>
        throw new SystemException($"Line {lineNumber}: Subtract in string scope.");

    static ValueBase Subtract(int lineNumber, StringValue x, IntValue y) =>
        throw new SystemException($"Line {lineNumber}: Subtract in string scope.");
}