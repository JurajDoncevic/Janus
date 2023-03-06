namespace Janus.Lenses;
public interface ICreatingRightLens<TRight>
{
    public TRight CreateRight();
}

public interface ICreatingRightSpecsLens<TRight, TRightSpecs>
    where TRightSpecs : class
{
    public TRight CreateRight(TRightSpecs rightSpecs);
}
