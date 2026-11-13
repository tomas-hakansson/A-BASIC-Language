using System;
using System.Globalization;
#if DEBUG
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace A_BASIC_Language;

public static class Extensions
{
    extension(double n)
    {
        public bool NanOrInfinity() =>
            double.IsNaN(n) || double.IsInfinity(n);

        public bool NanOrInfinityOrNegative() =>
            double.IsNaN(n) || double.IsInfinity(n) || n < 0;

        public bool IsByte() =>
            n.IsInt(0, 255);

        public bool IsInt(int min, int max) =>
            !(double.IsNaN(n) || double.IsInfinity(n) || n < min || n > max || Math.Abs(n - Math.Truncate(n)) > 0.001);

    }

    extension(string s)
    {
        public bool ParseDouble(out double n) =>
            double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out n);
    }
}

#if DEBUG
[TestClass]
public class ExtensionsTests
{
    [TestMethod]
    public void NanOrInfinityOrNegative()
    {
        Assert.IsFalse(55.0.NanOrInfinity());
        Assert.IsFalse((-55.0).NanOrInfinity());
        Assert.IsFalse(55.0.NanOrInfinityOrNegative());
        Assert.IsTrue((-55.0).NanOrInfinityOrNegative());
    }

    [TestMethod]
    public void IsByte()
    {
        Assert.IsTrue(0.0.IsByte());
        Assert.IsTrue(255.0.IsByte());
        Assert.IsFalse((-1.0).IsByte());
        Assert.IsFalse(256.0.IsByte());
        Assert.IsFalse(55.5.IsByte());
    }

    [TestMethod]
    public void ParseDouble()
    {
        Assert.IsTrue("55.0".ParseDouble(out var n1) && Math.Abs(n1 - 55.0) < 0.0004);
        Assert.IsFalse("abc".ParseDouble(out _));
    }

    [TestMethod]
    public void NonFiniteValuesAreRejectedByNumericChecks()
    {
        foreach (var value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
        {
            Assert.IsTrue(value.NanOrInfinity());
            Assert.IsTrue(value.NanOrInfinityOrNegative());
            Assert.IsFalse(value.IsByte());
            Assert.IsFalse(value.IsInt(-10, 10));
        }
    }

    [TestMethod]
    public void IsIntIncludesBothBounds()
    {
        Assert.IsTrue((-10.0).IsInt(-10, 10));
        Assert.IsTrue(10.0.IsInt(-10, 10));
        Assert.IsFalse((-11.0).IsInt(-10, 10));
        Assert.IsFalse(11.0.IsInt(-10, 10));
    }

    [TestMethod]
    public void IsIntAllowsSmallDifferencesFromTruncatedValue()
    {
        Assert.IsTrue(3.0005.IsInt(-10, 10));
        Assert.IsTrue((-3.0005).IsInt(-10, 10));
        Assert.IsFalse(3.002.IsInt(-10, 10));
        Assert.IsFalse((-3.002).IsInt(-10, 10));
    }

    [TestMethod]
    public void ParseDoubleUsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("sv-SE");
            Assert.IsTrue("  -1.25e2  ".ParseDouble(out var value));
            Assert.AreEqual(-125.0, value);
            Assert.IsFalse("1,25".ParseDouble(out _));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [TestMethod]
    public void ParseDoubleRejectsEmptyAndPartialNumbers()
    {
        Assert.IsFalse("".ParseDouble(out _));
        Assert.IsFalse("   ".ParseDouble(out _));
        Assert.IsFalse("44x".ParseDouble(out _));
    }
}
#endif
