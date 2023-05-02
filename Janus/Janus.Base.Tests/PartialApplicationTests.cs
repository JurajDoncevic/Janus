using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Janus.Base.PartialApplication;

namespace Janus.Base.Tests
{
    public class PartialApplicationTests
    {
        [Fact]
        public void OneArgumentFunctionApplyTest()
        {
            Func<int, int> f = (x) => x + 1;

            var result = f.Apply(2)();

            Assert.Equal(3, result);
        }

        [Fact]
        public void TwoArgumentFunctionApplyTest()
        {
            Func<int, int, int> f = (x, y) => x + y;
            Func<int, int> g = f.Apply(2);

            var result1 = g.Apply(1)();
            var result2 = g.Apply(3)();

            Assert.Equal(3, result1);
            Assert.Equal(5, result2);
        }

        [Fact]
        public void ThreeArgumentFunctionApplyTest()
        {
            Func<int, int, int, int> f = (x, y, z) => x + y + z;
            Func<int, int> g = f.Apply(2).Apply(3);
            
            var result1 = g.Apply(1)();
            var result2 = g.Apply(4)();

            Assert.Equal(6, result1);
            Assert.Equal(9, result2);
        }

        [Fact]
        public void ThreeArgumentMethodApplyTest()
        {
            Func<int, int, int, int> f = ProdFunction;

            Assert.Equal(24, f.Apply(2).Apply(3).Apply(4)());
        }

        public int ProdFunction(int x, int y, int z) =>
            x * y * z;
    }
}
