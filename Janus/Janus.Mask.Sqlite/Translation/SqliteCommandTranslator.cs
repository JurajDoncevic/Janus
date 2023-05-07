using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mask.Sqlite.MaskedCommandModel;
using Janus.Mask.Translation;

namespace Janus.Mask.Sqlite.Translation;
public sealed class SqliteCommandTranslator : IMaskCommandTranslator<SqliteDelete, SqliteInsert, SqliteUpdate, Unit, Unit, Unit>
{
    public Result<DeleteCommand> TranslateDelete(SqliteDelete delete)
    {
        return Results.OnException<DeleteCommand>(new NotImplementedException());
    }

    public Result<InsertCommand> TranslateInsert(SqliteInsert insert)
    {
        return Results.OnException<InsertCommand>(new NotImplementedException());
    }

    public Result<Instantiation> TranslateInstantiation(Option<Unit> instantiation)
    {
        return Results.OnException<Instantiation>(new NotImplementedException());
    }

    public Result<Mutation> TranslateMutation(Option<Unit> mutation)
    {
        return Results.OnException<Mutation>(new NotImplementedException());
    }

    public Result<SelectionExpression> TranslateSelection(Option<Unit> selection)
    {
        return Results.OnException<SelectionExpression>(new NotImplementedException());
    }

    public Result<UpdateCommand> TranslateUpdate(SqliteUpdate update)
    {
        return Results.OnException<UpdateCommand>(new NotImplementedException());
    }
}
