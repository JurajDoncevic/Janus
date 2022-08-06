namespace Janus.Components.Translation;
public interface ICommandTranslator
    <TDeleteCommandSource, TInsertCommandSource, TUpdateCommandSource, TSelectionSource, TMutationSource, TInstantiationSource,
    TDeleteCommandDestination, TInsertCommandDestination, TUpdateCommandDestination, TSelectionDestination, TMutationDestination, TInstantiationDestination>
{
    Result<TDeleteCommandDestination> TranslateDelete(TDeleteCommandSource query);
    Result<TInsertCommandDestination> TranslateInsert(TInsertCommandSource query);
    Result<TUpdateCommandDestination> TranslateUpdate(TUpdateCommandSource query);
    Result<TSelectionDestination> TranslateSelection(Option<TSelectionSource> selection);
    Result<TMutationDestination> TranslateMutation(Option<TMutationSource> mutation);
    Result<TInstantiationDestination> TranslateProjection(Option<TInstantiationSource> instantiation);
}
