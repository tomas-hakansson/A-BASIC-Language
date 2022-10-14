using A_BASIC_Language;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language_Tests
{
    [TestClass]
    public class Dimension_Tests
    {
        [TestMethod]
        public void blah()
        {
            //int[,] a1 = new int[2, 4];
            //int[,] a2 = new int[3, 4];
            //int[,,] a3 = new int[2, 3, 4];
            //List<DIM> ases = new();
            var shape = new List<int>() { 1, 2 };
            Dimension dim = new(shape);
            dim.Add(10, 0, 0);
            dim.Add(11, 0, 1);
            dim.Add(12, 0, 2);
            dim.Add(13, 1, 0);
            dim.Add(14, 1, 1);
            dim.Add(15, 1, 2);

            var value11 = dim.Get(1, 0);
            //ases.Add(new DIM(a1, new int[] {2, 4}));
            //ases.Add(new DIM(a3, new int[3] {2, 3, 4}));
            //ases.Add(a1);
            //ases.Add(a2);
            //ases.Add(a3);
        }
    }
}
