using System;
using System.Collections.Generic;
using A_BASIC_Language.Language.ValueTypes;

namespace A_BASIC_Language.Language.SpecificExecutors;

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
            dynamic right = _data.Pop();
            dynamic left = _data.Pop();
            ValueBase result = Subtract(left, right);
            _data.Push(result);
        }
        else
            throw new InvalidOperationException("Insufficient items on the stack.");

    }

    // Float rules

    static ValueBase Subtract(FloatValue x, FloatValue y) =>
        new FloatValue(x.Value - y.Value);

    static ValueBase Subtract(FloatValue x, IntValue y) =>
        new FloatValue(x.Value - y.Value);

    static ValueBase Subtract(FloatValue x, StringValue y) =>
        throw new InvalidOperationException("Subtract in string scope.");

    // Int rules

    static ValueBase Subtract(IntValue x, IntValue y) =>
        new FloatValue(x.Value - y.Value);

    static ValueBase Subtract(IntValue x, FloatValue y) =>
        new FloatValue(x.Value - y.Value);

    static ValueBase Subtract(IntValue x, StringValue y) =>
        throw new InvalidOperationException("Subtract in string scope.");

    // String rules

    static ValueBase Subtract(StringValue x, StringValue y) =>
        throw new InvalidOperationException("Subtract in string scope.");

    static ValueBase Subtract(StringValue x, FloatValue y) =>
        throw new InvalidOperationException("Subtract in string scope.");

    static ValueBase Subtract(StringValue x, IntValue y) =>
        throw new InvalidOperationException("Subtract in string scope.");
}
