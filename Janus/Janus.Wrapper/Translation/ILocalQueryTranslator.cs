using FunctionalExtensions.Base.Results;
using Janus.Commons.QueryModels;
using Janus.Wrapper.LocalQuerying;
using Janus.Components.Translation;

namespace Janus.Wrapper.Translation;
public interface ILocalQueryTranslator<TLocalQuery, TSelection, TJoinning, TProjection>
    : IQueryTranslator<Query, Selection, Joining, Projection, TLocalQuery, TSelection, TJoinning, TProjection>
    where TLocalQuery : LocalQuery<TSelection, TJoinning, TProjection>
{
}
