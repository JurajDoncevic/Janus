namespace Janus.Components.Translation;

/// <summary>
/// Describes a command translator
/// </summary>
/// <typeparam name="TDeleteCommandSource">Source delete command type</typeparam>
/// <typeparam name="TInsertCommandSource">Source insert command type</typeparam>
/// <typeparam name="TUpdateCommandSource">Source update command type</typeparam>
/// <typeparam name="TSelectionSource">Source selection clause type</typeparam>
/// <typeparam name="TMutationSource">Source mutation clause type</typeparam>
/// <typeparam name="TInstantiationSource">Source instantiation clause type</typeparam>
/// <typeparam name="TDeleteCommandDestination">Destination delete command type</typeparam>
/// <typeparam name="TInsertCommandDestination">Destination insert command type</typeparam>
/// <typeparam name="TUpdateCommandDestination">Destination update command type</typeparam>
/// <typeparam name="TSelectionDestination">Destination selection clause type</typeparam>
/// <typeparam name="TMutationDestination">Destination mutation clause type</typeparam>
/// <typeparam name="TInstantiationDestination">Destination instantiation clause type</typeparam>
public interface ICommandTranslator
    <TDeleteCommandSource, TInsertCommandSource, TUpdateCommandSource, TSelectionSource, TMutationSource, TInstantiationSource,
    TDeleteCommandDestination, TInsertCommandDestination, TUpdateCommandDestination, TSelectionDestination, TMutationDestination, TInstantiationDestination>
{
    /// <summary>
    /// Translates a source delete command to a destination type
    /// </summary>
    /// <param name="delete">Source delete command</param>
    /// <returns>Destination delete command</returns>
    Result<TDeleteCommandDestination> TranslateDelete(TDeleteCommandSource delete);
    /// <summary>
    /// Translates a source insert command to a destination type
    /// </summary>
    /// <param name="insert">Source insert command</param>
    /// <returns>Destination insert command</returns>
    Result<TInsertCommandDestination> TranslateInsert(TInsertCommandSource insert);
    /// <summary>
    /// Translates a source update command to a destination type
    /// </summary>
    /// <param name="update">Source update command</param>
    /// <returns>Destination update command</returns>
    Result<TUpdateCommandDestination> TranslateUpdate(TUpdateCommandSource update);
    /// <summary>
    /// Translates a source selection clause to a destination type
    /// </summary>
    /// <param name="selection">Source selection clause</param>
    /// <returns>Destination selection clause</returns>
    Result<TSelectionDestination> TranslateSelection(Option<TSelectionSource> selection);
    /// <summary>
    /// Translates a source mutation clause to a destination type
    /// </summary>
    /// <param name="mutation">Source mutation clause</param>
    /// <returns>Destination mutation clause</returns>
    Result<TMutationDestination> TranslateMutation(Option<TMutationSource> mutation);
    /// <summary>
    /// Translates a source instantiation clause to a destination type
    /// </summary>
    /// <param name="instantiation">Source instantiation clause</param>
    /// <returns>Destination instantiation clause</returns>
    Result<TInstantiationDestination> TranslateInstantiation(Option<TInstantiationSource> instantiation);
}
