using Janus.Base;
using Janus.Base.Resulting;
using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Mask.Sqlite.MaskedCommandModel;
using Janus.Mask.Sqlite.MaskedSchemaModel;
using Janus.Mask.Sqlite.Translation;
using Janus.Mask.Translation;

namespace Janus.Mask.Sqlite;
public sealed class SqliteMaskCommandManager : MaskCommandManager<SqliteDelete, SqliteInsert, SqliteUpdate, Unit, Unit, Unit, Database>
{
    public SqliteMaskCommandManager(MaskCommunicationNode communicationNode, SqliteMaskSchemaManager schemaManager, SqliteCommandTranslator commandTranslator, ILogger? logger = null) : base(communicationNode, schemaManager, commandTranslator, logger)
    {
    }

    public override async Task<Result> RunCommand(SqliteDelete command)
    {
        return await Task.FromResult(Results.OnException(new NotImplementedException()));
    }

    public override async Task<Result> RunCommand(SqliteInsert command)
    {
        return await Task.FromResult(Results.OnException(new NotImplementedException()));
    }

    public override async Task<Result> RunCommand(SqliteUpdate command)
    {
        return await Task.FromResult(Results.OnException(new NotImplementedException()));
    }
}
