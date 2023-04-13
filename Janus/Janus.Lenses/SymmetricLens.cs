namespace Janus.Lenses;
public abstract class SymmetricLens<TLeft, TRight> 
    : ICreatingLeftSpecsLens<TLeft, TRight>, 
      ICreatingRightSpecsLens<TRight, TLeft>
{
    /// <summary>
    /// Puts a Left object to a Right object
    /// </summary>
    /// <param name="left">Left object</param>
    /// <param name="right">Source right object</param>
    /// <returns>Right object</returns>
    public TRight CallPutRight(TLeft left, TRight? right) => PutRight(left, right);

    /// <summary>
    /// Puts a Right object to a Left Object
    /// </summary>
    /// <param name="right">Right object</param>
    /// <param name="left">Source left object</param>
    /// <returns>Left object</returns>
    public TLeft CallPutLeft(TRight right, TLeft? left) => PutLeft(right, left);

    public abstract TLeft CreateLeft(Option<TRight> right);

    public abstract TRight CreateRight(Option<TLeft> left);


    protected SymmetricLens()
    {
    }

    public abstract Func<TLeft, TRight?, TRight> PutRight { get; }

    public abstract Func<TRight, TLeft?, TLeft> PutLeft { get; }


    public override bool Equals(object? obj)
    {
        return obj is not null &&
               obj is SymmetricLens<TLeft, TRight> lens &&
               PutLeft.Equals(lens.PutLeft) &&
               PutRight.Equals(lens.PutRight);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PutLeft, PutRight);
    }

    public static bool operator ==(SymmetricLens<TLeft, TRight> left, SymmetricLens<TLeft, TRight> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SymmetricLens<TLeft, TRight> left, SymmetricLens<TLeft, TRight> right)
    {
        return !(left == right);
    }
}
