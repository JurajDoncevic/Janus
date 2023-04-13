namespace Janus.Lenses;

/// <summary>
/// Describes a Lens that can create a default left (source) object
/// </summary>
/// <typeparam name="TLeft">Left/Source type</typeparam>
public interface ICreatingLeftLens<TLeft>
{
    /// <summary>
    /// Creates a default left (source) object of a lens 
    /// </summary>
    /// <returns>Default left (source) object</returns>
    public TLeft CreateLeft();
}

/// <summary>
/// Describes a Lens that can create a left (source) object via some specifications
/// </summary>
/// <typeparam name="TLeft">Left/Source type</typeparam>
/// <typeparam name="TLeftSpecs">Construction specification type</typeparam>
public interface ICreatingLeftSpecsLens<TLeft, TLeftSpecs>
    where TLeftSpecs : class
{
    /// <summary>
    /// Creates a left (source) object of a lens via some specifications
    /// </summary>
    /// <param name="leftSpecs">Construction specification</param>
    /// <returns>Left (source) object</returns>
    public TLeft CreateLeft(TLeftSpecs leftSpecs);
}
