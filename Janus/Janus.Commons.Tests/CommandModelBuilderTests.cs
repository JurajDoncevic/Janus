using Janus.Commons.CommandModels;
using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Janus.Commons.SelectionExpressions.SelectionExpressions;

namespace Janus.Commons.Tests
{
    public class CommandModelBuilderTests
    {

        private DataSource GetSchemaModel() =>
            SchemaModelBuilder.InitDataSource("dataSource")
                .AddSchema("schema1",
                    schemaConf => schemaConf.AddTableau("tableau1",
                        tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                             .WithIsNullable(true))
                                                  .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                             .WithIsNullable(false)))
                    .AddTableau("tableau2",
                        tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                             .WithIsNullable(false))))
                .Build();


        [Fact(DisplayName = "Construct a valid insert command")]
        public void ConstructSimpleInsertCommand()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var dataToInsert =
                TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.3 } }))
                                  .Build();

            var insertCommand =
                InsertCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                    .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                    .Build();

            Assert.NotNull(insertCommand);
            Assert.Equal(tableauId, insertCommand.OnTableauId);
            Assert.NotNull(insertCommand.Instantiation);

        }

        [Fact(DisplayName = "Construct a valid insert command with open builder")]
        public void ConstructSimpleInsertCommandWithOpenBuilder()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var dataToInsert =
                TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.3 } }))
                                  .Build();

            var insertCommand =
                InsertCommandOpenBuilder.InitOpenInsert(tableauId)
                                    .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                    .Build();

            Assert.NotNull(insertCommand);
            Assert.True(insertCommand.IsValidForDataSource(dataSource));
            Assert.Equal(tableauId, insertCommand.OnTableauId);
            Assert.NotNull(insertCommand.Instantiation);

        }

        [Fact(DisplayName = "Fail to create an insert command without all tableau values")]
        public void ConstructInvalidInsertWithoutAllValues()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var dataToInsert =
                TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", "TEST_STRING2" } }))
                                  .Build();

            Assert.Throws<MissingInstantiationAttributesException>(() =>
            {
                var insertCommand =
                    InsertCommandBuilder.InitOnDataSource(tableauId, dataSource)
                        .WithInstantiation(conf => conf.WithValues(dataToInsert))
                        .Build();
            });
        }

        [Fact(DisplayName = "Fail to create an insert command without all tableau values on open builder")]
        public void OpenConstructInvalidInsertWithoutAllValues()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var dataToInsert =
                TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", "TEST_STRING2" } }))
                                  .Build();

            var insertCommand =
                InsertCommandOpenBuilder.InitOpenInsert(tableauId)
                    .WithInstantiation(conf => conf.WithValues(dataToInsert))
                    .Build();


            Assert.False(insertCommand.IsValidForDataSource(dataSource));
        }

        [Fact(DisplayName = "Fail to create an insert command without incompatible data type values")]
        public void ConstructInvalidInsertWithIncompatibleDataTypes()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var dataToInsert =
                TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.DECIMAL }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1.2 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2.3 }, { "attr2", "TEST_STRING2" }, { "attr3", 2.3 } }))
                                  .Build();

            Assert.Throws<IncompatibleInstantiationDataTypesException>(() =>
            {
                var insertCommand =
                    InsertCommandBuilder.InitOnDataSource(tableauId, dataSource)
                        .WithInstantiation(conf => conf.WithValues(dataToInsert))
                        .Build();
            });
        }

        [Fact(DisplayName = "Fail to create an insert command without incompatible data type values on open builder")]
        public void OpenConstructInvalidInsertWithIncompatibleDataTypes()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var dataToInsert =
                TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.DECIMAL }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1.2 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2.3 }, { "attr2", "TEST_STRING2" }, { "attr3", 2.3 } }))
                                  .Build();

            var insertCommand =
                InsertCommandOpenBuilder.InitOpenInsert(tableauId)
                    .WithInstantiation(conf => conf.WithValues(dataToInsert))
                    .Build();

            var result = insertCommand.IsValidForDataSource(dataSource);
            Assert.False(result);
        }

        [Fact(DisplayName = "Fail to create an insert command with null in non-nullable field")]
        public void ConstructInvalidInsertWithNullOnAttribute()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau2"].Id;
            var dataToInsert =
                TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.3 } }))
                                  .Build();

            Assert.Throws<NullGivenForNonNullableAttributeException>(() =>
            {
                var insertCommand =
                    InsertCommandBuilder.InitOnDataSource(tableauId, dataSource)
                        .WithInstantiation(conf => conf.WithValues(dataToInsert))
                        .Build();
            });
        }

        [Fact(DisplayName = "Fail to create an insert command with null in non-nullable field on open builder")]
        public void OpenConstructInvalidInsertWithNullOnAttribute()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau2"].Id;
            var dataToInsert =
                TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 } }))
                                  .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.3 } }))
                                  .Build();

            var insertCommand =
                InsertCommandOpenBuilder.InitOpenInsert(tableauId)
                    .WithInstantiation(conf => conf.WithValues(dataToInsert))
                    .Build();

            var result = insertCommand.IsValidForDataSource(dataSource);
            Assert.False(result);
        }

        [Fact(DisplayName = "Construct a valid update command")]
        public void ConstructSimpleUpdateCommand()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var valueUpdates = new Dictionary<string, object>() { { "attr2", "TEST_STRING_MOD" }, { "attr3", 1.9 } };

            var updateCommand =
                UpdateCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                    .WithMutation(conf => conf.WithValues(valueUpdates))
                                    .WithSelection(conf => conf.WithExpression(EQ("attr1", 1)))
                                    .Build();

            Assert.NotNull(updateCommand);
            Assert.Equal(tableauId, updateCommand.OnTableauId);
            Assert.NotNull(updateCommand.Mutation);
            Assert.True(updateCommand.Selection);

        }

        [Fact(DisplayName = "Fail to construct an update command with an unknown attribute mutation")]
        public void ConstructInvalidUpdateWithNonExistingAttribute()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var valueUpdates = new Dictionary<string, object>() { { "attr2", "TEST_STRING_MOD" }, { "attrX", 1.9 } };

            Assert.Throws<AttributeNotInTargetTableauException>(() =>
            {
                var updateCommand =
                    UpdateCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                        .WithMutation(conf => conf.WithValues(valueUpdates))
                                        .WithSelection(conf => conf.WithExpression(EQ("attr1", 1)))
                                        .Build();
            });
        }

        [Fact(DisplayName = "Fail to construct an update command with an wrong attribute data type")]
        public void ConstructInvalidUpdateCommandWithWrongDataType()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var valueUpdates = new Dictionary<string, object>() { { "attr2", 3 }, { "attr3", 1.9 } };

            Assert.Throws<IncompatibleMutationDataTypesException>(() =>
            {
                var updateCommand =
                    UpdateCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                        .WithMutation(conf => conf.WithValues(valueUpdates))
                                        .WithSelection(conf => conf.WithExpression(EQ("attr1", 1)))
                                        .Build();
            });

        }

        [Fact(DisplayName = "Construct a valid update command with open builder")]
        public void ConstructSimpleUpdateCommandWithOpenBuilder()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var valueUpdates = new Dictionary<string, object>() { { "attr2", "TEST_STRING_MOD" }, { "attr3", 1.9 } };

            var updateCommand =
                UpdateCommandOpenBuilder.InitOpenUpdate(tableauId)
                                    .WithMutation(conf => conf.WithValues(valueUpdates))
                                    .WithSelection(conf => conf.WithExpression(EQ("attr1", 1)))
                                    .Build();

            Assert.NotNull(updateCommand);
            Assert.Equal(tableauId, updateCommand.OnTableauId);
            Assert.NotNull(updateCommand.Mutation);
            Assert.True(updateCommand.Selection);
            Assert.True(updateCommand.IsValidOnDataSource(dataSource));

        }

        [Fact(DisplayName = "Fail to construct an update command with an unknown attribute mutation on open builder")]
        public void OpenConstructInvalidUpdateWithNonExistingAttribute()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var valueUpdates = new Dictionary<string, object>() { { "attr2", "TEST_STRING_MOD" }, { "attrX", 1.9 } };

                var updateCommand =
                    UpdateCommandOpenBuilder.InitOpenUpdate(tableauId)
                                        .WithMutation(conf => conf.WithValues(valueUpdates))
                                        .WithSelection(conf => conf.WithExpression(EQ("attr1", 1)))
                                        .Build();
            var result = updateCommand.IsValidOnDataSource(dataSource);
            Assert.False(result);
        }

        [Fact(DisplayName = "Fail to construct an update command with an wrong attribute data type on open builder")]
        public void OpenConstructInvalidUpdateCommandWithWrongDataType()
        {
            var dataSource = GetSchemaModel();
            var tableauId = dataSource["schema1"]["tableau1"].Id;
            var valueUpdates = new Dictionary<string, object>() { { "attr2", 3 }, { "attr3", 1.9 } };

                var updateCommand =
                    UpdateCommandOpenBuilder
                        .InitOpenUpdate(tableauId)
                        .WithMutation(conf => conf.WithValues(valueUpdates))
                        .WithSelection(conf => conf.WithExpression(EQ("attr1", 1)))
                        .Build();

            var result = updateCommand.IsValidOnDataSource(dataSource);
            Assert.False(result);
        }
    }
}
