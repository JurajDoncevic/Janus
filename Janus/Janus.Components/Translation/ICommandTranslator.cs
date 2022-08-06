namespace Janus.Components.Translation;
public interface ICommandTranslator
    <TDeleteCommandSource, TInsertCommandSource, TUpdateCommandSource, TSelectionSource, TMutationSource, TInstantiationSource,
    TDeleteCommandDestination, TInsertCommandDestination, TUpdateCommandDestination, TSelectionDestination, TMutationDestination, TInstantiationDestination>
{
    Result<TDeleteCommandDestination> TranslateDelete(TDeleteCommandSource delete);
    Result<TInsertCommandDestination> TranslateInsert(TInsertCommandSource insert);
    Result<TUpdateCommandDestination> TranslateUpdate(TUpdateCommandSource update);
    Result<TSelectionDestination> TranslateSelection(Option<TSelectionSource> selection);
    Result<TMutationDestination> TranslateMutation(Option<TMutationSource> mutation);
    Result<TInstantiationDestination> TranslateInstantiation(Option<TInstantiationSource> instantiation);
}
