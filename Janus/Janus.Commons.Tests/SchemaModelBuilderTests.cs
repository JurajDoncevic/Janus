using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Commons.Tests
{
    public class SchemaModelBuilderTests
    {
        [Fact(DisplayName = "Test builder on a use case")]
        public void TestBuilderOnCase()
        {
            var dataSource =
            SchemaModelBuilder.InitDataSource("datasource1")
                              .AddSchema("schema1", schemaBuilder =>
                                schemaBuilder
                                    .AddTableau("tableau1", tableauBuilder =>
                                        tableauBuilder
                                            .AddAttribute("attr1_FK", attributeBuilder =>
                                                attributeBuilder.WithIsNullable(false)
                                                                .WithDataType(DataTypes.INT)
                                                                .WithOrdinal(0)
                                                                .WithIsPrimaryKey(true))
                                            .AddAttribute("attr2", attributeBuilder =>
                                                attributeBuilder.WithIsNullable(true)
                                                                .WithDataType(DataTypes.STRING)
                                                                .WithOrdinal(1)
                                                                .WithIsPrimaryKey(false))
                                            .AddAttribute("attr3", attributeBuilder =>
                                                attributeBuilder.WithIsNullable(true)
                                                                .WithDataType(DataTypes.DECIMAL)
                                                                .WithOrdinal(2)
                                                                .WithIsPrimaryKey(false)))
                                        .AddTableau("tableau2", tableauBuilder => tableauBuilder)
                                        .AddTableau("tableau3", tableauBuilder => tableauBuilder))
                              .AddSchema("schema2", schemaBuilder => schemaBuilder)
                              .AddSchema("schema3", schemaBuilder => schemaBuilder)
                              .Build();


            Assert.Equal("datasource1", dataSource.Name);
            Assert.Contains("schema1", dataSource.SchemaNames);
            Assert.Contains("schema2", dataSource.SchemaNames);
            Assert.Contains("schema3", dataSource.SchemaNames);
            Assert.Equal("tableau1", dataSource["schema1"]["tableau1"].Name);
            Assert.Equal("tableau2", dataSource["schema1"]["tableau2"].Name);
            Assert.Equal("tableau3", dataSource["schema1"]["tableau3"].Name);
            Assert.Equal("attr1_FK", dataSource["schema1"]["tableau1"]["attr1_FK"].Name);
            Assert.Equal("attr2", dataSource["schema1"]["tableau1"]["attr2"].Name);
            Assert.Equal("attr3", dataSource["schema1"]["tableau1"]["attr3"].Name);
            Assert.Equal(DataTypes.INT, dataSource["schema1"]["tableau1"]["attr1_FK"].DataType);
            Assert.Equal(DataTypes.STRING, dataSource["schema1"]["tableau1"]["attr2"].DataType);
            Assert.Equal(DataTypes.DECIMAL, dataSource["schema1"]["tableau1"]["attr3"].DataType);
            Assert.True(dataSource["schema1"]["tableau1"]["attr1_FK"].IsPrimaryKey);
            Assert.False(dataSource["schema1"]["tableau1"]["attr2"].IsPrimaryKey);
            Assert.False(dataSource["schema1"]["tableau1"]["attr3"].IsPrimaryKey);
            Assert.False(dataSource["schema1"]["tableau1"]["attr1_FK"].IsNullable);
            Assert.True(dataSource["schema1"]["tableau1"]["attr2"].IsNullable);
            Assert.True(dataSource["schema1"]["tableau1"]["attr3"].IsNullable);
        }
    }
}
