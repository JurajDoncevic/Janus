using Janus.Commons.SchemaModels;
using Janus.Wrapper.Core.SchemaInferrence;

namespace Janus.Wrapper.CsvFiles.Tests;

public class WrapperCsvFilesTests
{
    [Fact(DisplayName = "Infer a schema in the CSV files example")]
    public void InferSchemaOnCsvFilesExample()
    {
        var expectedSchemaModel = SchemaModelBuilder.InitDataSource("DataSet")
            .Build();

        var schemaModelProvider = new CsvFileSystemSchemaModelProvider("./DataSet", ';');
        var schemaInferrer = new SchemaInferrer(schemaModelProvider);

        var result = schemaInferrer.InferSchemaModel();
        var str = result.Data?.ToString();
        Assert.True(result);
        Assert.NotNull(result.Data);
        //Assert.Equal(expectedSchemaModel, result.Data);

    }
}