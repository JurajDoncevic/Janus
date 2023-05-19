namespace Janus.Lenses.Implementations;
public sealed class IntStringLens : SymmetricLens<int, string>
{
    protected override Result<int> _CreateLeft(string? right)
        => Results.AsResult(() => Convert.ToInt32(right));

    protected override Result<string> _CreateRight(int left)
        => Results.AsResult(() => left.ToString());

    protected override Result<int> _PutLeft(string right, int left)
        => Results.AsResult(() => Convert.ToInt32(right));

    protected override Result<string> _PutRight(int left, string? right)
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