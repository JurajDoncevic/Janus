namespace Janus.Lenses.Implementations;
public sealed class IntStringLens : Lens<int, string>
{
    internal IntStringLens() : base()
    {
    }

    public override Func<string, int, int> Put => 
        (view, source) => view is not null &&
                          int.TryParse(view, out int updatedSource)
                            ? updatedSource
                            : source;

    public override Func<int, string> Get => 
        (source) => source.ToString();
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