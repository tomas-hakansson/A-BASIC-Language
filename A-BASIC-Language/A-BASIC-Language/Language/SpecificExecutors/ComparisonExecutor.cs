using A_BASIC_Language.ValueTypes;
using System.Diagnostics;

namespace A_BASIC_Language.SpecificExecutors;

public class ComparisonExecutor
{
    private readonly Stack<ValueBase> _data;

    public ComparisonExecutor(Stack<ValueBase> data)
    {
        _data = data;
    }

    public void Run(int lineNumber, Func<double, double, bool> f)
    {
        if (_data.Count >= 2)
        {
            var x = _data.Pop();
            var y = _data.Pop();

            if (x.IsOfType<IntValue>() && y.IsOfType<IntValue>())
            {
                var i2 = (int)x.GetValueAsType<IntValue>();
                var i1 = (int)y.GetValueAsType<IntValue>();
                _data.Push(new FloatValue(f(i1, i2) ? -1 : 0));
            }
            else // TODO: Check that they can be converted to float
            {
                var f2 = (double)x.GetValueAsType<FloatValue>();
                var f1 = (double)y.GetValueAsType<FloatValue>();
                _data.Push(new FloatValue(f(f1, f2) ? -1 : 0));
            }
        }
        else
            Debug.Fail($"Insufficient items on the stack on line {lineNumber}.");
    }
}