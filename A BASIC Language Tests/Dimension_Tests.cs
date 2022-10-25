using A_BASIC_Language;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace A_BASIC_Language_Tests;

[TestClass]
public class Dimension_Tests
{
    [TestMethod]
    public void Dimension1()
    {
        var maxIndices = new List<int>() { 1 };
        Dimension dim = new(maxIndices);
        dim.Add(1, 0);
        dim.Add(2, 1);

        Assert.AreEqual(1, dim.Get(0));
        Assert.AreEqual(2, dim.Get(1));
    }

    [TestMethod]
    public void Dimension2()
    {
        var maxIndices = new List<int>() { 1, 2 };
        Dimension dim = new(maxIndices);
        dim.Add(10, 0, 0);
        dim.Add(11, 0, 1);
        dim.Add(12, 0, 2);
        dim.Add(13, 1, 0);
        dim.Add(14, 1, 1);
        dim.Add(15, 1, 2);

        Assert.AreEqual(10, dim.Get(0, 0));
        Assert.AreEqual(11, dim.Get(0, 1));
        Assert.AreEqual(12, dim.Get(0, 2));
        Assert.AreEqual(13, dim.Get(1, 0));
        Assert.AreEqual(14, dim.Get(1, 1));
        Assert.AreEqual(15, dim.Get(1, 2));
    }

    [TestMethod]
    public void Dimension3()
    {
        var maxIndices = new List<int>() { 1, 2, 3 };
        Dimension dim = new(maxIndices);
        dim.Add(1, 0, 0, 0);
        dim.Add(2, 0, 0, 1);
        dim.Add(3, 0, 0, 2);
        dim.Add(4, 0, 0, 3);
        dim.Add(5, 0, 1, 0);
        dim.Add(6, 0, 2, 0);
        dim.Add(7, 1, 2, 3);

        Assert.AreEqual(1,dim.Get(0, 0, 0));
        Assert.AreEqual(2,dim.Get(0, 0, 1));
        Assert.AreEqual(3,dim.Get(0, 0, 2));
        Assert.AreEqual(4,dim.Get(0, 0, 3));
        Assert.AreEqual(5,dim.Get(0, 1, 0));
        Assert.AreEqual(6,dim.Get(0, 2, 0));
        Assert.AreEqual(7, dim.Get(1, 2, 3));

    }
}
