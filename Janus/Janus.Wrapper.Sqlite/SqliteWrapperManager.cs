using Janus.Communication.Nodes.Implementations;
using Janus.Logging;
using Janus.Wrapper.Persistence;
using Janus.Wrapper.Sqlite.LocalCommanding;
using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Sqlite.LocalQuerying;

namespace Janus.Wrapper.Sqlite;
public sealed class SqliteWrapperManager
    : WrapperManager<SqliteQuery, SqliteDelete, SqliteInsert, SqliteUpdate, string, string, string, SqliteTabularData, string, string>
{
    private readonly ILogger<SqliteWrapperManager>? _logger;
    public SqliteWrapperManager(WrapperCommunicationNode communicationNode, SqliteWrapperQueryManager queryManager, SqliteWrapperCommandManager commandManager, SqliteWrapperSchemaManager schemaManager, WrapperPersistenceProvider persistenceProvider, SqliteWrapperOptions wrapperOptions, ILogger? logger = null) : base(communicationNode, queryManager, commandManager, schemaManager, persistenceProvider, wrapperOptions, logger)
    {
        _logger = logger?.ResolveLogger<SqliteWrapperManager>();
    }
}
