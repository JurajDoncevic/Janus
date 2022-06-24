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

    public SchemaInferrer(ISchemaModelProvider schemaModelProvider)
    {
        _provider = schemaModelProvider;
    }

    public Result<DataSource> InferSchemaModel_v1()
        => ResultExtensions.AsResult<DataSource>(() =>
        {
            var dataSourceResult = _provider.GetDataSource();
            var schemasResult = _provider.GetSchemas();

            if (dataSourceResult && schemasResult)
            {
                var dataSourceBuilder = SchemaModelBuilder.InitDataSource(dataSourceResult.Data!.Name);

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

    public Result<DataSource> InferSchemaModel_v2()
    {
        var result =
        _provider.GetDataSource()
            .Map(dataSource => (dataSource, dataSourceBuilder: SchemaModelBuilder.InitDataSource(dataSource.Name)))
            .Map(r => _provider.GetSchemas().Match(x => x, s => new List<SchemaInfo>()).Fold(r.dataSourceBuilder, (schema, dataSourceBuilder) =>
            {
                return dataSourceBuilder.AddSchema(schema.Name, schemaBuilder => _provider.GetTableaus(schema.Name).Match(x => x, s => new List<TableauInfo>()).Fold(schemaBuilder, (tableau, sBuilder) =>
                {
                    return sBuilder.AddTableau(tableau.Name, tableauBuilder => _provider.GetAttributes(schema.Name, tableau.Name).Match(x => x, s => new List<AttributeInfo>()).Fold(tableauBuilder, (attribute, tBuilder) =>
                    {
                        return tBuilder.AddAttribute(attribute.Name, attributeBuilder => attributeBuilder.WithDataType(attribute.DataType)
                                                                                                         .WithIsNullable(attribute.IsNullable)
                                                                                                         .WithIsPrimaryKey(attribute.IsPrimaryKey)
                                                                                                         .WithOrdinal(attribute.Ordinal));
                    }));
                }));
            }))
            .Bind(dataSourceBuilder => ResultExtensions.AsResult(() => dataSourceBuilder.Build()));

        return result;
    }
}
