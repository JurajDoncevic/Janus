using Janus.Commons.SchemaModels;

namespace Janus.Components.Translation;
public interface IQueryTranslator<TQuerySource, TSelectionSource, TJoiningSource, TProjectionSource, TQueryDestination, TSelectionDestination, TJoiningDestination, TProjectionDestination>
{
    Result<TQueryDestination> Translate(TQuerySource query);
    Result<TSelectionDestination> TranslateSelection(Option<TSelectionSource> selection);
    Result<TJoiningDestination> TranslateJoining(Option<TJoiningSource> joining, TableauId? startingWith = null);
    Result<TProjectionDestination> TranslateProjection(Option<TProjectionSource> projection);
}
