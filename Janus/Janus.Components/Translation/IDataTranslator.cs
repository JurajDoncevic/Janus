namespace Janus.Components.Translation;
public interface IDataTranslator<TSource, TDestination>
{
    TDestination Translate(TSource source);
}
