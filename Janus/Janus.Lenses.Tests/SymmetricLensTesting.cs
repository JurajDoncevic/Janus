using Janus.Base;
using Janus.Base.Resulting;

namespace Janus.Lenses.Tests;

/// <summary>
/// Test declarations for SIMPLE symmetric lenses
/// </summary>
public abstract class SymmetricLensTesting
{
    /// <summary>
    /// Should confirm that putL x (createR x) = x
    /// </summary>
    [Fact(DisplayName = $"CREATE_PUT_RL rule test")]
    public abstract void CreatePutRLTest();

    /// <summary>
    /// Should confirm that putR y (createL y) = y
    /// </summary>
    [Fact(DisplayName = $"CREATE_PUT_LR rule test")]
    public abstract void CreatePutLRTest();

    /// <summary>
    /// Should confirm that putL x (putR y x) = x
    /// </summary>
    [Fact(DisplayName = $"PUT_RL rule test")]
    public abstract void PutRLTest();

    /// <summary>
    /// Should confirm that putR y (putL x y) = y
    /// </summary>
    [Fact(DisplayName = $"PUT_LR rule test")]
    public abstract void PutLRTest();
}


public abstract class SymmetricLensTestingFramework<TLeft, TRight> : SymmetricLensTesting
{
    protected abstract TLeft _x { get; }
    protected abstract TRight _y { get; }

    protected abstract SymmetricLens<TLeft, TRight> _lens { get; }

    public override void CreatePutLRTest()
    {
        var result =
        _lens.CreateLeft(Option<TRight>.Some(_y))
            .Bind(x => _lens.PutRight(x, Option<TRight>.Some(_y)));

        Assert.True(result);
        Assert.Equal(_y, result.Data);
    }

    public override void CreatePutRLTest()
    {
        var result =
        _lens.CreateRight(Option<TLeft>.Some(_x))
            .Bind(y => _lens.PutLeft(y, Option<TLeft>.Some(_x)));

        Assert.True(result);
        Assert.Equal(_x, result.Data);
    }

    public override void PutLRTest()
    {
        var result =
        _lens.PutLeft(_y, Option<TLeft>.Some(_x))
            .Bind(x => _lens.PutRight(x, Option<TRight>.Some(_y)));

        Assert.True(result);
        Assert.Equal(_y, result.Data);
    }

    public override void PutRLTest()
    {
        var result =
        _lens.PutRight(_x, Option<TRight>.Some(_y))
            .Bind(y => _lens.PutLeft(y, Option<TLeft>.Some(_x)));

        Assert.True(result);
        Assert.Equal(_x, result.Data);
    }

}