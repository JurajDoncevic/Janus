using Janus.Commons.QueryModels;
using Janus.Components.Translation;
using Janus.Wrapper.LocalQuerying;

namespace Janus.Wrapper.Translation;
public interface IWrapperQueryTranslator<TLocalQuery, TSelection, TJoinning, TProjection>
    : IQueryTranslator<Query, Selection, Joining, Projection, TLocalQuery, TSelection, TJoinning, TProjection>
    where TLocalQuery : LocalQuery<TSelection, TJoinning, TProjection>
{
}
