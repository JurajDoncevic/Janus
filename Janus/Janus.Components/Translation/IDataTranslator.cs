namespace Janus.Components.Translation;

/// <summary>
/// Describes a translator for data
/// </summary>
/// <typeparam name="TSource">Source data type</typeparam>
/// <typeparam name="TDestination">Destination data type</typeparam>
public interface IDataTranslator<TSource, TDestination>
{
    /// <summary>
    /// Translates data from the source type to the destination type
    /// </summary>
    /// <param name="source">Source type data</param>
    /// <returns>Destination type data</returns>
    TDestination Translate(TSource source);
}
