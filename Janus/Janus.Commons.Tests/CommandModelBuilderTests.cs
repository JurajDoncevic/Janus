using Janus.Commons.CommandModels;
using Janus.Commons.CommandModels.Exceptions;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;
using System.Collections.Generic;
using Xunit;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Commons.Tests;

public class CommandModelBuilderTests
{

    private DataSource GetSchemaModel() =>
        SchemaModelBuilder.InitDataSource("datasource")
            .AddSchema("schema1",
                schemaConf => schemaConf.AddTableau("tableau1",
                    tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                         .WithIsNullable(false))
                                              .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                         .WithIsNullable(true))
                                              .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                         .WithIsNullable(false))
                                              .AddAttribute("attr4", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                         .WithIsNullable(false)
                                                                                         .WithIsIdentity(true))
                                              .WithDefaultUpdateSet())
                .AddTableau("tableau2",
                    tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                         .WithIsNullable(false))
                                              .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                         .WithIsNullable(false))
                                              .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                         .WithIsNullable(false))
                                              .WithDefaultUpdateSet())
                .AddTableau("tableau3",
                    tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                         .WithIsNullable(false))
                                              .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                         .WithIsNullable(false))
                                              .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                         .WithIsNullable(false))
                                              .AddAttribute("attr4", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                         .WithIsNullable(false))
                                              .AddUpdateSet(conf => conf.WithAttributesNamed("attr1", "attr2"))
                                              .AddUpdateSet(conf => conf.WithAttributesNamed("attr3", "attr4"))))
            .Build();


    [Fact(DisplayName = "Construct a valid insert command")]
    public void ConstructSimpleInsertCommand()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau1"].Id;
        var dataToInsert =
            TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL }, { "attr4", DataTypes.INT } })
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 }, { "attr4", 0 } }))
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.3 }, { "attr4", 1 } }))
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
            TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL }, { "attr4", DataTypes.INT } })
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 }, { "attr4", 0 } }))
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.3 }, { "attr4", 1 } }))
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


    [Fact(DisplayName = "Fail to create a valid insert command without tableau-wide update set")]
    public void FailToCreateInsertCommandWithoutTableauWideUpdateSet()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau3"].Id;
        var dataToInsert =
            TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL }, { "attr4", DataTypes.INT } })
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 }, { "attr4", 0 } }))
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.3 }, { "attr4", 1 } }))
                              .Build();

        Assert.Throws<CommandAllowedOnTableauWideUpdateSetException>(() =>
        {
            var insertCommand =
            InsertCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                .Build();
        });
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
            TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.DECIMAL }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL }, { "attr4", DataTypes.INT } })
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 1.2 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 }, { "attr4", 0 } }))
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 2.3 }, { "attr2", "TEST_STRING2" }, { "attr3", 2.3 }, { "attr4", 1 } }))
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
                                .WithSelection(conf => conf.WithExpression(EQ("datasource.schema1.tableau1.attr1", 1)))
                                .Build();

        Assert.NotNull(updateCommand);
        Assert.Equal(tableauId, updateCommand.OnTableauId);
        Assert.NotNull(updateCommand.Mutation);
        Assert.True(updateCommand.Selection);

    }

    [Fact(DisplayName = "Fail to construct a valid update command outside an update set")]
    public void ConstructInvalidUpdateCommandWithAttributesOutsideUpdateSet()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau3"].Id;
        var valueUpdates = new Dictionary<string, object>() { { "attr2", "TEST_STRING_MOD" }, { "attr3", 1 } };

        Assert.Throws<NoUpdateSetFoundForExpressionAttributesException>(() =>
        {
            var updateCommand =
            UpdateCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                .WithMutation(conf => conf.WithValues(valueUpdates))
                                .WithSelection(conf => conf.WithExpression(EQ("attr1", 1)))
                                .Build();
        });
    }

    [Fact(DisplayName = "Fail to construct a valid update command with an update set mismatch in clause")]
    public void ConstructInvalidUpdateCommandWithUpdateSetMismatch()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau3"].Id;
        var valueUpdates = new Dictionary<string, object>() { { "attr2", "TEST_STRING_MOD" }, { "attr1", 1 } };

        Assert.Throws<UpdateSetMismatchBetweenClausesException>(() =>
        {
            var updateCommand =
            UpdateCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                .WithMutation(conf => conf.WithValues(valueUpdates))
                                .WithSelection(conf => conf.WithExpression(EQ("datasource.schema1.tableau3.attr3", 1)))
                                .Build();
        });
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

    [Fact(DisplayName = "Fail to construct an update command with mutation on a primary key")]
    public void ConstructInvalidUpdateWithPrimaryKeyAttribute()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau1"].Id;
        var valueUpdates = new Dictionary<string, object?>() { { "attr2", "TEST_STRING_MOD" }, { "attr4", 2 } };

        Assert.Throws<MutationOnPrimaryKeyNotAllowedException>(() =>
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
                                .WithSelection(conf => conf.WithExpression(EQ("datasource.schema1.tableau1.attr1", 1)))
                                .Build();

        Assert.NotNull(updateCommand);
        Assert.Equal(tableauId, updateCommand.OnTableauId);
        Assert.NotNull(updateCommand.Mutation);
        Assert.True(updateCommand.Selection);
        Assert.True(updateCommand.IsValidForDataSource(dataSource));

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
                                .WithSelection(conf => conf.WithExpression(EQ("datasource.schema1.tableau1.attr1", 1)))
                                .Build();
        var result = updateCommand.IsValidForDataSource(dataSource);
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
                .WithSelection(conf => conf.WithExpression(EQ("datasource.schema1.tableau1.attr1", 1)))
                .Build();

        var result = updateCommand.IsValidForDataSource(dataSource);
        Assert.False(result);
    }



    [Fact(DisplayName = "Construct a valid delete command")]
    public void CreateValidDeleteCommand()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau1"].Id;

        var deleteCommand =
            DeleteCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                .WithSelection(conf => conf.WithExpression(EQ("datasource.schema1.tableau1.attr1", 1)))
                                .Build();

        Assert.NotNull(deleteCommand);
        Assert.Equal(tableauId, deleteCommand.OnTableauId);
        Assert.True(deleteCommand.Selection);
    }

    [Fact(DisplayName = "Fail to create a delete command with attribute references outside of a update set")]
    public void CreateInvalidDeleteCommandOutsideUpdateSet()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau3"].Id;

        Assert.Throws<CommandAllowedOnTableauWideUpdateSetException>(() =>
        {
            var deleteCommand =
                DeleteCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                    .WithSelection(conf => conf.WithExpression(AND(EQ("attr1", 1), EQ("attr3", 1))))
                                    .Build();
        });
    }

    [Fact(DisplayName = "Fail to create a delete command with unknown attribute")]
    public void CreateInvalidDeleteCommand()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau1"].Id;

        Assert.Throws<AttributeNotInReferencedTableausException>(() =>
        {
            var deleteCommand =
                DeleteCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                    .WithSelection(conf => conf.WithExpression(EQ("datasource.schema1.tableau1.attrX", 1)))
                                    .Build();
        });
    }

    [Fact(DisplayName = "Fail to create a delete command with unknown attribute with open builder")]
    public void CreateInvalidDeleteCommandWithOpenBuilder()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau1"].Id;

        var deleteCommand =
            DeleteCommandOpenBuilder
                .InitOpenDelete(tableauId)
                .WithSelection(conf => conf.WithExpression(EQ("datasource.schema1.tableau1.attrX", 1)))
                .Build();

        Assert.False(deleteCommand.IsValidForDataSource(dataSource));
    }

    [Fact(DisplayName = "Construct a valid delete command with open builder")]
    public void CreateValidDeleteCommandWithOpenBuilder()
    {
        var dataSource = GetSchemaModel();
        var tableauId = dataSource["schema1"]["tableau1"].Id;

        var deleteCommand =
            DeleteCommandOpenBuilder
                .InitOpenDelete(tableauId)
                .WithSelection(conf => conf.WithExpression(EQ("datasource.schema1.tableau1.attr1", 1)))
                .Build();

        Assert.NotNull(deleteCommand);
        Assert.Equal(tableauId, deleteCommand.OnTableauId);
        Assert.True(deleteCommand.Selection);
        Assert.True(deleteCommand.IsValidForDataSource(dataSource));
    }
}
