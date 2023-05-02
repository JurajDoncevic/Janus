using Janus.Base.Resulting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Base.Tests
{
    public class TimingTests
    {
        [Fact, Trait("Category", "TimingTest")]
        public void TestBlockingResultTimeout()
        {
            Func<int> shortOp = () => { Task.Delay(50).Wait(); return 50; };
            Func<int> longOp = () => { Task.Delay(1500).Wait(); return 1500; };

            var shortOpResult = shortOp.RunWithTimeout(700);
            var longOpResult = longOp.RunWithTimeout(700);

            Assert.True(shortOpResult.IsSuccess);
            Assert.True(shortOpResult.HasData);
            Assert.Equal(50, shortOpResult.Data);

            Assert.False(longOpResult);
        }

        [Fact, Trait("Category", "TimingTest")]
        public void BlockingResultTest()
        {
            var operation = (CancellationToken token) =>
            {
                Task.Delay(20).Wait();
                token.ThrowIfCancellationRequested();
                return 64;
            };

            var result = operation.RunWithTimeout(700);

            Assert.True(result);
            Assert.True(result.HasData);
            Assert.Equal(64, result.Data);

        }

        [Fact, Trait("Category", "TimingTest")]
        public void BlockingResultWhileTrueTest()
        {
            var whileTrueOp = new Func<CancellationToken, int>(token => { while (true) { token.ThrowIfCancellationRequested(); } return 64; });

            var result = whileTrueOp.RunWithTimeout(700);

            Assert.False(result);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.Message);

        }

        [Fact, Trait("Category", "TimingTest")]
        public void BlockingResultWhileTrueThrowsExceptionTest()
        {
            var whileTrueOp = new Func<CancellationToken, int>(token => { while (true) { token.ThrowIfCancellationRequested(); throw new Exception("TEST"); } return 64; });

            var result = whileTrueOp.RunWithTimeout(700);

            Assert.False(result);
            Assert.Contains("TEST", result.Message);

        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationTest()
        {
            Func<CancellationToken, Task<int>> operation =
                async token =>
                {
                    await Task.Delay(1);
                    token.ThrowIfCancellationRequested();
                    return 700;
                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
            Assert.Equal(700, result.Data);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationWhileTrueTest()
        {
            Func<CancellationToken, Task<int>> operation =
                async token =>
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);
                    }
                    return 700;
                };

            var result = await operation.RunWithTimeout(700);

            Assert.False(result);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationWhileTrueThrowsExceptionTest()
        {
            Func<CancellationToken, Task<int>> operation =
                async token =>
                {
                    while (true)
                    {
                        throw new Exception("TEST");
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);

                    }
                    return 700;
                };

            var result = await operation.RunWithTimeout(700000);

            Assert.False(result);
            Assert.Contains("TEST", result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncResultOperationTest()
        {
            Func<CancellationToken, Task<Result<int>>> operation =
                async token =>
                {
                    return await Results.AsResult(async () =>
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
            Assert.Equal(700, result.Data);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncResultOperationWhileTrueTest()
        {
            Func<CancellationToken, Task<Result<int>>> operation =
                async token =>
                {
                    return await Results.AsResult(async () =>
                    {
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            await Task.Delay(1);
                        }
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.False(result);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncResultOperationWhileTrueThrowsExceptionTest()
        {
            Func<CancellationToken, Task<Result<int>>> operation =
                async token =>
                {
                    return await Results.AsResult(async () =>
                    {
                        while (true)
                        {
                            throw new Exception("TEST");
                            token.ThrowIfCancellationRequested();
                            await Task.Delay(1);
                        }
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.False(result);
            Assert.Contains("TEST", result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void ResultOperationTest()
        {
            Func<CancellationToken, Result<int>> operation =
                token =>
                {
                    return Results.AsResult(() =>
                    {
                        token.ThrowIfCancellationRequested();
                        Task.Delay(1).Wait();
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
            Assert.Equal(700, result.Data);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void ResultOperationWhileTrueTest()
        {
            Func<CancellationToken, Result<int>> operation =
                token =>
                {
                    return Results.AsResult(() =>
                    {
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            Task.Delay(1).Wait();
                        }
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.False(result);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void ResultOperationWhileTrueThrowsExceptionTest()
        {
            Func<CancellationToken, Result<int>> operation =
                token =>
                {
                    return Results.AsResult(() =>
                    {
                        while (true)
                        {
                            throw new Exception("TEST");
                            token.ThrowIfCancellationRequested();
                            Task.Delay(1).Wait();
                        }
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.False(result);
            Assert.Contains("TEST", result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public void OperationBoolResultTest()
        {
            var operation = (CancellationToken token) =>
            {
                token.ThrowIfCancellationRequested();
                Task.Delay(1).Wait();
                return true;
            };

            var result = operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "TimingTest")]
        public void OperationBoolWhileTrueResultTest()
        {
            var operation = (CancellationToken token) =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    Task.Delay(1).Wait();
                }
                return true;
            };

            var result = operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public void OperationBoolWhileTrueResultExceptionTest()
        {
            var operation = (CancellationToken token) =>
            {
                while (true)
                {
                    throw new Exception("TEST");
                    token.ThrowIfCancellationRequested();
                    Task.Delay(1).Wait();
                }
                return true;
            };

            var result = operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Contains("TEST", result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationBoolResultTest()
        {
            var operation = async (CancellationToken token) =>
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1);

                return true;
            };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationBoolWhileTrueResultTest()
        {
            var operation = async (CancellationToken token) =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1);
                }
                return true;
            };

            var result = await operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationBoolWhileTrueResultExceptionTest()
        {
            var operation = async (CancellationToken token) =>
            {
                while (true)
                {
                    throw new Exception("TEST");
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1);
                }
                return true;
            };

            var result = await operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Contains("TEST", result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void OperationResultTimeoutTest()
        {
            var operation = (CancellationToken token) => Results.AsResult(
                () =>
                {
                    Task.Delay(1).Wait();
                    token.ThrowIfCancellationRequested();
                    return true;
                });


            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void OperationResultWhileTrueTimeoutTest()
        {
            var operation = (CancellationToken token) => Results.AsResult(
                () =>
                {
                    while (true)
                    {
                        Task.Delay(1).Wait();
                        token.ThrowIfCancellationRequested();
                    }
                    return true;
                });


            var result = await operation.RunWithTimeout(700);
            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.Message);

        }

        [Fact, Trait("Category", "TimingTest")]
        public async void OperationResultWhileTrueExceptionTest()
        {
            var operation = (CancellationToken token) => Results.AsResult(
                () =>
                {
                    while (true)
                    {
                        throw new Exception("TEST");
                        Task.Delay(1).Wait();
                        token.ThrowIfCancellationRequested();
                    }
                    return true;
                });


            var result = await operation.RunWithTimeout(700);
            Assert.False(result.IsSuccess);
            Assert.Contains("TEST", result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationResultTimeoutTest()
        {
            var operation = async (CancellationToken token) => await Results.AsResult(
                async () =>
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1);
                    return true;
                });


            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationWhileTrueResultTimeoutTest()
        {
            var operation = async (CancellationToken token) => await Results.AsResult(
                async () =>
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);
                    }

                    return true;
                });


            var result = await operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.Message);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationWhileTrueThrowsExceptionResultTimeoutTest()
        {
            var operation = async (CancellationToken token) => await Results.AsResult(
                async () =>
                {
                    while (true)
                    {
                        throw new Exception("TEST");
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);
                    }

                    return true;
                });


            var result = await operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Contains("TEST", result.Message);
        }



    }
}
