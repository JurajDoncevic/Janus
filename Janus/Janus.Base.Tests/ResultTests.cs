using static Janus.Base.Try;
using static Janus.Base.ActionExtensions;
using static Janus.Base.UnitExtensions;
using Janus.Base.Resulting;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Janus.Base;
using System.Threading.Tasks;

namespace Janus.Base.Tests
{
    public class ResultTests
    {

        [Fact(DisplayName = "Get a success result")]
        public void ResultSuccessTest()
        {
            var result =
                Resulting.Results.ToResult(TryCatch(
                    ((Action)(() => Console.WriteLine())).ToFunc(),
                    (ex) => ex
                    ));

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get an exception on Result")]
        public void ResultExceptionFailTest()
        {
            const string exceptionMessage = "Exception message";

            var result =
                Resulting.Results.ToResult(TryCatch(
                    ((Action)(() => { throw new Exception(exceptionMessage); })).ToFunc(),
                    (ex) => ex
                    ));

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
        }

        [Fact(DisplayName = "Get specific type of exception on Result")]
        public void ResultTypedExceptionFailTest()
        {
            var result =
                Resulting.Results.ToResult(TryCatch(
                    ((Action)(() => { throw new ArgumentNullException(); })).ToFunc(),
                    (ex) => ex
                    ));

            Assert.False(result);
            Assert.IsType<ArgumentNullException>(result.Exception);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
        }

        [Fact(DisplayName = "Get Result success")]
        public void ResultLogicSuccessTest()
        {

            var result =
                Resulting.Results.ToResult(TryCatch(
                    () => true,
                    (ex) => ex
                    ));

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure")]
        public void ResultLogicFailTest()
        {

            var result =
                Resulting.Results.ToResult(TryCatch(
                    () => false,
                    (ex) => ex
                    ));

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Use multiple Binds with success on Result")]
        public void ResultBindSuccessTest()
        {
            var result =
                GetLogicSuccess()
                .Bind(_ => GetSuccessResult())
                .Bind(_ => GetLogicSuccess())
                .Bind(_ => GetSuccessResult());

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure through Binds")]
        public void ResultBindLogicFailTest()
        {
            var result =
                GetLogicSuccess()
                .Bind(_ => GetSuccessResult())
                .Bind(_ => GetLogicFail())
                .Bind(_ => GetSuccessResult());

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Get Result exception through Binds")]
        public void ResultBindExceptionFailTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";
            var result =
                GetLogicSuccess()
                    .Bind(_ => GetSuccessResult())
                    .Bind(_ => GetExceptionFail(exceptionMessage))
                    .Bind(_ => GetSuccessResult());

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Contains(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Get Result success through async Binds")]
        public async void ResultBindSuccessAsyncTest()
        {
            var result =
                await GetLogicSuccessAsync()
                    .Bind(_ => GetSuccessResultAsync())
                    .Bind(_ => GetLogicSuccessAsync())
                    .Bind(_ => GetSuccessResultAsync());

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure through Binds async")]
        public async void ResultBindLogicFailAsyncTest()
        {
            var result =
                await GetLogicSuccessAsync()
                    .Bind(_ => GetSuccessResultAsync())
                    .Bind(_ => GetLogicFailAsync())
                    .Bind(_ => GetSuccessResultAsync());

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Get Result exception through Binds async")]
        public async void ResultBindExceptionFailAsyncTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";
            var result =
                await GetLogicSuccessAsync()
                    .Bind(_ => GetSuccessResultAsync())
                    .Bind(_ => GetExceptionFailAsync(exceptionMessage))
                    .Bind(_ => GetSuccessResultAsync());

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Contains(exceptionMessage, result.Message);
        }



        [Fact(DisplayName = "Get Result success through Fish chain")]
        public void ResultFishSuccessTest()
        {
            var composition =
                ((Func<bool, Result>)GetResultByLogic)
                    .Fish(_ => GetSuccessResult())
                    .Fish(_ => GetLogicSuccess())
                    .Fish(_ => GetSuccessResult());


            var result = composition(true);

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure through Fish chain")]
        public void ResultFishLogicFailTest()
        {
            var composition =
                ((Func<bool, Result>)GetResultByLogic)
                    .Fish(_ => GetSuccessResult())
                    .Fish(_ => GetLogicSuccess())
                    .Fish(_ => GetSuccessResult());


            var result = composition(false);

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Get Result exception through Fish chain")]
        public void ResultFishExceptionFailTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";

            var composition =
                ((Func<bool, Result>)GetResultByLogic)
                    .Fish(_ => GetSuccessResult())
                    .Fish(_ => GetExceptionFail(exceptionMessage))
                    .Fish(_ => GetSuccessResult());


            var result = composition(true);

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Contains(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Get Result success through Fish chain async")]
        public async void ResultFishSuccessAsyncTest()
        {
            var composition =
                ((Func<bool, Task<Result>>)GetResultByLogicAsync)
                    .Fish(_ => GetSuccessResultAsync())
                    .Fish(_ => GetLogicSuccessAsync())
                    .Fish(_ => GetSuccessResultAsync());


            var result = await composition(true);

            Assert.True(result);
            Assert.Equal(ResultTypes.SUCCESS, result.ResultType);
        }

        [Fact(DisplayName = "Get Result failure through Fish chain async")]
        public async void ResultFishLogicFailAsyncTest()
        {
            var composition =
                ((Func<bool, Task<Result>>)GetResultByLogicAsync)
                    .Fish(_ => GetSuccessResultAsync())
                    .Fish(_ => GetLogicSuccessAsync())
                    .Fish(_ => GetSuccessResultAsync());


            var result = await composition(false);

            Assert.False(result);
            Assert.Equal(ResultTypes.FAILURE, result.ResultType);
        }

        [Fact(DisplayName = "Get Result exception through Fish chain async")]
        public async void ResultFishExceptionFailAsyncTest()
        {
            string exceptionMessage = "EXCEPTION MESSAGE";

            var composition =
                ((Func<bool, Task<Result>>)GetResultByLogicAsync)
                    .Fish(_ => GetSuccessResultAsync())
                    .Fish(_ => GetExceptionFailAsync(exceptionMessage))
                    .Fish(_ => GetSuccessResultAsync());


            var result = await composition(true);

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Contains(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Get AsResult success async")]
        public async void AsResultSuccessAsyncTest()
        {
            var result = await Results.AsResult(async () => { await Task.Delay(0); return true; });

            Assert.True(result);
        }

        [Fact(DisplayName = "Get AsResult failure async")]
        public async void AsResultFailAsyncTest()
        {
            var result = await Results.AsResult(async () => { await Task.Delay(0); return false; });

            Assert.False(result);
        }

        [Fact(DisplayName = "Get AsResult exception async")]
        public async void AsResultExceptionAsyncTest()
        {
            var exceptionMessage = "test";
            var result = await Results.AsResult(async () => { await Task.Delay(0); throw new Exception(exceptionMessage); return true; });

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Contains(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Get AsResult success")]
        public void AsResultSuccessTest()
        {
            var result = Results.AsResult(() => true);

            Assert.True(result);
        }

        [Fact(DisplayName = "Get AsResult failure")]
        public void AsResultFailTest()
        {
            var result = Results.AsResult(() => false);

            Assert.False(result);
        }

        [Fact(DisplayName = "Get AsResult exception")]
        public void AsResultExceptionTest()
        {
            var exceptionMessage = "test";
            var result = Results.AsResult(() => { throw new Exception(exceptionMessage); return true; });

            Assert.False(result);
            Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
            Assert.Contains(exceptionMessage, result.Message);
        }

        [Fact(DisplayName = "Run AsResult with operation returning Result")]
        public void AsResultAcceptingResultTest()
        {
            var successMessage = "Test success!";
            var failureMessage = "Test failure!";
            var exceptionToThrow = new Exception("Exception failure");

            var operationSuccess = () =>
            {
                return Results.OnSuccess(successMessage);
            };

            var operationFailure = () =>
            {
                return Results.OnFailure(failureMessage);
            };

            var operationException = () =>
            {
                return Results.OnException(exceptionToThrow);
            };

            var operationThrownException = () =>
            {
                throw exceptionToThrow;
                return Results.OnSuccess(successMessage); return Results.OnException(exceptionToThrow);
            };

            var successResult = Results.AsResult(operationSuccess);
            var failureResult = Results.AsResult(operationFailure);
            var exceptionResult = Results.AsResult(operationException);
            var exceptionThrownResult = Results.AsResult(operationThrownException);

            Assert.Equal(successMessage, successResult.Message);
            Assert.Equal(failureMessage, failureResult.Message);
            Assert.Equal(exceptionToThrow, exceptionResult.Exception);
            Assert.Equal(exceptionToThrow, exceptionThrownResult.Exception);
        }

        [Fact(DisplayName = "Run AsResult async with operation returning Result")]
        public async void AsResultAcceptingResultWithDataTest()
        {
            var successMessage = "Test success!";
            var failureMessage = "Test failure!";
            var exceptionToThrow = new Exception("Exception failure");

            var operationSuccess = async () =>
            {
                return await Task.FromResult(Results.OnSuccess(1, successMessage)); ;
            };

            var operationFailure = async () =>
            {
                return await Task.FromResult(Results.OnFailure<int>(failureMessage));
            };

            var operationException = async () =>
            {
                return await Task.FromResult(Results.OnException<int>(exceptionToThrow));
            };

            var operationThrownException = async () =>
            {
                throw exceptionToThrow;
                return await Task.FromResult(Results.OnSuccess(1, successMessage));
            };

            var successResult = await Results.AsResult(operationSuccess);
            var failureResult = await Results.AsResult(operationFailure);
            var exceptionResult = await Results.AsResult(operationException);
            var exceptionThrownResult = await Results.AsResult(operationThrownException);

            Assert.Equal(successMessage, successResult.Message);
            Assert.Equal(1, successResult.Data);
            Assert.Equal(failureMessage, failureResult.Message);
            Assert.Equal(exceptionToThrow, exceptionResult.Exception);
            Assert.Equal(exceptionToThrow, exceptionThrownResult.Exception);
        }

        [Fact(DisplayName = "Run AsResult async with operation returning Result")]
        public async void AsResultAsyncAcceptingResultWithDataTest()
        {
            var successMessage = "Test success!";
            var failureMessage = "Test failure!";
            var exceptionToThrow = new Exception("Exception failure");

            var operationSuccess = async () =>
            {
                return await Task.FromResult(Results.OnSuccess(1, successMessage));
            };

            var operationFailure = async () =>
            {
                return await Task.FromResult(Results.OnFailure<int>(failureMessage));
            };

            var operationException = async () =>
            {
                return await Task.FromResult(Results.OnException<int>(exceptionToThrow));
            };

            var operationThrownException = async () =>
            {
                throw exceptionToThrow;
                return await Task.FromResult(Results.OnSuccess(1, successMessage));
            };

            var successResult = await Results.AsResult(operationSuccess);
            var failureResult = await Results.AsResult(operationFailure);
            var exceptionResult = await Results.AsResult(operationException);
            var exceptionThrownResult = await Results.AsResult(operationThrownException);

            Assert.Equal(successMessage, successResult.Message);
            Assert.Equal(1, successResult.Data);
            Assert.Equal(failureMessage, failureResult.Message);
            Assert.Equal(exceptionToThrow, exceptionResult.Exception);
            Assert.Equal(exceptionToThrow, exceptionThrownResult.Exception);
        }

        #region HELPER METHODS
        private Result GetLogicFail()
            => Resulting.Results.ToResult(TryCatch(
                    () => false,
                    (ex) => ex
                    ));

        private Result GetLogicSuccess()
            => Resulting.Results.ToResult(TryCatch(
                    () => true,
                    (ex) => ex
                    ));

        private Result GetSuccessResult()
            => Resulting.Results.ToResult(TryCatch(
                    ((Action)(() => Console.WriteLine())).ToFunc(),
                    (ex) => ex
                    ));

        private Result GetExceptionFail(string exceptionMessage)
            => Resulting.Results.ToResult(TryCatch(
                    ((Action)(() => { throw new Exception(exceptionMessage); })).ToFunc(),
                    (ex) => ex
                    ));

        private async Task<Result> GetLogicFailAsync()
            => await Resulting.Results.ToResult(TryCatch(
                    async () => { await Task.Run(() => 0); return false; },
                    (ex) => ex
                    ));

        private async Task<Result> GetLogicSuccessAsync()
            => await Resulting.Results.ToResult(TryCatch(
                    async () => { await Task.Run(() => 0); return true; },
                    (ex) => ex
                    ));

        private async Task<Result> GetSuccessResultAsync()
            => await Resulting.Results.ToResult(TryCatch(
                    async () => (await Task.Run(() => 0)).Ignore(),
                    (ex) => ex
                    ));

        private async Task<Result> GetExceptionFailAsync(string exceptionMessage)
            => await Resulting.Results.ToResult(TryCatch(
                    async () => { throw new Exception(exceptionMessage); return await Task.Run(() => UnitExtensions.Unit()); },
                    (ex) => ex
                    ));

        private Result GetResultByLogic(bool isSuccess)
            => Resulting.Results.ToResult(TryCatch(
                    () => isSuccess,
                    (ex) => ex
                    ));

        private async Task<Result> GetResultByLogicAsync(bool isSuccess)
            => await Resulting.Results.ToResult(TryCatch(
                    async () => { await Task.Run(() => 0); return isSuccess; },
                    (ex) => ex
                    ));
        #endregion
    }
}
