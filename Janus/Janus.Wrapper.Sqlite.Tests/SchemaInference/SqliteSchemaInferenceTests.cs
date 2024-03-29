﻿using Janus.Wrapper.SchemaInference;
using Janus.Wrapper.Sqlite.SchemaInference;
using Xunit;

namespace Janus.Wrapper.Sqlite.Tests.SchemaInference;
public abstract class SqliteSchemaInferenceTests
{
    private readonly SchemaInferrer _schemaInferrer;
    public abstract string ConnectionString { get; }
    public abstract string DataSourceName { get; }
    public abstract string ExpectedSchemaString { get; }

    public SqliteSchemaInferenceTests()
    {
        _schemaInferrer = new SchemaInferrer(new SqliteSchemaModelProvider(ConnectionString), DataSourceName);
    }

    [Fact(DisplayName = "Inferr a schema model on Sqlite source")]
    public void InferSchemaTest()
    {
        var inferrenceResult = _schemaInferrer.InferSchemaModel();
        var retrivedSchemaString = inferrenceResult.Data!.ToString().Replace("\n", "");

        Assert.True(inferrenceResult);
        Assert.Equal(ExpectedSchemaString, retrivedSchemaString);
    }
}
