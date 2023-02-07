using Janus.Commons.SchemaModels;

namespace Janus.Components.Translation;
/// <summary>
/// Describes a query translator
/// </summary>
/// <typeparam name="TQuerySource">Source query type</typeparam>
/// <typeparam name="TSelectionSource">Source selection clause type</typeparam>
/// <typeparam name="TJoiningSource">Source joining clause type</typeparam>
/// <typeparam name="TProjectionSource">Source projection clause type</typeparam>
/// <typeparam name="TQueryDestination">Destination query type</typeparam>
/// <typeparam name="TSelectionDestination">Destination selection clause type</typeparam>
/// <typeparam name="TJoiningDestination">Destination joining clause type</typeparam>
/// <typeparam name="TProjectionDestination">Destination projection clause type</typeparam>
public interface IQueryTranslator<TQuerySource, TSelectionSource, TJoiningSource, TProjectionSource, TQueryDestination, TSelectionDestination, TJoiningDestination, TProjectionDestination>
{
    /// <summary>
    /// Translates a source query to a destination type query
    /// </summary>
    /// <param name="query">Source query</param>
    /// <returns>Destination query</returns>
    Result<TQueryDestination> Translate(TQuerySource query);
    /// <summary>
    /// Translates a source selection clause to a destination type selection clause
    /// </summary>
    /// <param name="selection">Source selection clause</param>
    /// <returns>Destination selection clause</returns>
    Result<TSelectionDestination> TranslateSelection(Option<TSelectionSource> selection);
    /// <summary>
    /// Translates a source joining clause to a destination type joining clause
    /// </summary>
    /// <param name="joining">Source joining clause</param>
    /// <param name="startingWith">Initial tableau identifier</param>
    /// <returns>Destination joining clause</returns>
    Result<TJoiningDestination> TranslateJoining(Option<TJoiningSource> joining, TableauId? startingWith = null);
    /// <summary>
    /// Translates a source projection clause to a destination type projection clause
    /// </summary>
    /// <param name="projection">Source projection clause</param>
    /// <returns>Destination projection clause</returns>
    Result<TProjectionDestination> TranslateProjection(Option<TProjectionSource> projection);
}
