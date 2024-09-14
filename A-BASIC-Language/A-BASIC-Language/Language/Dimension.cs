using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language.Language;

public class Dimension
{
    readonly ValueBase[] _atoms;
    readonly int[] _multipliers;
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
        var shape = maxIndices.Select(x => x + 1).ToList();

        _atoms = new ValueBase[shape.Aggregate((x, y) => x * y)];

        Array.Fill(_atoms, defaultValue);

        IndexCount = shape.Count;

        _multipliers = new int[IndexCount];

        _multipliers[IndexCount - 1] = 1;

        for (int i = 1, j = IndexCount - 2; i < IndexCount; i++, j--)
            _multipliers[j] = shape.TakeLast(i).Aggregate((x, y) => x * y);
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

    int ToOneDimensionalIndex(IEnumerable<int> indices)
    {
        //ToDo: Validate input.
        return _multipliers.Zip(indices, (x, y) => x * y).Aggregate((x, y) => x + y);
    }
}