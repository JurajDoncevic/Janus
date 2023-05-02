using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Janus.Base.Tests
{
    
    public class CompositionTests
    {

        [Fact]
        public void OneParamFuncBeforeTest()
        {
            Func<int, int> inc5 = Increment5;

            var operation =
                inc5.Before(Decrement2)
                    .Before(Decrement2)
                    .Before(Decrement2)
                    .Before(IsNegative);

            Assert.True(operation(0));
            Assert.False(operation(1));
        }

        [Fact]
        public void MultiParamFuncBeforeTest()
        {
            Func<int, int, int, int> sum3 = Sum3;

            var operation =
                sum3.Before(Increment5)
                    .Before(((Func<int, int, int>)(Sub)).Apply(5))
                    .Before(IsPositive);

            Assert.True(operation(0, 0, 0));
            Assert.False(operation(1, 0, 0));
        }

        [Fact]
        public void OneParamFuncAfterTest()
        {
            Func<int, bool> isNeg = IsNegative;

            var operation =
                isNeg.After((Func<int, int>)Decrement2)
                     .After((Func<int, int>)Decrement2)
                     .After((Func<int, int>)Decrement2)
                     .After((Func<int, int>)Increment5);
                     

            Assert.True(operation(0));
            Assert.False(operation(1));
        }

        [Fact]
        public void MultiParamFuncAfterTest()
        {
            Func<int, bool> isPos = IsPositive;

            var operation =
                isPos.After(((Func<int, int, int>)(Sub)).Apply(5))
                     .After((Func<int, int>)Increment5)
                     .After((Func<int, int, int, int>)Sum3);

            Assert.True(operation(0, 0, 0));
            Assert.False(operation(1, 0, 0));
        }


        #region TEST METHODS
        private int Increment5(int x) => x + 5;

        private int Decrement2(int x) => x - 2;

        private bool IsNegative(int x) => x < 0;

        private bool IsPositive(int x) => x >= 0;

        private int Add(int x, int y) => x + y;

        private int Sub(int x, int y) => x - y;

        private int Sum3(int x, int y, int z) => x + y + z;
        #endregion
    }
}
