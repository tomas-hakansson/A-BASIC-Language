using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language_Tests;

[TestClass]
public class Value_Tests
{
    [TestMethod]
    public void CanCreateValueTypes()
    {
        Assert.IsTrue(ValueBase.GetValueType("Hej") is StringValue);
        Assert.IsTrue(ValueBase.GetValueType("5") is IntValue);
        Assert.IsTrue(ValueBase.GetValueType("5.0") is IntValue);
        Assert.IsTrue(ValueBase.GetValueType("5.1") is FloatValue);
        Assert.IsTrue(ValueBase.GetValueType(5) is IntValue);
        Assert.IsTrue(ValueBase.GetValueType(5.0) is IntValue);
        Assert.IsTrue(ValueBase.GetValueType(5.1) is FloatValue);
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

    [TestMethod]
    public void CanCastFromString()
    {
        var s = new StringValue("40.5");

        Assert.IsTrue(s.CanGetAsType<IntValue>());
        Assert.IsTrue(s.GetValueAsType<IntValue>() is int);
        Assert.IsTrue((int)s.GetValueAsType<IntValue>() == 40);

        Assert.IsTrue(s.CanGetAsType<FloatValue>());
        Assert.IsTrue(s.GetValueAsType<FloatValue>() is double);
        Assert.IsTrue((double)s.GetValueAsType<FloatValue>() == 40.5);

        Assert.IsTrue(s.CanGetAsType<StringValue>());
        Assert.IsTrue(s.GetValueAsType<StringValue>() is string);
        Assert.IsTrue((string)s.GetValueAsType<StringValue>() == "40.5");
    }

    [TestMethod]
    public void CanNotCastFromString()
    {
        var s = new StringValue("Hello");

        Assert.IsFalse(s.CanGetAsType<IntValue>());

        Assert.IsFalse(s.CanGetAsType<FloatValue>());

        Assert.IsTrue(s.CanGetAsType<StringValue>());
        Assert.IsTrue(s.GetValueAsType<StringValue>() is string);
        Assert.IsTrue((string)s.GetValueAsType<StringValue>() == "Hello");
    }
}