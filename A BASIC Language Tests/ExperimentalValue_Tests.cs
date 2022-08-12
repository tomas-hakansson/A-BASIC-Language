using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_BASIC_Language.Experiments.TryingToUnderstandValueTypes;

namespace A_BASIC_Language_Tests;

[TestClass]
public class ExperimentalValue_Tests
{
    [TestMethod]
    public void MyTestMethod()
    {
        var equalString = new StringValue("50");
        var equalFloat = new FloatValue(50.0);
        var equalInt = new IntValue(50);

        var unequalString = new StringValue("51");
        var unequalFloat = new FloatValue(50.1);
        var unequalInt = new IntValue(49);

        var aNull = new NullValue(42);
        var anotherNull = new NullValue(-1);

        Assert.IsTrue(equalString == equalFloat);
        Assert.IsTrue(equalString == equalInt);
        Assert.IsTrue(equalFloat == equalString);
        Assert.IsTrue(equalFloat == equalInt);
        Assert.IsTrue(equalInt == equalString);
        Assert.IsTrue(equalInt == equalFloat);

        Assert.IsTrue(unequalString != equalFloat);
        Assert.IsTrue(unequalString != equalInt);
        Assert.IsTrue(unequalFloat != equalString);
        Assert.IsTrue(unequalFloat != equalInt);
        Assert.IsTrue(unequalInt != equalString);
        Assert.IsTrue(unequalInt != equalFloat);

        Assert.IsTrue(aNull != equalString);
        Assert.IsTrue(aNull != equalFloat);
        Assert.IsTrue(aNull != equalInt);
        Assert.IsTrue(aNull == anotherNull);
    }
}
