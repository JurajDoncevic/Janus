using Janus.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Janus.Base.Tests
{
    public class UnitAndPipingTests
    {
        [Fact]
        public void ActionToFuncConversionTest()
        {
            int z = 0;
            Action<int, int> action = (x, y) => { z = x + y; return; };

            var func = action.ToFunc();

            var result = func(1, 2);

            Assert.Equal(UnitExtensions.Unit(), result);
            Assert.Equal(3, z);
        }

        [Fact]
        public void IgnoreTest()
        {
            Func<int, int, int> func = (x, y) => x + y;

            var result = func(1, 2).Ignore();

            Assert.Equal(UnitExtensions.Unit(), result);
        }

        [Fact]
        public void PassOverPrimitiveTest()
        {
            Func<int, int, int> func = (x, y) => x + y;

            var result = func(1, 2).Pass(_ => { _ = _ + 1; return _.Ignore(); });

            Assert.Equal(3, result);
        }

        [Fact]
        public void PassOverClassTest()
        {
            Func<string, string, PersonClass> func = (x, y) => new PersonClass { FirstName = x, LastName = y };

            var person = func("John", "Doe").Pass(_ => { _.LastName = "Smith"; return _.Ignore(); });

            Assert.Equal("Smith", person.LastName);
        }

        [Fact]
        public void PassOverStructTest()
        {
            Func<string, string, PersonStruct> func = (x, y) => new PersonStruct { FirstName = x, LastName = y };

            var person = func("John", "Doe").Pass(_ => { _.LastName = "Smith"; return _.Ignore(); });

            Assert.Equal("Doe", person.LastName);
        }

        [Fact]
        public void IfNotNullTest()
        {
            var person = new PersonClass() { FirstName = "John", LastName = "Doe" };

            var resultTrue = 
                person.IfNotNull(_ => _.LastName)
                      .IfNotNull(_ => _.Trim())
                      .IfNotNull(_ => (bool?)(_.Length == 3));

            var resultNull =
                person.IfNotNull(_ => _.LastName)
                      .IfNotNull(_ => (int?) null)
                      .IfNotNull(_ => (bool?)(_ == 3));

            Assert.True(resultTrue);
            Assert.Null(resultNull);
        }

        private struct PersonStruct
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        private class PersonClass
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}
