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

    public Result<DataSource> InferSchemaModel()
    {
        return
        _provider.GetDataSource()
            .Bind<DataSourceInfo, DataSource>(dataSourceInfo =>
            {
                var dataSourceBuilder = SchemaModelBuilder.InitDataSource(dataSourceInfo.Name);

                var schemaInfosResult = _provider.GetSchemasInDataSource();
                if (schemaInfosResult)
                {
                    foreach (var schemaInfo in schemaInfosResult.Data ?? Enumerable.Empty<SchemaInfo>())
                    {
                        dataSourceBuilder.AddSchema(schemaInfo.Name, schemaBuilder =>
                        {
                            var tableauInfosResult = _provider.GetTableausInSchema(schemaInfo.Name);
                            if (tableauInfosResult)
                            {
                                foreach (var tableauInfo in tableauInfosResult.Data ?? Enumerable.Empty<TableauInfo>())
                                {
                                    schemaBuilder.AddTableau(tableauInfo.Name, tableauBuilder =>
                                    {

                                        var attributeInfosResult = _provider.GetAttributesInTableau(tableauInfo.Name);
                                        if (attributeInfosResult)
                                        {
                                            foreach (var attributeInfo in attributeInfosResult.Data ?? Enumerable.Empty<AttributeInfo>())
                                            {
                                                tableauBuilder.AddAttribute(attributeInfo.Name, attributeBuilder =>
                                                    attributeBuilder.WithDataType(attributeInfo.DataType)
                                                                    .WithIsNullable(attributeInfo.IsNullable)
                                                                    .WithIsPrimaryKey(attributeInfo.IsPrimaryKey)
                                                                    .WithOrdinal(attributeInfo.Ordinal));
                                            }
                                        }
                                        return tableauBuilder;
                                    });
                                }
                            }
                            return schemaBuilder;
                        });
                    }
                }
                return dataSourceBuilder.Build();
            });
    }
}
