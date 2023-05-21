namespace Janus.Lenses.Implementations;
public sealed class IntStringLens : SymmetricLens<int, string>
{
    protected override Result<int> _CreateLeft(Option<string> right)
        => Results.AsResult(() => right.Match(r => Convert.ToInt32(r), () => default));

    protected override Result<string> _CreateRight(Option<int> left)
        => Results.AsResult(() => left.Match(l => l.ToString(), () => string.Empty));

    protected override Result<int> _PutLeft(string right, Option<int> left)
        => Results.AsResult(() => Convert.ToInt32(right));

    protected override Result<string> _PutRight(int left, Option<string> right)
        => Results.AsResult(() => left.ToString());
}

public static class IntStringLenses
{
    /// <summary>
    /// Constructs a IntStringLens
    /// </summary>
    /// <returns>IntStringLens instance</returns>
    public static IntStringLens Construct()
        => new IntStringLens();
}