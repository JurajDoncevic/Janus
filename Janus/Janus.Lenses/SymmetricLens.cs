namespace Janus.Lenses;

/// <summary>
/// Describes a SIMPLE symmetric lens
/// </summary>
/// <typeparam name="TLeft">Left type</typeparam>
/// <typeparam name="TRight">Right type</typeparam>
public abstract class SymmetricLens<TLeft, TRight>
{
    /// <summary>
    /// putL : Y -> X? -> X
    /// </summary>
    public Func<TRight, TLeft?, Result<TLeft>> PutLeft => _PutLeft;
    /// <summary>
    /// putR : X -> Y? -> Y
    /// </summary>
    public Func<TLeft, TRight?, Result<TRight>> PutRight => _PutRight;
    /// <summary>
    /// createR : X? -> Y
    /// </summary>
    public Func<TLeft?, Result<TRight>> CreateRight => _CreateRight;
    /// <summary>
    /// createL : Y? -> X
    /// </summary>
    public Func<TRight?, Result<TLeft>> CreateLeft => _CreateLeft;

    /// <summary>
    /// Constructor
    /// </summary>
    protected SymmetricLens() { }

    protected abstract Result<TLeft> _PutLeft(TRight right, TLeft? left);
    protected abstract Result<TRight> _PutRight(TLeft left, TRight? right);
    protected abstract Result<TRight> _CreateRight(TLeft? left);
    protected abstract Result<TLeft> _CreateLeft(TRight? right);
}
