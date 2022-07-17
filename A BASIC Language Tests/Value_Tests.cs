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
}