using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Components.Translation;
using Janus.Logging;
using Janus.Mask.LiteDB.MaskedDataModel;
using Janus.Mask.LiteDB.MaskedQueryModel;
using Janus.Mask.LiteDB.MaskedSchemaModel;
using Janus.Mask.LiteDB.Translation;
using Janus.Mask.MaskedDataModel;
using Janus.Mask.Translation;
using LiteDB;

namespace Janus.Mask.LiteDB;
public class LiteDbMaskQueryManager : MaskQueryManager<LiteDbQuery, TableauId, Unit, Unit, Unit, Database>
{
    private readonly LiteDbQueryTranslator _queryTranslator;
    public LiteDbMaskQueryManager(MaskCommunicationNode communicationNode, LiteDbMaskSchemaManager schemaManager, LiteDbQueryTranslator queryTranslator, ILogger? logger = null) : base(communicationNode, schemaManager, queryTranslator, logger)
    {
        _queryTranslator = queryTranslator;
    }

    public override Task<Result<MaskedData<TDataItem>>> RunQuery<TDataItem>(LiteDbQuery query)
    {
        throw new NotImplementedException();
    }
}
