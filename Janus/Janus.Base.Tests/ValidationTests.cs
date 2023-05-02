using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Janus.Base.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void ValidationPassTest()
        {
            var person = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                Age = 24
            };

            var result = person.Validate(
                _ => _.FirstName.Length > 2,
                _ => _.LastName.Length > 2,
                _ => _.Age > 18
                );

            Assert.True(result);
        }

        [Fact]
        public void ValidationFailTest()
        {
            var person = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                Age = 16
            };

            var result = person.Validate(
                _ => _.FirstName.Length > 2,
                _ => _.LastName.Length > 2,
                _ => _.Age > 18
                );

            Assert.False(result);
        }


        [Fact]
        public void ValidatorTest()
        {
            var person1 = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                Age = 24
            };

            var person2 = new Person
            {
                FirstName = "Joe",
                LastName = "Smith",
                Age = 18
            };

            var validator = 
                Validation.Validator<Person>(
                    _ => _.FirstName.Length > 2,
                    _ => _.LastName.Length > 2,
                    _ => _.Age > 18
                    );

            var result1 = validator(person1);
            var result2 = validator(person2);

            Assert.True(result1);
            Assert.False(result2);
        }

        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
        }
    }
}
