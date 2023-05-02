using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using Janus.Base;

namespace Janus.Base.Tests
{
    public class OptionTests
    {
        [Fact]
        public void SomeMatchTest()
        {
            var result =
                Option<int>.Some(2)
                    .Match(
                        _ => _ + 1,
                        () => 1
                    );

            Assert.Equal(3, result);
        }

        [Fact]
        public void NoneMatchTest()
        {
            var result =
                Option<int>.None
                    .Match(
                        _ => _ + 1,
                        () => 1
                    );

            Assert.Equal(1, result);
        }

        [Fact]
        public void OptionMapSomeTest()
        {
            var result =
                Option<string>.Some("This ")
                    .Map(_ => _ + "is ")
                    .Map(_ => _ + "some ")
                    .Map(_ => _ + "text.");

            Assert.Equal("This is some text.", result.Value);
        }

        [Fact]
        public void OptionMapNoneTest()
        {
            var result =
                Option<string>.None
                    .Map(_ => _ + "is ")
                    .Map(_ => _ + "some ")
                    .Map(_ => _ + "text.");

            Assert.False(result.IsSome);
            Assert.Equal(Option<string>.None, result);
        }
    }
}
