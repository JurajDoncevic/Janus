using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading.Tasks;

namespace Janus.Base.Tests
{
    public class AwaitingTests
    {
        [Fact]
        public void AwaitAnAsyncTaskTest()
        {
            int result =
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    return 1;
                }).WaitFor(r => r + 1);

            Assert.Equal(2, result);
        }

        [Fact]
        public void AwaitTaskUnderTimeoutTest()
        {
            int result =
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    return 1;
                }).WaitFor(r => r + 1, 1000);

            Assert.Equal(2, result);
        }

        [Fact]
        public void AwaitTaskOverTimeoutTest()
        {
            int result =
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(1000);
                    return 1;
                }).WaitFor(r => r + 1, 50);

            Assert.Equal(default(int), result);
        }
    }
}
