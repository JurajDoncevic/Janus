using Janus.Commons.QueryModels;
using Janus.Commons.SelectionExpressions;
using Janus.Components.Translation;
using Janus.Mask.LocalQuerying;

namespace Janus.Mask.Translation;
public interface IMaskQueryTranslator<TLocalQuery, TStartingWith, TSelection, TJoinning, TProjection>
    : IQueryTranslator<TLocalQuery, TSelection, TJoinning, TProjection, Query, SelectionExpression, Joining, Projection>
    where TLocalQuery : LocalQuery<TStartingWith, TSelection, TJoinning, TProjection>
{
}
