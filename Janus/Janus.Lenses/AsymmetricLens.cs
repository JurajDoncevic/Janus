namespace Janus.Lenses;
public abstract class AsymmetricLens<TSource, TView>
{

    protected AsymmetricLens()
    {
    }

    public abstract Func<TView, TSource?, TSource> Put { get; }

    public abstract Func<TSource, TView> Get { get; }

    /// <summary>
    /// For a given source returns a view
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public TView CallGet(TSource source) => Get(source);

    /// <summary>
    /// For a given updated view and original source returns an updated source
    /// </summary>
    /// <param name="updatedView"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public TSource CallPut(TView updatedView, TSource? source) => Put(updatedView, source);

    public override bool Equals(object? obj)
    {
        return obj is not null && 
               obj is AsymmetricLens<TSource, TView> lens &&
               Get.Equals(lens.Get) &&
               Put.Equals(lens.Put);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Get, Put);
    }

    public static bool operator ==(AsymmetricLens<TSource, TView> left, AsymmetricLens<TSource, TView> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AsymmetricLens<TSource, TView> left, AsymmetricLens<TSource, TView> right)
    {
        return !(left == right);
    }
}


public static class LensExtensions
{
    //public static Lens<TSource, RView> Compose<TSource, TView, RView>(this Lens<TSource, TView> lens1, Lens<TView, RView> lens2)
    //    => Lens<TSource, TView>.Create<TSource, RView>(
    //        s => lens2.Get(lens1.Get(s)),
    //        (uv, s) => lens1.Put(lens2.Put(uv, lens1.Get(s)), s)
    //        );
}