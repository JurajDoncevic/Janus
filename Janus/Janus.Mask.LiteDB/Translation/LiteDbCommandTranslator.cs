using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mask.LiteDB.MaskedCommandModel;
using Janus.Mask.Translation;

namespace Janus.Mask.LiteDB.Translation;
public class LiteDbCommandTranslator : IMaskCommandTranslator<LiteDbDelete, LiteDbInsert, LiteDbUpdate, Unit, Unit, Unit>
{
    public Result<DeleteCommand> TranslateDelete(LiteDbDelete delete)
    {
        return Results.OnException<DeleteCommand>(new NotImplementedException());
    }

    public Result<InsertCommand> TranslateInsert(LiteDbInsert insert)
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

    public Result<UpdateCommand> TranslateUpdate(LiteDbUpdate update)
    {
        return Results.OnException<UpdateCommand>(new NotImplementedException());
    }
}
