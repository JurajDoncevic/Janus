using Janus.Commons.SchemaModels;
using Janus.Wrapper.Core.SchemaInferrence.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core.SchemaInferrence;
public class SchemaInferrer
{
    private readonly ISchemaModelProvider _provider;
    private readonly string? _dataSourceName;
    public SchemaInferrer(ISchemaModelProvider schemaModelProvider, string? dataSourceName)
    {
        _provider = schemaModelProvider;
        _dataSourceName = dataSourceName;
    }

    public Result<DataSource> InferSchemaModel()
        => ResultExtensions.AsResult<DataSource>(() =>
        {
            var dataSourceResult = _provider.GetDataSource();
            var schemasResult = _provider.GetSchemas();

            if (dataSourceResult && schemasResult)
            {
                var dataSourceBuilder = SchemaModelBuilder.InitDataSource(_dataSourceName ?? dataSourceResult.Data!.Name);

                foreach (var schema in schemasResult.Data!)
                {
                    dataSourceBuilder.AddSchema(schema.Name, schemaBuilder =>
                    {
                        var tableausResult = _provider.GetTableaus(schema.Name);
                        if (tableausResult)
                        {
                            foreach (var tableau in tableausResult.Data!)
                            {
                                schemaBuilder.AddTableau(tableau.Name, tableauBuilder =>
                                {
                                    var attributesResult = _provider.GetAttributes(schema.Name, tableau.Name);
                                    if (attributesResult)
                                    {
                                        foreach (var attribute in attributesResult.Data!)
                                        {
                                            tableauBuilder.AddAttribute(attribute.Name, attributeBuilder => attributeBuilder.WithDataType(attribute.DataType)
                                                                                                                            .WithIsNullable(attribute.IsNullable)
                                                                                                                            .WithIsPrimaryKey(attribute.IsPrimaryKey)
                                                                                                                            .WithOrdinal(attribute.Ordinal));
                                        }
                                    }
                                    return tableauBuilder;
                                });
                            }
                        }
                        return schemaBuilder;
                    });
                }
                return dataSourceBuilder.Build();
            }
            return null!;
        });
}
