namespace Janus.Components.Translation;
public interface ICommandTranslator
    <TCommandSource, TSelectionSource, TMutationSource, TInstantiationSource,
    TCommandDestination, TSelectionDestination, TMutationDestination, TInstantiationDestination>
{
    Result<TCommandDestination> Translate(TCommandSource query);
    Result<TSelectionDestination> TranslateSelection(Option<TSelectionSource> selection);
    Result<TMutationDestination> TranslateJoin(Option<TMutationSource> mutation);
    Result<TInstantiationDestination> TranslateProjection(Option<TInstantiationSource> instantiation);
}
