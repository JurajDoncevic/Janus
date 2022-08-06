using Janus.Wrapper.SchemaInferrence;

namespace Janus.Wrapper.Sqlite;
public class SqliteWrapperSchemaManager : WrapperSchemaManager
{
    public SqliteWrapperSchemaManager(SchemaInferrer schemaInferrer) : base(schemaInferrer)
    {
    }
}
