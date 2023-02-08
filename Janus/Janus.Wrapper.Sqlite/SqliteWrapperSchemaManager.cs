using Janus.Logging;
using Janus.Wrapper.SchemaInference;

namespace Janus.Wrapper.Sqlite;
public sealed class SqliteWrapperSchemaManager : WrapperSchemaManager
{
    private readonly ILogger<SqliteWrapperSchemaManager>? _logger;
    public SqliteWrapperSchemaManager(SchemaInferrer schemaInferrer, ILogger? logger = null) : base(schemaInferrer, logger)
    {
        _logger = logger?.ResolveLogger<SqliteWrapperSchemaManager>();
    }
}
