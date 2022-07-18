using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language_Tests;

[TestClass]
public class Value_Tests
{
    [TestMethod]
    public void CanCreateValueType()
    {
        var f = ValueBase.GetValueType("5.5");
        Assert.IsTrue(f.GetType() == typeof(FloatValue));

        var i = ValueBase.GetValueType("6");
        Assert.IsTrue(i.GetType() == typeof(IntValue));

        var s = ValueBase.GetValueType("Fish");
        Assert.IsTrue(s.GetType() == typeof(StringValue));
    }

    [TestMethod]
    public void CanCastFromInt()
    {
        var i = new IntValue(40);

        Assert.IsTrue(i.CanGetAsType<IntValue>());
        Assert.IsTrue(i.GetValueAsType<IntValue>() is int);
        Assert.IsTrue((int)i.GetValueAsType<IntValue>() == 40);

        Assert.IsTrue(i.CanGetAsType<FloatValue>());
        Assert.IsTrue(i.GetValueAsType<FloatValue>() is double);
        Assert.IsTrue((double)i.GetValueAsType<FloatValue>() == 40.0);

        Assert.IsTrue(i.CanGetAsType<StringValue>());
        Assert.IsTrue(i.GetValueAsType<StringValue>() is string);
        Assert.IsTrue((string)i.GetValueAsType<StringValue>() == "40");
    }

    [TestMethod]
    public void CanCastFromFloat()
    {
        var f = new FloatValue(40.5);

        Assert.IsTrue(f.CanGetAsType<IntValue>());
        Assert.IsTrue(f.GetValueAsType<IntValue>() is int);
        Assert.IsTrue((int)f.GetValueAsType<IntValue>() == 40);

        Assert.IsTrue(f.CanGetAsType<FloatValue>());
        Assert.IsTrue(f.GetValueAsType<FloatValue>() is double);
        Assert.IsTrue((double)f.GetValueAsType<FloatValue>() == 40.5);

        Assert.IsTrue(f.CanGetAsType<StringValue>());
        Assert.IsTrue(f.GetValueAsType<StringValue>() is string);
        Assert.IsTrue((string)f.GetValueAsType<StringValue>() == "40.5");
    }
}