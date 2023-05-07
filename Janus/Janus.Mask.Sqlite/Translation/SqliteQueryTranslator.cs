using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mask.Sqlite.MaskedQueryModel;
using Janus.Mask.Translation;

namespace Janus.Mask.Sqlite.Translation;
public sealed class SqliteQueryTranslator : IMaskQueryTranslator<SqliteQuery, TableauId, Unit, Unit, Unit>
{
    public Result<Query> Translate(SqliteQuery query)
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
