using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.LiteDB.MaskedCommandModel;
using Janus.Mask.LiteDB.MaskedSchemaModel;
using Janus.Mask.LiteDB.Translation;

namespace Janus.Mask.LiteDB;
public class LiteDbMaskCommandManager : MaskCommandManager<LiteDbDelete, LiteDbInsert, LiteDbUpdate, Unit, Unit, Unit, Database>
{
    public LiteDbMaskCommandManager(MaskCommunicationNode communicationNode, LiteDbMaskSchemaManager schemaManager, LiteDbCommandTranslator commandTranslator, ILogger? logger = null) : base(communicationNode, schemaManager, commandTranslator, logger)
    {
    }

    public override async Task<Result> RunCommand(LiteDbDelete command)
    {
        return Results.OnException(new NotImplementedException());
    }

    public override async Task<Result> RunCommand(LiteDbInsert command)
    {
        return Results.OnException(new NotImplementedException());
    }

    public override async Task<Result> RunCommand(LiteDbUpdate command)
    {
        return Results.OnException(new NotImplementedException());
    }
}
