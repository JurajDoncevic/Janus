﻿using Janus.Commons.SchemaModels;
using Janus.Commons.SchemaModels.Exceptions;
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
        [Fact(DisplayName = "Test builder on a general use case")]
        public void BuilderCaseTest()
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

        [Fact(DisplayName = "Exception is thrown when adding a schema with existing name")]
        public void BuildWithAssignedSchemaName()
        {
            Assert.Throws<SchemaNameAssignedException>(() =>
            {
                var dataSource =
                    SchemaModelBuilder.InitDataSource("testDataSource")
                        .AddSchema("testSchema", schemaBuilder => schemaBuilder)
                        .AddSchema("testSchema", schemaBuilder => schemaBuilder)
                    .Build();
            });
        }

        [Fact(DisplayName = "Exception is thrown when adding a tableau with existing name")]
        public void BuildWithAssignedTableauName()
        {
            Assert.Throws<TableauNameAssignedException>(() =>
            {
                var dataSource =
                    SchemaModelBuilder.InitDataSource("testDataSource")
                        .AddSchema("testSchema", schemaBuilder => 
                            schemaBuilder
                                .AddTableau("testTableau", tableauBuilder => tableauBuilder)
                                .AddTableau("testTableau", tableauBuilder => tableauBuilder))
                    .Build();
            });
        }

        [Fact(DisplayName = "Exception is thrown when adding an attribute with existing name")]
        public void BuildWithAssignedAttributeName()
        {
            Assert.Throws<AttributeNameAssignedException>(() =>
            {
                var dataSource =
                    SchemaModelBuilder.InitDataSource("testDataSource")
                        .AddSchema("testSchema", schemaBuilder =>
                            schemaBuilder
                                .AddTableau("testTableau", tableauBuilder => 
                                    tableauBuilder.AddAttribute("testAttribute", attributeBuilder => attributeBuilder)
                                                  .AddAttribute("testAttribute", attributeBuilder => attributeBuilder)))
                    .Build();
            });
        }

        [Fact(DisplayName = "Exception is thrown when adding an attribute with existing ordinal")]
        public void BuildWithAssignedAttributeOrdinal()
        {
            Assert.Throws<AttributeOrdinalAssignedException>(() =>
            {
                var dataSource =
                    SchemaModelBuilder.InitDataSource("testDataSource")
                        .AddSchema("testSchema", schemaBuilder =>
                            schemaBuilder
                                .AddTableau("testTableau", tableauBuilder =>
                                    tableauBuilder.AddAttribute("testAttribute1", attributeBuilder => attributeBuilder.WithOrdinal(1))
                                                  .AddAttribute("testAttribute2", attributeBuilder => attributeBuilder.WithOrdinal(1))))
                    .Build();
            });
        }

        [Fact(DisplayName = "Exception is thrown when adding an attribute with ordinal out of range")]
        public void BuildWitAttributeOrdinalOutOfRange()
        {
            Assert.Throws<AttributeOrdinalOutOfRange>(() =>
            {
                var dataSource =
                    SchemaModelBuilder.InitDataSource("testDataSource")
                        .AddSchema("testSchema", schemaBuilder =>
                            schemaBuilder
                                .AddTableau("testTableau", tableauBuilder =>
                                    tableauBuilder.AddAttribute("testAttribute1", attributeBuilder => attributeBuilder.WithOrdinal(-2))))
                    .Build();
            });
        }

        [Fact(DisplayName = "Test automatic incremental ordinal assignment")]
        public void BuildWithAutomaticOrdinalGeneration()
        {
            var dataSource =
                SchemaModelBuilder.InitDataSource("testDataSource")
                    .AddSchema("testSchema", schemaBuilder =>
                        schemaBuilder.AddTableau("testTableau", tableauBuilder => 
                            tableauBuilder.AddAttribute("attr1", attributeBuilder => attributeBuilder)
                                          .AddAttribute("attr2", attributeBuilder => attributeBuilder)
                                          .AddAttribute("attr3", attributeBuilder => attributeBuilder)
                                          .AddAttribute("attr4", attributeBuilder => attributeBuilder)))
                    .Build();
            Assert.Equal(0, dataSource["testSchema"]["testTableau"]["attr1"].Ordinal);
            Assert.Equal(1, dataSource["testSchema"]["testTableau"]["attr2"].Ordinal);
            Assert.Equal(2, dataSource["testSchema"]["testTableau"]["attr3"].Ordinal);
            Assert.Equal(3, dataSource["testSchema"]["testTableau"]["attr4"].Ordinal);
        }

        [Fact(DisplayName = "Test element ID generation")]
        public void CheckIdGenerationOnBuild()
        {

            var dataSource = SchemaModelBuilder.InitDataSource("testDataSource")
                                .AddSchema("schema1", schemaBuilder =>
                                    schemaBuilder
                                        .AddTableau("tableau1", tableauBuilder => 
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder))
                                        .AddTableau("tableau2", tableauBuilder => 
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder)))
                                .AddSchema("schema2", schemaBuilder =>
                                    schemaBuilder
                                        .AddTableau("tableau1", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder))
                                        .AddTableau("tableau2", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder)
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder)))
                                .Build();

            Assert.Equal("testDataSource.schema1", dataSource["schema1"].Id);
            Assert.Equal("testDataSource.schema2", dataSource["schema2"].Id);

            Assert.Equal("testDataSource.schema1.tableau1", dataSource["schema1"]["tableau1"].Id);
            Assert.Equal("testDataSource.schema1.tableau2", dataSource["schema1"]["tableau2"].Id);
            Assert.Equal("testDataSource.schema2.tableau1", dataSource["schema2"]["tableau1"].Id);
            Assert.Equal("testDataSource.schema2.tableau2", dataSource["schema2"]["tableau2"].Id);

            Assert.Equal("testDataSource.schema1.tableau1.attr1", dataSource["schema1"]["tableau1"]["attr1"].Id);
            Assert.Equal("testDataSource.schema1.tableau1.attr2", dataSource["schema1"]["tableau1"]["attr2"].Id);
            Assert.Equal("testDataSource.schema1.tableau1.attr3", dataSource["schema1"]["tableau1"]["attr3"].Id);
            Assert.Equal("testDataSource.schema1.tableau2.attr1", dataSource["schema1"]["tableau2"]["attr1"].Id);
            Assert.Equal("testDataSource.schema1.tableau2.attr2", dataSource["schema1"]["tableau2"]["attr2"].Id);
            Assert.Equal("testDataSource.schema1.tableau2.attr3", dataSource["schema1"]["tableau2"]["attr3"].Id);
            Assert.Equal("testDataSource.schema2.tableau1.attr1", dataSource["schema2"]["tableau1"]["attr1"].Id);
            Assert.Equal("testDataSource.schema2.tableau1.attr2", dataSource["schema2"]["tableau1"]["attr2"].Id);
            Assert.Equal("testDataSource.schema2.tableau1.attr3", dataSource["schema2"]["tableau1"]["attr3"].Id);
            Assert.Equal("testDataSource.schema2.tableau2.attr1", dataSource["schema2"]["tableau2"]["attr1"].Id);
            Assert.Equal("testDataSource.schema2.tableau2.attr2", dataSource["schema2"]["tableau2"]["attr2"].Id);
            Assert.Equal("testDataSource.schema2.tableau2.attr3", dataSource["schema2"]["tableau2"]["attr3"].Id);


        }
    }
}
