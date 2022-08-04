using Janus.Wrapper.SchemaInferrence;
using Janus.Wrapper.Sqlite.SchemaInferrence;
using Xunit;

namespace Janus.Wrapper.Sqlite.Tests.SchemaInferrence;
public abstract class SqliteSchemaInferrenceTests
{
    private readonly SchemaInferrer _schemaInferrer;
    public abstract string ConnectionString { get; }
    public abstract string DataSourceName { get; }
    public abstract string ExpectedSchemaString { get; }

    public SqliteSchemaInferrenceTests()
    {
        _schemaInferrer = new SchemaInferrer(new SqliteSchemaModelProvider(ConnectionString), DataSourceName);
    }

    [Fact]
    public void InferSchemaTest()
    {
        var inferrenceResult = _schemaInferrer.InferSchemaModel();
        var retrivedSchemaString = inferrenceResult.Data!.ToString().Replace("\n", "");

        Assert.True(inferrenceResult);
        Assert.Equal(ExpectedSchemaString, retrivedSchemaString);
    }
}
