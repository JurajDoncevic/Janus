using Janus.Logging;
using Janus.Wrapper.Sqlite.LocalCommanding;
using Janus.Wrapper.Sqlite.Translation;

namespace Janus.Wrapper.Sqlite;
public sealed class SqliteWrapperCommandManager : WrapperCommandManager<SqliteDelete, SqliteInsert, SqliteUpdate, string, string, string>
{
    private readonly ILogger<SqliteWrapperCommandManager>? _logger;
    public SqliteWrapperCommandManager(SqliteCommandTranslator commandTranslator, SqliteCommandExecutor commandExecutor, ILogger? logger = null)
        : base(commandTranslator, commandExecutor, logger)
    {
        _logger = logger?.ResolveLogger<SqliteWrapperCommandManager>();
    }
}
