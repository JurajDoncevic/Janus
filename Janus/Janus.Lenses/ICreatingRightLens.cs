namespace Janus.Lenses;

/// <summary>
/// Describes a Lens that can create a default right (view) object
/// </summary>
/// <typeparam name="TRight">Right/View type</typeparam>
public interface ICreatingRightLens<TRight>
{
    /// <summary>
    /// Creates a default right (view) object of a lens 
    /// </summary>
    /// <returns>Default right (view) object</returns>
    public TRight CreateRight();
}

/// <summary>
/// Describes a Lens that can create a right (view) object via some specifications
/// </summary>
/// <typeparam name="TRight">Right/View type</typeparam>
/// <typeparam name="TRightSpecs">Construction specification type</typeparam>
public interface ICreatingRightSpecsLens<TRight, TRightSpecs>
{
    /// <summary>
    /// Creates a right (view) object of a lens via some specifications
    /// </summary>
    /// <param name="rightSpecs">Construction specification</param>
    /// <returns>Right (view) object</returns>
    public TRight CreateRight(Option<TRightSpecs> rightSpecs);
}
