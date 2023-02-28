using Janus.Commons.QueryModels;
using Janus.Commons.SelectionExpressions;
using Janus.Components.Translation;
using Janus.Mask.MaskedQueryModel;

namespace Janus.Mask.Translation;
public interface IMaskQueryTranslator<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoinning, TMaskedProjection>
    : IQueryTranslator<TMaskedQuery, TMaskedSelection, TMaskedJoinning, TMaskedProjection, Query, SelectionExpression, Joining, Projection>
    where TMaskedQuery : MaskedQuery<TMaskedStartingWith, TMaskedSelection, TMaskedJoinning, TMaskedProjection>
{
}
