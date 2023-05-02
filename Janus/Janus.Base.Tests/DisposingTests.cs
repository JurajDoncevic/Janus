using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Base.Tests
{
    public class DisposingTests
    {
        [Fact]
        public void UsingDisposedCorrectlyTest()
        {
            MockDisposable mockDisposable = null;
            var result = 
                Disposing.Using(() => { mockDisposable = new MockDisposable(); return mockDisposable; },
                                disp => disp.RunOperation());

            Assert.NotNull(mockDisposable);
            Assert.Equal(1, result);
            Assert.True(mockDisposable.HasRunOperation);
            Assert.True(mockDisposable.IsDisposed);

        }

        [Fact]
        public async void UsingDisposedCorrectlyAsyncTest()
        {
            MockDisposable mockDisposable = null;
            var result =
                await Disposing.Using(() => { mockDisposable = new MockDisposable(); return mockDisposable; },
                                      disp => Task.Run(disp.RunOperation));

            Assert.NotNull(mockDisposable);
            Assert.Equal(1, result);
            Assert.True(mockDisposable.HasRunOperation);
            Assert.True(mockDisposable.IsDisposed);

        }


        private class MockDisposable : IDisposable
        {
            public bool IsDisposed { get; private set; }
            public bool HasRunOperation { get; private set; }

            public MockDisposable()
            {
                IsDisposed = false;
                HasRunOperation = false;
            }

            public int RunOperation()
            {
                HasRunOperation = true;
                return 1;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
