using Janus.Wrapper.SchemaInferrence;
using Janus.Wrapper.Sqlite.SchemaInferrence;
using Xunit;

namespace Janus.Wrapper.Sqlite.Tests;
public class SqliteSchemaInferrenceTests
{
    private readonly SchemaInferrer _schemaInferrer;
    private readonly string _connectionString;
    public SqliteSchemaInferrenceTests(string connectionString = "Data Source=./test.db;")
    {
        _connectionString = connectionString;
        _schemaInferrer = new SchemaInferrer(new SqliteSchemaModelProvider(_connectionString), "testdb");
    }

    [Fact]
    public void InferrSchemaTest()
    {
        var inferrenceResult = _schemaInferrer.InferSchemaModel();

        Assert.True(inferrenceResult);
    }
}
