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
            var x = _data.Pop();
            var y = _data.Pop();

            if (x.IsOfType<FloatValue>())
            {
                if (y.CanGetAsType<FloatValue>())
                {
                    var left = (double)x.GetValueAsType<FloatValue>();
                    var right = (double)y.GetValueAsType<FloatValue>();
                    _data.Push(new FloatValue(left + right));
                }
                else if (y.CanGetAsType<IntValue>())
                {
                    var left = (double)x.GetValueAsType<FloatValue>();
                    var right = (int)y.GetValueAsType<IntValue>();
                    _data.Push(new FloatValue(left + right));
                }
                else if (y.CanGetAsType<StringValue>() && x.CanGetAsType<StringValue>())
                {
                    var left = (string)x.GetValueAsType<StringValue>();
                    var right = (string)y.GetValueAsType<StringValue>();
                    _data.Push(new StringValue(left + right));
                }
            }
            else if (x.IsOfType<IntValue>())
            {
                if (y.CanGetAsType<FloatValue>())
                {
                    var left = (int)x.GetValueAsType<IntValue>();
                    var right = (double)y.GetValueAsType<FloatValue>();
                    _data.Push(new FloatValue(left + right));
                }
                else if (y.CanGetAsType<IntValue>())
                {
                    var left = (int)x.GetValueAsType<IntValue>();
                    var right = (int)y.GetValueAsType<IntValue>();
                    _data.Push(new FloatValue(left + right));
                }
                else if (y.CanGetAsType<StringValue>() && x.CanGetAsType<StringValue>())
                {
                    var left = (string)x.GetValueAsType<StringValue>();
                    var right = (string)y.GetValueAsType<StringValue>();
                    _data.Push(new StringValue(left + right));
                }
            }
            else if (x.IsOfType<StringValue>() && y.CanGetAsType<StringValue>())
            {
                var left = (string)x.GetValueAsType<StringValue>();
                var right = (string)y.GetValueAsType<StringValue>();
                _data.Push(new StringValue(left + right));
            }
            else
            {
                throw new SystemException("What?");
            }
        }
        else
            throw new SystemException("Insufficient items on the stack.");

    }
}