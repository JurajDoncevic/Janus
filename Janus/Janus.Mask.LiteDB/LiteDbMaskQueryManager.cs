using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.LiteDB.MaskedDataModel;
using Janus.Mask.LiteDB.MaskedQueryModel;
using Janus.Mask.LiteDB.MaskedSchemaModel;
using Janus.Mask.LiteDB.Translation;
using LiteDB;

namespace Janus.Mask.LiteDB;
public sealed class LiteDbMaskQueryManager : MaskQueryManager<LiteDbQuery, TableauId, Unit, Unit, Unit, Database, LiteDbData, BsonDocument>
{
    private readonly LiteDbQueryTranslator _queryTranslator;
    public LiteDbMaskQueryManager(MaskCommunicationNode communicationNode, LiteDbMaskSchemaManager schemaManager, LiteDbQueryTranslator queryTranslator, ILogger? logger = null) : base(communicationNode, schemaManager, queryTranslator, logger)
    {
        _queryTranslator = queryTranslator;
    }

    public override async Task<Result<LiteDbData>> RunQuery(LiteDbQuery query)
    {
        return Results.OnException<LiteDbData>(new NotImplementedException());
    }
}

