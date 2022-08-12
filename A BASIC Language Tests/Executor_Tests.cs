using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_BASIC_Language.SpecificExecutors;
using A_BASIC_Language.ValueTypes;
using System.Collections.Generic;

namespace A_BASIC_Language_Tests;

[TestClass]
public class Executor_Tests
{
    [TestMethod]
    public void Add()
    {
        Stack<ValueBase> values = new();
        AddExecutor executor = new(values);

        values.Push(new FloatValue(1));
        values.Push(new FloatValue(2));
        executor.Run();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);

        values.Push(new FloatValue(1));
        values.Push(new IntValue(2));
        executor.Run();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);

        values.Push(new FloatValue(1));
        values.Push(new StringValue("2"));
        executor.Run();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, "21");

        values.Push(new IntValue(1));
        values.Push(new IntValue(2));
        executor.Run();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);

        values.Push(new IntValue(1));
        values.Push(new FloatValue(2));
        executor.Run();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);

        values.Push(new IntValue(1));
        values.Push(new StringValue("2"));
        executor.Run();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, "21");

        values.Push(new StringValue("1"));
        values.Push(new StringValue("2"));
        executor.Run();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, "21");

        values.Push(new StringValue("1"));
        values.Push(new FloatValue(2));
        executor.Run();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);

        values.Push(new StringValue("1"));
        values.Push(new IntValue(2));
        executor.Run();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);
    }

    [TestMethod]
    public void Add2()
    {
        Stack<ValueBase> values = new();
        AddExecutor executor = new(values);

        values.Push(new FloatValue(1));
        values.Push(new FloatValue(2));
        executor.Run2();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);

        values.Push(new FloatValue(1));
        values.Push(new IntValue(2));
        executor.Run2();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);

        values.Push(new FloatValue(1));
        values.Push(new StringValue("2"));
        executor.Run2();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, "21");

        values.Push(new IntValue(1));
        values.Push(new IntValue(2));
        executor.Run2();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);

        values.Push(new IntValue(1));
        values.Push(new FloatValue(2));
        executor.Run2();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, 3);

        values.Push(new IntValue(1));
        values.Push(new StringValue("2"));
        executor.Run2();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, "21");

        values.Push(new StringValue("1"));
        values.Push(new StringValue("2"));
        executor.Run2();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, "21");

        values.Push(new StringValue("1"));
        values.Push(new FloatValue(2));
        executor.Run2();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, "21");

        values.Push(new StringValue("1"));
        values.Push(new IntValue(2));
        executor.Run2();
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(((dynamic)values.Pop()).Value, "21");
    }
}
