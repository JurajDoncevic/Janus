using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using System.Threading.Tasks;

namespace Janus.Base.Tests
{
    public class ForkingTests
    {
        [Fact]
        public void ForkSimpleTest()
        {
            int x = 0;
            var result =
                x.Fork(
                    _ => _.Sum(),
                    _ => _ + 1,
                    _ => _ + 2,
                    _ => _ + 3
                    );

            Assert.Equal(6, result);
        }

        [Fact]
        public async void ForkAsyncFuncsTest()
        {
            int x = 0;
            var result =
                await
                    x.Fork(
                        _ => _.Sum(),
                        async _ => await Task.FromResult(_ + 1),
                        async _ => await Task.FromResult(_ + 2),
                        async _ => await Task.FromResult(_ + 3)
                        );

            Assert.Equal(6, result);
        }

        [Fact]
        public async void ForkFullAsyncFuncsTest()
        {
            int x = 0;
            var result =
                await
                    x.Fork(
                        async _ => (await Task.WhenAll(_)).Sum(),
                        async _ => await Task.FromResult(_ + 1),
                        async _ => await Task.FromResult(_ + 2),
                        async _ => await Task.FromResult(_ + 3)
                        );

            Assert.Equal(6, result);
        }

        [Fact]
        public async void ForkAsyncNoParamFuncsTest()
        {
            var result =
                await
                    Forking.Fork(
                        _ => _.Sum(),
                        async () => await Task.FromResult(1),
                        async () => await Task.FromResult(2),
                        async () => await Task.FromResult(3)
                        );

            Assert.Equal(6, result);
        }
    }
}
