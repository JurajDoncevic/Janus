using FunctionalExtensions.Base.Results;
using Janus.Commons.QueryModels;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Commons.Tests
{
    public class QueryModelBuilderTests
    {
        private DataSource GetExampleSchema()
            => SchemaModelBuilder.InitDataSource("testDataSource")
                                .AddSchema("schema1", schemaBuilder =>
                                    schemaBuilder
                                        .AddTableau("tableau1", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT)))
                                        .AddTableau("tableau2", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT)))
                                        .AddTableau("tableau3", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))))
                                .AddSchema("schema2", schemaBuilder =>
                                    schemaBuilder
                                        .AddTableau("tableau1", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder))
                                        .AddTableau("tableau2", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT)))
                                        .AddTableau("tableau3", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT)))
                                        .AddTableau("tableau4", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.STRING))
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT)))
                                        .AddTableau("tableau5", tableauBuilder =>
                                            tableauBuilder
                                                .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))
                                                .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(true)
                                                                                                           .WithDataType(DataTypes.STRING))
                                                .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                           .WithDataType(DataTypes.INT))))
                                .Build();

        [Fact(DisplayName = "Create query on one tableau")]
        public void CreateQueryOnOneTableau()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
            QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                        .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr2")
                                                    .AddAttribute("testDataSource.schema1.tableau1.attr1"))
                        .WithSelection(conf => conf.WithExpression("EXPRESSION"))
                        .Build();

            Assert.True(query.Selection);
            Assert.Equal("EXPRESSION", query.Selection.Value.Expression);
            Assert.True(query.Projection);
            Assert.Equal(2, query.Projection.Value.IncludedAttributeIds.Count);
            Assert.Equal(tableauId, query.OnTableauId);
            Assert.False(query.Joining);

        }

        [Fact(DisplayName = "Create query on multiple tableaus")]
        public void CreateQueryOnMultipleTableaus()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
            QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                        .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr2", "testDataSource.schema1.tableau2.attr1")
                                                 .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau3.attr1"))
                        .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr1")
                                                    .AddAttribute("testDataSource.schema1.tableau1.attr2")
                                                    .AddAttribute("testDataSource.schema1.tableau2.attr1")
                                                    .AddAttribute("testDataSource.schema1.tableau2.attr2")
                                                    .AddAttribute("testDataSource.schema1.tableau3.attr1")
                                                    .AddAttribute("testDataSource.schema1.tableau3.attr2"))
                        .WithSelection(conf => conf.WithExpression("EXPRESSION"))
                        .Build();

            Assert.True(query.Selection);
            Assert.Equal("EXPRESSION", query.Selection.Value.Expression);
            Assert.True(query.Projection);
            Assert.Equal(6, query.Projection.Value.IncludedAttributeIds.Count);
            Assert.Equal(tableauId, query.OnTableauId);
            Assert.True(query.Joining);
            Assert.Equal(2, query.Joining.Value.Joins.Count);
        }


        [Fact(DisplayName = "Fail to create a self join query")]
        public void CreateSelfJoinQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<SelfJoinNotSupportedException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                            .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau1.attr1")
                                                     .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau3.attr1"))
                            .Build();
            });

        }

        [Fact(DisplayName = "Fail to create a cycle join query")]
        public void CreateCycleJoinQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<CyclicJoinNotSupportedException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                            .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1")
                                                     .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau3.attr1")
                                                     .AddJoin("testDataSource.schema1.tableau3.attr1", "testDataSource.schema2.tableau1.attr1")
                                                     .AddJoin("testDataSource.schema2.tableau1.attr1", "testDataSource.schema1.tableau1.attr1"))
                            .Build();
            });

        }

        [Fact(DisplayName = "Fail to create a disconnected join query")]
        public void CreateDisconnectedJoinQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<JoinsNotConnectedException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                            .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1")
                                                     .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau3.attr1")
                                                     .AddJoin("testDataSource.schema2.tableau1.attr1", "testDataSource.schema2.tableau2.attr1")
                                                     .AddJoin("testDataSource.schema2.tableau1.attr1", "testDataSource.schema2.tableau3.attr1"))
                            .Build();
            });

        }

        [Fact(DisplayName = "Fail to create a disconnected join query")]
        public void CreateDuplicatePkTableauJoinQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<TableauPrimaryKeyReferenceNotUniqueException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                            .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1")
                                                     .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema2.tableau3.attr1")
                                                     .AddJoin("testDataSource.schema2.tableau3.attr1", "testDataSource.schema1.tableau2.attr1"))
                            .Build();
            });

        }

        [Fact(DisplayName = "Fail to create a duplicate join query")]
        public void CreateDuplicateJoinQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<DuplicateJoinNotSupportedException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                            .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1")
                                                     .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1"))
                            .Build();
            });

        }

        [Fact(DisplayName = "Fail to create a query with a non-existing attribute in the projection")]
        public void CreateProjectionWithNonExistingAttribute()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<AttributeDoesNotExistException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                            .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr1")
                                                        .AddAttribute("testDataSource.schema1.tableau1.attrFAIL"))
                            .Build();
            });

        }

        [Fact(DisplayName = "Fail to create a query with a duplicate attribute in the projection")]
        public void CreateProjectionWitDuplicateAttribute()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<DuplicateAttributeAssignedToProjectionException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                            .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr1")
                                                        .AddAttribute("testDataSource.schema1.tableau1.attr1"))
                            .Build();
            });
        }

        [Fact(DisplayName = "Fail to create a query with a projection attribute from a tableau not referenced in the query beforehand")]
        public void CreateProjectionWithAttributeFromNonReferencedTableau()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<AttributeNotInReferencedTableausException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                            .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr1")
                                                        .AddAttribute("testDataSource.schema1.tableau2.attr1"))
                            .Build();
            });
        }

        [Fact(DisplayName = "Fail to create a query with incompatible types in join")]
        public void CreateJoinWithIncompatibleTypesInJoin()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<JoinedAttributesNotOfSameTypeException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                        .WithJoining(conf => conf.AddJoin("testDataSource.schema2.tableau4.attr1", "testDataSource.schema2.tableau5.attr2"))
                        .Build();
            });
        }

        [Fact(DisplayName = "Fail to create a query with primary key nullable in join")]
        public void CreateJoinWithPrimaryKeyNullableInJoin()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            Assert.Throws<PrimaryKeyAttributeNullableException>(() =>
            {
                var query =
                QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
                        .WithJoining(conf => conf.AddJoin("testDataSource.schema2.tableau4.attr2", "testDataSource.schema2.tableau5.attr2"))
                        .Build();
            });
        }

        [Fact(DisplayName = "Create valid open query")]
        public void CreateValidOpenQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
            QueryModelOpenBuilder.InitOpenQuery(tableauId)
                        .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr2", "testDataSource.schema1.tableau2.attr1")
                                                 .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau3.attr1"))
                        .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr1")
                                                    .AddAttribute("testDataSource.schema1.tableau1.attr2")
                                                    .AddAttribute("testDataSource.schema1.tableau2.attr1")
                                                    .AddAttribute("testDataSource.schema1.tableau2.attr2")
                                                    .AddAttribute("testDataSource.schema1.tableau3.attr1")
                                                    .AddAttribute("testDataSource.schema1.tableau3.attr2"))
                        .WithSelection(conf => conf.WithExpression("EXPRESSION"))
                        .Build();

            Assert.True(query.Selection);
            Assert.Equal("EXPRESSION", query.Selection.Value.Expression);
            Assert.True(query.Projection);
            Assert.Equal(6, query.Projection.Value.IncludedAttributeIds.Count);
            Assert.Equal(tableauId, query.OnTableauId);
            Assert.True(query.Joining);
            Assert.Equal(2, query.Joining.Value.Joins.Count);
            Assert.True(query.IsValidForDataSource(dataSource));
        }


        [Fact(DisplayName = "Create valid open query on one tableau")]
        public void CreateOneTableauOpenQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
            QueryModelOpenBuilder.InitOpenQuery(tableauId)
                        .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr2")
                                                    .AddAttribute("testDataSource.schema1.tableau1.attr1"))
                        .WithSelection(conf => conf.WithExpression("EXPRESSION"))
                        .Build();

            Assert.True(query.Selection);
            Assert.Equal("EXPRESSION", query.Selection.Value.Expression);
            Assert.True(query.Projection);
            Assert.Equal(2, query.Projection.Value.IncludedAttributeIds.Count);
            Assert.Equal(tableauId, query.OnTableauId);
            Assert.False(query.Joining);
            Assert.True(query.IsValidForDataSource(dataSource));

        }


        [Fact(DisplayName = "Fail to create a self join open query")]
        public void CreateSelfJoinOpenQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;
            var query =
                QueryModelOpenBuilder.InitOpenQuery(tableauId)
                    .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau1.attr1")
                                             .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau3.attr1"))
                    .Build();

            var result = query.IsValidForDataSource(dataSource);
            Assert.False(result);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);

        }

        [Fact(DisplayName = "Fail to create a cycle join open query")]
        public void CreateCycleJoinOpenQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
                QueryModelOpenBuilder.InitOpenQuery(tableauId)
                    .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1")
                                             .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau3.attr1")
                                             .AddJoin("testDataSource.schema1.tableau3.attr1", "testDataSource.schema2.tableau1.attr1")
                                             .AddJoin("testDataSource.schema2.tableau1.attr1", "testDataSource.schema1.tableau1.attr1"))
                    .Build();
            var result = query.IsValidForDataSource(dataSource);
            Assert.False(result);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact(DisplayName = "Fail to create a disconnected join open query")]
        public void CreateDisconnectedJoinOpenQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
                QueryModelOpenBuilder.InitOpenQuery(tableauId)
                    .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1")
                                             .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau3.attr1")
                                             .AddJoin("testDataSource.schema2.tableau1.attr1", "testDataSource.schema2.tableau2.attr1")
                                             .AddJoin("testDataSource.schema2.tableau1.attr1", "testDataSource.schema2.tableau3.attr1"))
                    .Build();

            var result = query.IsValidForDataSource(dataSource);

            Assert.False(result);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);

        }

        [Fact(DisplayName = "Fail to create a disconnected join open query")]
        public void CreateDuplicatePkTableauJoinOpenQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
                QueryModelOpenBuilder.InitOpenQuery(tableauId)
                    .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1")
                                             .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema2.tableau3.attr1")
                                             .AddJoin("testDataSource.schema2.tableau3.attr1", "testDataSource.schema1.tableau2.attr1"))
                    .Build();

            var result = query.IsValidForDataSource(dataSource);

            Assert.False(result);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact(DisplayName = "Fail to create a duplicate join open query")]
        public void CreateDuplicateJoinOpenQuery()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
                QueryModelOpenBuilder.InitOpenQuery(tableauId)
                    .WithJoining(conf => conf.AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1")
                                             .AddJoin("testDataSource.schema1.tableau1.attr1", "testDataSource.schema1.tableau2.attr1"))
                    .Build();

            var result = query.IsValidForDataSource(dataSource);

            Assert.False(result);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact(DisplayName = "Fail to create an open query with a non-existing attribute in the projection")]
        public void CreateOpenQueryProjectionWithNonExistingAttribute()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
                QueryModelOpenBuilder.InitOpenQuery(tableauId)
                    .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr1")
                                                .AddAttribute("testDataSource.schema1.tableau1.attrFAIL"))
                    .Build();

            var result = query.IsValidForDataSource(dataSource);

            Assert.False(result);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact(DisplayName = "Create an open query with a duplicate attribute in the projection")]
        public void CreateOpenQueryProjectionWitDuplicateAttribute()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;


            var query =
                QueryModelOpenBuilder.InitOpenQuery(tableauId)
                    .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr1")
                                                .AddAttribute("testDataSource.schema1.tableau1.attr1"))
                    .Build();


            var result = query.IsValidForDataSource(dataSource);

            Assert.True(result);
        }

        [Fact(DisplayName = "Fail to create an open query with a projection attribute from a tableau not referenced in the query beforehand")]
        public void CreateOpenQueryProjectionWithAttributeFromNonReferencedTableau()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
            QueryModelOpenBuilder.InitOpenQuery(tableauId)
                        .WithProjection(conf => conf.AddAttribute("testDataSource.schema1.tableau1.attr1")
                                                    .AddAttribute("testDataSource.schema1.tableau2.attr1"))
                        .Build();

            var result = query.IsValidForDataSource(dataSource);

            Assert.False(result);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact(DisplayName = "Fail to create an open query with incompatible types in join")]
        public void CreateOpenQueryJoinWithIncompatibleTypesInJoin()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
                QueryModelOpenBuilder.InitOpenQuery(tableauId)
                    .WithJoining(conf => conf.AddJoin("testDataSource.schema2.tableau4.attr1", "testDataSource.schema2.tableau5.attr2"))
                    .Build();

            var result = query.IsValidForDataSource(dataSource);

            Assert.False(result);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }

        [Fact(DisplayName = "Fail to create an open query with primary key nullable in join")]
        public void CreateOpenQueryJoinWithPrimaryKeyNullableInJoin()
        {
            var dataSource = GetExampleSchema();
            string tableauId = dataSource["schema1"]["tableau1"].Id;

            var query =
                QueryModelOpenBuilder.InitOpenQuery(tableauId)
                    .WithJoining(conf => conf.AddJoin("testDataSource.schema2.tableau4.attr2", "testDataSource.schema2.tableau5.attr2"))
                    .Build();

            var result = query.IsValidForDataSource(dataSource);

            Assert.False(result);
            Assert.Equal(ErrorTypes.ExceptionThrown, result.ErrorType);
        }
    }
}
