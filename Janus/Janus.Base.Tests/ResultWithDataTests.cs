using System;
using System.Collections.Generic;
using System.Text;
using Janus.Base.Resulting;
using static Janus.Base.Try;
using Xunit;
using Janus.Base;
using System.Threading.Tasks;

namespace Janus.Base.Tests;

public class ResultWithDataTests
{

    [Fact(DisplayName = "Get success Result with data")]
    public void ResultSuccessTest()
    {
        var dataResult =
            TryCatch(
                () => new { Name = "test" },
                (ex) => ex
                ).ToResult();


        Assert.True(dataResult.IsSuccess);

        Assert.Equal("test", dataResult.Data.Name);
    }

    [Fact(DisplayName = "Get exception Result with data")]
    public void ResultExceptionFailTest()
    {
        const string exceptionMessage = "Exception message";

        var dataResult =
            TryCatch(
                () => { throw new Exception(exceptionMessage); return new { Name = "test" }; },
                (ex) => ex
                ).ToResult();


        Assert.False(dataResult);
        Assert.Contains(exceptionMessage, dataResult.Message);
        Assert.Equal(ResultTypes.EXCEPTION, dataResult.ResultType);
    }

    [Fact(DisplayName = "Get typed exception Result with data")]
    public void ResultTypedExceptionFailTest()
    {

        var dataResult =
            TryCatch(
                () => { throw new ArgumentNullException(); return new { Name = "test" }; },
                (ex) => ex
                ).ToResult();


        Assert.False(dataResult);
        Assert.Contains(new ArgumentNullException().Message, dataResult.Message);
        Assert.Equal(ResultTypes.EXCEPTION, dataResult.ResultType);
    }

    [Fact(DisplayName = "Get success Result with data using Bind chain")]
    public void ResultBindingSuccessTest()
    {

        var dataResult1 =
            TryCatch(
                () => 2,
                (ex) => ex
                ).ToResult();

        var dataResult2 =
            dataResult1.Bind(x =>
                TryCatch(
                        () => "TEST" + x.ToString(),
                        (ex) => ex
                    ).ToResult());

        Assert.True(dataResult2);
        Assert.True(dataResult2.HasData);
        Assert.NotNull(dataResult2.Data);
        Assert.Equal("TEST2", dataResult2.Data);
    }

    [Fact(DisplayName = "Get exception Result with data using Bind chain")]
    public void ResultBindingExceptionFailTest()
    {
        const string exceptionMessage = "Exception message";

        var dataResult1 =
            TryCatch(
                () => { throw new Exception(exceptionMessage); return 2; },
                (ex) => ex
                ).ToResult();

        var dataResult2 =
            dataResult1.Bind(x =>
                TryCatch(
                    () => "TEST" + x.ToString(),
                    (ex) => ex
                    ).ToResult());

        Assert.False(dataResult2);
        Assert.Contains(exceptionMessage, dataResult2.Message);
        Assert.Equal(ResultTypes.EXCEPTION, dataResult2.ResultType);
    }

    [Fact(DisplayName = "Get success Result with data using longer Bind chain")]
    public void ResultBinding4LevelSuccessTest()
    {

        var finalResult =
            TryCatch(
                () => 1,
                (ex) => ex
                ).ToResult()
            .Bind(x =>
                TryCatch(
                    () => x.ToString() + "2",
                    (ex) => ex
                    ).ToResult())
            .Bind(x =>
                TryCatch(
                    () => x.ToString() + "3",
                    (ex) => ex
                    ).ToResult())
            .Bind(x =>
                TryCatch(
                    () => x.ToString() + "4",
                    (ex) => ex
                    ).ToResult()
            );


        Assert.True(finalResult);
        Assert.True(finalResult.HasData);
        Assert.Equal("1234", finalResult.Data);
    }

