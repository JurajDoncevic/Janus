using Janus.Wrapper.SchemaInferrence;

namespace Janus.Wrapper.Sqlite;
public sealed class SqliteWrapperSchemaManager : WrapperSchemaManager
{
    public SqliteWrapperSchemaManager(SchemaInferrer schemaInferrer) : base(schemaInferrer)
    {
    }
}
