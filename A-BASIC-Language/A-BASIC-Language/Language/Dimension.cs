using System;
using System.Collections.Generic;
using System.Linq;
using A_BASIC_Language.Language.ValueTypes;
#if DEBUG
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace A_BASIC_Language.Language;

public class Dimension
{
    readonly ValueBase[] _atoms;
    readonly int[] _multipliers;
    readonly int[] _shape;
    public int IndexCount { get; }

    /// <summary>
    /// Test only.
    /// </summary>
    /// <param name="maxIndices"></param>
    public Dimension(IEnumerable<int> maxIndices) : this(maxIndices, new IntValue(0)) { }

    public Dimension(IEnumerable<int> maxIndices, ValueBase defaultValue)
    {
        //ToDo: Validate input.
        //Ponder: Maybe have an optimisation that skips all the machinery for one-dimensional arrays?
        //Hack: I first wrote the code under the assumption that the constructor would be given the shape as argument.
        var bounds = maxIndices.ToArray();

        if (bounds.Length == 0 || bounds.Any(x => x < 0))
            throw new ArgumentException("Invalid array dimensions.");
        
        var shape = new IntegerList(bounds.Select(x => checked(x + 1)).ToList());
        _shape = shape.ToArray();
        _atoms = new ValueBase[shape.Aggregate((x, y) => checked(x * y))];
        Fill(ref _atoms, defaultValue);
        IndexCount = shape.Count;
        _multipliers = new int[IndexCount];
        _multipliers[IndexCount - 1] = 1;

        for (int i = 1, j = IndexCount - 2; i < IndexCount; i++, j--)
            _multipliers[j] = shape.TakeLast(i).Aggregate((x, y) => x * y);
    }

    private static void Fill(ref ValueBase[] array, ValueBase value)
    {
        for (var i = 0; i < array.Length; i++)
            array[i] = value;
    }

    /// <summary>
    /// This method is test only. It is hardcoded to only create IntValues and it has a params parameter.
    /// </summary>
    /// <param name="atom"></param>
    /// <param name="indices"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Add(int atom, params int[] indices) =>
        Add(new IntValue(atom), indices.ToList());

    public void Add(double atom, List<int> indices) =>
        Add(new FloatValue(atom), indices.ToList());

    public void Add(int atom, List<int> indices) =>
        Add(new IntValue(atom), indices.ToList());

    public void Add(string atom, List<int> indices) =>
        Add(new StringValue(atom), indices.ToList());

    public void Add(ValueBase atom, List<int> indices) =>
        _atoms[ToOneDimensionalIndex(indices)] = atom;

    /// <summary>
    /// This method is test only.
    /// </summary>
    /// <param name="indices"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int Get(params int[] indices) =>
        (int)Get(indices.ToList()).GetValueAsType<IntValue>();

    public ValueBase Get(List<int> indices) =>
        _atoms[ToOneDimensionalIndex(indices)];

    private int ToOneDimensionalIndex(IEnumerable<int> indices)
    {
        var subscripts = indices.ToArray();
        
        if (subscripts.Length != IndexCount)
            throw new ArgumentException("Wrong number of array subscripts.");
        
        var offset = 0;
        
        for (var i = 0; i < IndexCount; i++)
        {
            if (subscripts[i] < 0 || subscripts[i] >= _shape[i])
                throw new ArgumentOutOfRangeException(nameof(indices), @"Array subscript out of range.");
            
            offset += subscripts[i] * _multipliers[i];
        }

        return offset;
    }
}

#if DEBUG
[TestClass]
public class DimensionTests
{
    [TestMethod]
    public void MultidimensionalArrayStoresValuesInDistinctCells()
    {
        var array = new Dimension([1, 2, 3], new StringValue(""));
        var first = new List<int> { 0, 2, 3 };
        var second = new List<int> { 1, 0, 0 };
        var last = new List<int> { 1, 2, 3 };

        array.Add("first", first);
        array.Add("second", second);
        array.Add("last", last);

        Assert.AreEqual("first", ((StringValue)array.Get(first)).Value);
        Assert.AreEqual("second", ((StringValue)array.Get(second)).Value);
        Assert.AreEqual("last", ((StringValue)array.Get(last)).Value);
        Assert.AreEqual("", ((StringValue)array.Get(new List<int> { 0, 0, 0 })).Value);
    }

    [TestMethod]
    public void RejectsInvalidSubscriptsBeforeAccessingStorage()
    {
        var array = new Dimension([1, 2]);
        Assert.ThrowsExactly<ArgumentException>(() => array.Get(1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.Get(1, -1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => array.Add(99, 0, 3));
        Assert.AreEqual(0, array.Get(1, 0));
    }
}
#endif
