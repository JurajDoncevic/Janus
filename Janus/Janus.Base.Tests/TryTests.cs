using static Janus.Base.Try;
using static Janus.Base.ActionExtensions;
using static Janus.Base.UnitExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace Janus.Base.Tests
{

    public class TryTests
    {

        [Fact]
        public void UnitTrySuccessTest()
        {
            var unitTry =
                TryCatch(
                    ((Action)(() => Console.WriteLine())).ToFunc(),
                    (ex) => ex
                    );

            Assert.NotNull(unitTry);
            Assert.False(unitTry.IsData);
            Assert.False(unitTry.IsException);
            Assert.Equal(Unit(), unitTry.ExpectedData);
        }

        [Fact]
        public void DataTrySuccessTest()
        {
            const string testString = "Test";
            var stringTry =
                TryCatch(
                    () => testString,
                    (ex) => ex
                    );

            Assert.NotNull(stringTry);
            Assert.True(stringTry.IsData);
            Assert.False(stringTry.IsException);
            Assert.Equal(testString, stringTry.ExpectedData);
        }

        [Fact]
        public void TryExceptionTest()
        {
            const string exceptionMessage = "Some exception";

            var exceptionTry =
                TryCatch(
                    ((Action)(() =>
                    {
                        throw new Exception(exceptionMessage);
                    })).ToFunc(),
                    (ex) => ex
                    );

            Assert.NotNull(exceptionTry);
            Assert.False(exceptionTry.IsData);
            Assert.True(exceptionTry.IsException);
            Assert.Equal(exceptionMessage, exceptionTry.Exception.Message);
        }

        [Fact]
        public void TryExceptionSubclassTest()
        {

            var exceptionTry =
                TryCatch<Unit, ArgumentOutOfRangeException>(
                    ((Action)(() =>
                    {
                        throw new ArgumentOutOfRangeException();
                    })).ToFunc(),
                    (ex) => ex
                    );

            Assert.NotNull(exceptionTry);
            Assert.False(exceptionTry.IsData);
            Assert.True(exceptionTry.IsException);
            Assert.Equal(typeof(ArgumentOutOfRangeException), exceptionTry.Exception.GetType());
        }
    }
}
