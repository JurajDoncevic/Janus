namespace Janus.Lenses;
public interface ICreatingLeftLens<TLeft>
{
    public TLeft CreateLeft();
}

public interface ICreatingLeftSpecsLens<TLeft, TLeftSpecs>
    where TLeftSpecs : class
{
    public TLeft CreateLeft(TLeftSpecs leftSpecs);
}
