﻿using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Wrapper.SchemaInferrence.Model;

namespace Janus.Wrapper.SchemaInferrence;
public class SchemaInferrer
{
    private readonly ISchemaModelProvider _provider;
    private readonly string? _dataSourceName;
    public SchemaInferrer(ISchemaModelProvider schemaModelProvider, string? dataSourceName = null)
    {
        _provider = schemaModelProvider;
        _dataSourceName = dataSourceName;
    }

    public Result<DataSource> InferSchemaModel()
        => Results.AsResult<DataSource>(() =>
        {
            var dataSource =
            _provider.GetDataSource()
                .Match(
                    ds => ds,
                    msg => new DataSourceInfo(_dataSourceName!)
                );
            var schemasResult = _provider.GetSchemas();

            if (schemasResult && dataSource != null)
            {
                var dataSourceBuilder = SchemaModelBuilder.InitDataSource(_dataSourceName ?? dataSource.Name);

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
                                                                                                                            .WithIsIdentity(attribute.IsPrimaryKey)
                                                                                                                            .WithOrdinal(attribute.Ordinal));
                                        }
                                    }
                                    tableauBuilder.WithDefaultUpdateSet();
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