    [Fact(DisplayName = "Get exception Result with data using longer Bind chain")]
    public void ResultBinding4LevelFailTest()
    {
        const string exceptionMessage = "Exception at level 3";

        // Fails at level 3
        var finalResult =
            TryCatch(
                () => 1,
                (ex) => ex
                ).ToResult()
            .Bind(x =>
                TryCatch(
                    () => x.ToString() + "2",
                    (ex) => ex
                    ).ToResult())
            .Bind(x =>
                TryCatch(
                    () => { throw new Exception(exceptionMessage); return "3"; },
                    (ex) => ex
                    ).ToResult())
            .Bind(x =>
                TryCatch(
                    () => x.ToString() + "4",
                    (ex) => ex
                    ).ToResult()
            );

        Assert.False(finalResult.IsSuccess);
        Assert.False(finalResult.HasData);
        Assert.Contains(exceptionMessage, finalResult.Message);
        Assert.Equal(ResultTypes.EXCEPTION, finalResult.ResultType);
    }

    [Fact(DisplayName = "Get success Result with data on async Map")]
    public async void ResultSuccessAsyncMapTest()
    {
        var result =
            await TryCatch(
                async () => { await Task.Delay(100); return 1; },
                (ex) => ex)
            .ToResult()
            .Map((int _) => _.ToString());

        Assert.True(result);
        Assert.True(result.HasData);
        Assert.Equal("1", result.Data);
    }

    [Fact(DisplayName = "Get exception Result with data on async Map")]
    public async void ResultExceptionAsyncMapTest()
    {
        const string exceptionMessage = "Exception occured";
        var result =
            await TryCatch(
                async () => { await Task.Delay(100); throw new Exception(exceptionMessage); return 1; },
                (ex) => ex
                ).ToResult()
                .Map(_ => _ + 1);

        Assert.False(result);
        Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
    }

    [Fact(DisplayName = "Get success Result with data on async Bind chain")]
    public async void ResultSuccessBindAsyncTest()
    {
        var result =
            await GetSuccessResultAsync()
                .Bind(_ => GetSuccessResultAsync2())
                .Bind(_ => GetSuccessResultAsync());

        Assert.True(result);
        Assert.True(result.HasData);
        Assert.Equal(new PersonStruct { FirstName = "John", LastName = "Doe" }, result.Data);
    }

    [Fact(DisplayName = "Get exception Result with data on async Bind chain")]
    public async void ResultFailBindAsyncTest()
    {
        string exceptionMessage = "EXCEPTION MESSAGE";
        var result =
            await GetSuccessResultAsync()
                .Bind(_ => GetSuccessResultAsync2())
                .Bind(_ => GetExceptionFailAsync(exceptionMessage))
                .Bind(_ => GetSuccessResultAsync());

        Assert.False(result);
        Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
        Assert.Contains(exceptionMessage, result.Message);
    }

    [Fact(DisplayName = "Get success Result with data on Fish chain")]
    public void ResultSuccessFishTest()
    {

        var composition =
            ((Func<bool, Result<PersonStruct>>)GetResult)
                .Fish(_ => GetSuccessResult())
                .Fish(_ => GetSuccessResult())
                .Fish(_ => GetSuccessResult());

        var result = composition(false);

        Assert.True(result);
        Assert.True(result.HasData);
        Assert.Equal(new PersonStruct { FirstName = "John", LastName = "Doe" }, result.Data);
    }

    [Fact(DisplayName = "Get exception Result with data on Fish chain")]
    public void ResultExceptionFishTest()
    {
        string exceptionMessage = "EXCEPTION MESSAGE";

        var composition =
            ((Func<bool, Result<PersonStruct>>)GetResult)
                .Fish(_ => GetSuccessResult())
                .Fish(_ => GetExceptionFail(exceptionMessage))
                .Fish(_ => GetSuccessResult());

        var result = composition(false);

        Assert.False(result);
        Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
        Assert.Contains(exceptionMessage, result.Message);
    }

    [Fact(DisplayName = "Get success Result with data on async Fish chain")]
    public async void ResultSuccessFishAsyncTest()
    {

        var composition =
            ((Func<bool, Task<Result<PersonStruct>>>)GetResultAsync)
                .Fish(_ => GetSuccessResultAsync())
                .Fish(_ => GetSuccessResultAsync2())
                .Fish(_ => GetSuccessResultAsync());

        var result = await composition(false);

        Assert.True(result);
        Assert.True(result.HasData);
        Assert.Equal(new PersonStruct { FirstName = "John", LastName = "Doe" }, result.Data);
    }

