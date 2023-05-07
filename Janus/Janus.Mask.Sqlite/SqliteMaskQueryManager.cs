using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.Sqlite.MaskedDataModel;
using Janus.Mask.Sqlite.MaskedQueryModel;
using Janus.Mask.Sqlite.MaskedSchemaModel;
using Janus.Mask.Sqlite.Translation;
using Janus.Mask.Translation;

namespace Janus.Mask.Sqlite;
public sealed class SqliteMaskQueryManager : MaskQueryManager<SqliteQuery, TableauId, Unit, Unit, Unit, Database, SqliteTabularData, SqliteDataRow>
{
    public SqliteMaskQueryManager(MaskCommunicationNode communicationNode, SqliteMaskSchemaManager schemaManager, SqliteQueryTranslator queryTranslator, ILogger? logger = null) : base(communicationNode, schemaManager, queryTranslator, logger)
    {
    }

    public override async Task<Result<SqliteTabularData>> RunQuery(SqliteQuery query)
    {
        return await Task.FromResult(Results.OnException<SqliteTabularData>(new NotImplementedException()));
    }
}
