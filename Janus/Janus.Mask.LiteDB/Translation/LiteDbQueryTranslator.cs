using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mask.LiteDB.MaskedQueryModel;
using Janus.Mask.Translation;

namespace Janus.Mask.LiteDB.Translation;
public class LiteDbQueryTranslator : IMaskQueryTranslator<LiteDbQuery, TableauId, Unit, Unit, Unit>
{
    public Result<Query> Translate(LiteDbQuery query)
    {
        return Results.OnException<Query>(new NotImplementedException());
    }

    public Result<Joining> TranslateJoining(Option<Unit> joining, TableauId? startingWith = null)
    {
        return Results.OnException<Joining>(new NotImplementedException());
    }

    public Result<Projection> TranslateProjection(Option<Unit> projection)
    {
        return Results.OnException<Projection>(new NotImplementedException());
    }

    public Result<SelectionExpression> TranslateSelection(Option<Unit> selection)
    {
        return Results.OnException<SelectionExpression>(new NotImplementedException());
    }
}
