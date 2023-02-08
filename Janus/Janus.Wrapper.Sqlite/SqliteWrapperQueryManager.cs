using Janus.Logging;
using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Sqlite.LocalQuerying;
using Janus.Wrapper.Sqlite.Translation;

namespace Janus.Wrapper.Sqlite;
public sealed class SqliteWrapperQueryManager : WrapperQueryManager<SqliteQuery, string, string, string, SqliteTabularData>
{
    private readonly ILogger<SqliteWrapperQueryManager>? _logger;
    public SqliteWrapperQueryManager(SqliteQueryTranslator queryTranslator, SqliteDataTranslator dataTranslator, SqliteQueryExecutor queryExecutor, ILogger? logger = null) : base(queryTranslator, dataTranslator, queryExecutor, logger)
    {
        _logger = logger?.ResolveLogger<SqliteWrapperQueryManager>();
    }
}
