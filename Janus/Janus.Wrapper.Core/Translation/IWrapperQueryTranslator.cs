using Janus.Commons.QueryModels;
using Janus.Components.Translation;

namespace Janus.Wrapper.Core.Translation;
public interface IWrapperQueryTranslator<TQueryDestination, TSelectionDestination, TJoiningDestination, TProjectionDestination> : IQueryTranslator<Query, Selection, Joining, Projection, TQueryDestination, TSelectionDestination, TJoiningDestination, TProjectionDestination>
{
}
