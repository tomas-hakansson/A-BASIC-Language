using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_BASIC_Language.ValueTypes2;

namespace A_BASIC_Language_Tests;

[TestClass]
public class ValueTests2
{
    private const double FloatValueCompareErrorTolerance = 0.00001;

    [TestMethod]
    public void CanCreateValueTypes()
    {
        Assert.IsTrue(ValueBase.GetValueType("Hej") is StringValue);
        Assert.IsTrue(ValueBase.GetValueType("5") is IntValue); //FixMe: This casting is probably not correct Basic.
        Assert.IsTrue(ValueBase.GetValueType("5.0") is IntValue);//FixMe: This casting is probably not correct Basic.
        Assert.IsTrue(ValueBase.GetValueType("5.1") is FloatValue);//FixMe: This casting is probably not correct Basic.
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
        Assert.IsTrue(Math.Abs((double)i.GetValueAsType<FloatValue>() - 40.0) < FloatValueCompareErrorTolerance);

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
        Assert.IsTrue(Math.Abs((double)f.GetValueAsType<FloatValue>() - 40.5) < FloatValueCompareErrorTolerance);

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
        Assert.IsTrue(Math.Abs((double)s.GetValueAsType<FloatValue>() - 40.5) < FloatValueCompareErrorTolerance);

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

    [TestMethod]
    public void CanGetDefaultValue()
    {
        var stringThing = ValueBase.GetDefaultValueFor("A$");
        Assert.IsTrue(stringThing is StringValue);
        Assert.IsTrue((stringThing as StringValue)!.Value == "");

        var intThing = ValueBase.GetDefaultValueFor("A%");
        Assert.IsTrue(intThing is IntValue);
        Assert.IsTrue((intThing as IntValue)!.Value == 0);

        var floatThing = ValueBase.GetDefaultValueFor("A");
        Assert.IsTrue(floatThing is FloatValue);
        Assert.IsTrue((floatThing as FloatValue)!.Value == 0.0);
    }

    [TestMethod]
    public void CanCompare()
    {
        var fltGreater = new FloatValue(50.1);
        var fltLesser = new FloatValue(50.0);

        var intGreater = new IntValue(50);
        var intLesser = new IntValue(49);

        var strFiftyOne = new StringValue("51");
        var strFifty = new StringValue("50");

        Assert.IsTrue(null == null);
        Assert.IsFalse(null == fltGreater);

        Assert.IsTrue(fltLesser == intGreater);
        Assert.IsTrue(intGreater == fltLesser);
        Assert.IsFalse(intGreater == fltGreater);
        Assert.IsFalse(intLesser == intGreater);
        Assert.IsFalse(strFifty == strFiftyOne);

        Assert.IsTrue(fltGreater > intGreater);
        Assert.IsFalse(intLesser > fltLesser);
        Assert.IsFalse(intLesser > intGreater);

        Assert.IsTrue(fltGreater >= intGreater);
        Assert.IsFalse(intLesser >= fltLesser);
        Assert.IsFalse(intLesser >= intGreater);

        //Note: String casting:
        Assert.IsTrue(strFifty == fltLesser);
        Assert.IsTrue(strFifty == intGreater);
        Assert.IsTrue(fltLesser == strFifty);
        Assert.IsTrue(intGreater == strFifty);

        Assert.IsTrue(strFiftyOne != fltLesser);
        Assert.IsTrue(strFiftyOne != intGreater);
        Assert.IsTrue(fltGreater != strFifty);
        Assert.IsTrue(intLesser != strFifty);

        Assert.IsTrue(strFiftyOne > fltLesser);
        Assert.IsTrue(strFiftyOne > intGreater);
        Assert.IsTrue(fltGreater > strFifty);
        Assert.IsFalse(intLesser > strFifty);

        Assert.IsTrue(strFiftyOne >= fltLesser);
        Assert.IsTrue(strFiftyOne >= intGreater);
        Assert.IsTrue(fltGreater >= strFifty);
        Assert.IsFalse(intLesser >= strFifty);

        //Note: Since '!=', '<' and '<=' are all defined in terms of '==', '>' and '>=' there is little need for many tests.
        Assert.IsFalse(null != null);
        Assert.IsTrue(null != fltGreater);
        Assert.IsTrue(fltGreater != intGreater);
        Assert.IsFalse(fltGreater < intGreater);
        Assert.IsFalse(fltGreater <= intGreater);
    }
}