    [Fact(DisplayName = "Get exception Result with data on async Fish chain")]
    public async void ResultFailFishAsyncTest()
    {
        string exceptionMessage = "EXCEPTION MESSAGE";

        var composition =
            ((Func<bool, Task<Result<PersonStruct>>>)GetResultAsync)
                .Fish(_ => GetSuccessResultAsync())
                .Fish(_ => GetExceptionFailAsync(exceptionMessage))
                .Fish(_ => GetSuccessResultAsync());

        var result = await composition(false);


        Assert.False(result);
        Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
        Assert.Contains(exceptionMessage, result.Message);
    }

    [Fact(DisplayName = "Get success on async AsResult with data")]
    public async void AsResultSuccessAsyncTest()
    {
        var person = new PersonStruct { FirstName = "John", LastName = "Smith" };
        var result = await Results.AsResult(async () => { await Task.Delay(0); return person; });

        Assert.True(result);
        Assert.True(result.HasData);
        Assert.Equal(person, result.Data);
    }

    [Fact(DisplayName = "Get exception on async AsResult with data")]
    public async void AsResultExceptionAsyncTest()
    {
        var person = new PersonStruct { FirstName = "John", LastName = "Smith" };
        var exceptionMessage = "test";
        var result = await Results.AsResult(async () => { await Task.Delay(0); throw new Exception(exceptionMessage) ; return person; });

        Assert.False(result);
        Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
        Assert.Contains(exceptionMessage, result.Message);
    }

    [Fact(DisplayName = "Get success on AsResult with data")]
    public void AsResultSuccessTest()
    {
        var person = new PersonStruct { FirstName = "John", LastName = "Smith" };
        var result = Results.AsResult(() =>  person);

        Assert.True(result);
        Assert.True(result.HasData);
        Assert.Equal(person, result.Data);
    }

    [Fact(DisplayName = "Get exception on AsResult with data")]
    public void AsResultExceptionTest()
    {
        var person = new PersonStruct { FirstName = "John", LastName = "Smith" };
        var exceptionMessage = "test";
        var result = Results.AsResult(() => { throw new Exception(exceptionMessage); return person; });

        Assert.False(result);
        Assert.Equal(ResultTypes.EXCEPTION, result.ResultType);
        Assert.Contains(exceptionMessage, result.Message);
    }

    #region HELPER METHODS

    private Result<PersonStruct> GetResult(bool throwsException)
        => TryCatch(
                () => { if (throwsException) throw new Exception(); return new PersonStruct { FirstName = "John", LastName = "Doe" }; },
                (ex) => ex
                ).ToResult();

    private async Task<Result<PersonStruct>> GetResultAsync(bool throwsException)
        => await TryCatch(
                async () => { if (throwsException) throw new Exception(); return await Task.Run(() => new PersonStruct { FirstName = "John", LastName = "Doe" }); },
                (ex) => ex
                ).ToResult();

    private Result<PersonStruct> GetSuccessResult()
        => TryCatch(
                () => new PersonStruct { FirstName = "John", LastName = "Doe" },
                (ex) => ex
                ).ToResult();

    private Result<PersonStruct> GetSuccessResult2()
        => TryCatch(
                () => new PersonStruct { FirstName = "John", LastName = "Smith" },
                (ex) => ex
                ).ToResult();

    private Result<PersonStruct> GetExceptionFail(string exceptionMessage)
        => TryCatch(
                () => { throw new Exception(exceptionMessage); return new PersonStruct { FirstName = "John", LastName = "Doe" }; },
                (ex) => ex
                ).ToResult();

    private async Task<Result<PersonStruct>> GetSuccessResultAsync()
        => await TryCatch(
                async () => await Task.Run(() => new PersonStruct { FirstName = "John", LastName = "Doe" }),
                (ex) => ex
                ).ToResult();

    private async Task<Result<PersonStruct>> GetSuccessResultAsync2()
        => await TryCatch(
                async () => await Task.Run(() => new PersonStruct { FirstName = "John", LastName = "Smith" }),
                (ex) => ex
                ).ToResult();

    private async Task<Result<PersonStruct>> GetExceptionFailAsync(string exceptionMessage)
        => await TryCatch(
                async () => { throw new Exception(exceptionMessage); return await Task.Run(() => new PersonStruct { FirstName = "John", LastName = "Doe" }); },
                (ex) => ex
                ).ToResult();

    private struct PersonStruct
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    #endregion
}
