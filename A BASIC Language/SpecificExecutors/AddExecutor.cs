using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language.SpecificExecutors;

public class AddExecutor : ISubExecutor
{
    private readonly Stack<ValueBase> _data;

    public AddExecutor(Stack<ValueBase> data)
    {
        _data = data;
    }

    public void Run()
    {
        if (_data.Count >= 2)
        {
            dynamic x = _data.Pop();
            dynamic y = _data.Pop();
            _data.Push(Add(x, y));
        }
        else
            throw new SystemException("Insufficient items on the stack.");

    }

    // Float rules

    static ValueBase Add(FloatValue x, FloatValue y) =>
        new FloatValue(x.Value + y.Value);

    static ValueBase Add(FloatValue x, IntValue y) =>
        new FloatValue(x.Value + y.Value);

    static ValueBase Add(FloatValue x, StringValue y) =>
        new StringValue(x.Value + y.Value);

    // Int rules

    static ValueBase Add(IntValue x, IntValue y) =>
        new FloatValue(x.Value + y.Value);

    static ValueBase Add(IntValue x, FloatValue y) =>
        new FloatValue(x.Value + y.Value);

    static ValueBase Add(IntValue x, StringValue y) =>
        new StringValue(x.Value + y.Value);

    // String rules

    static ValueBase Add(StringValue x, StringValue y) =>
        new StringValue(x.Value + y.Value);

    static ValueBase Add(StringValue x, FloatValue y) =>
        new StringValue(x.Value + y.Value);

    static ValueBase Add(StringValue x, IntValue y) =>
        new StringValue(x.Value + y.Value);
}