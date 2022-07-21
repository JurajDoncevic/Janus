using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Serialization.Json.CommandModels;
using Janus.Serialization.Json.DataModels;
using Janus.Serialization.Json.QueryModels;
using Janus.Serialization.Json.SchemaModels;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.Tests.Json;
public class ModelsSerializationTests
{
    private DataSource GetTestDataSource()
        => SchemaModelBuilder.InitDataSource("datasource1")
                             .AddSchema("schema1", schemaBuilder =>
                               schemaBuilder.AddTableau("tableau1", tableauBuilder =>
                                       tableauBuilder.AddAttribute("attr1_FK", attributeBuilder =>
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
                                            .AddTableau("tableau2", tableauBuilder =>
                                                tableauBuilder.AddAttribute("attr1", attributeBuilder =>
                                                        attributeBuilder.WithDataType(DataTypes.INT)
                                                                        .WithIsNullable(false)))
                                            .AddTableau("tableau3", tableauBuilder => tableauBuilder))
                             .AddSchema("schema2", schemaBuilder => schemaBuilder)
                             .AddSchema("schema3", schemaBuilder => schemaBuilder)
                             .Build();

    private TabularData GetTestTabularData()
        => TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?> { { "attr1", 1 }, { "attr2", "TEST1" }, { "attr3", 1.0 } }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?> { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.1 } }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?> { { "attr1", 3 }, { "attr2", "TEST3" }, { "attr3", 3.1 } }))
            .Build();

    [Fact(DisplayName = "Round-trip serialization of DataSource with JSON")]
    public void RoundTripDataSource()
    {
        var dataSource = GetTestDataSource();
        DataSourceSerializer serializer = new DataSourceSerializer();

        var serialization = serializer.Serialize(dataSource);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(dataSource, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of Query with JSON")]
    public void RoundTripQuery()
    {
        var dataSource = GetTestDataSource();
        var query =
            QueryModelBuilder.InitQueryOnDataSource("datasource1.schema1.tableau1", dataSource)
                .WithJoining(conf => conf.AddJoin("datasource1.schema1.tableau1.attr1_FK", "datasource1.schema1.tableau2.attr1"))
                .WithProjection(conf => conf.AddAttribute("datasource1.schema1.tableau1.attr1_FK")
                                            .AddAttribute("datasource1.schema1.tableau2.attr1"))
                .WithSelection(conf => conf.WithExpression(AND(GT("datasource1.schema1.tableau2.attr1", 1), TRUE())))
                .Build();
        var serializer = new QuerySerializer();

        var serialization = serializer.Serialize(query);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(query, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of TabularData with JSON")]
    public void RoundTripTabularData()
    {
        var tabularData = GetTestTabularData();

        var serializer = new TabularDataSerializer();

        var serialization = serializer.Serialize(tabularData);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(tabularData, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of DeleteCommand with JSON")]
    public void RoundTripDeleteCommand()
    {
        var dataSource = GetTestDataSource();


        var deleteCommand = DeleteCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithSelection(conf => conf.WithExpression(EQ("attr1_FK", 1)))
                                .Build();
        var serializer = new DeleteCommandSerializer();

        var serialization = serializer.Serialize(deleteCommand);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(deleteCommand, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of UpdateCommand with JSON")]
    public void RoundTripUpdateCommand()
    {
        var dataSource = GetTestDataSource();

        var updateCommand = UpdateCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithMutation(conf => conf.WithValues(new() { { "attr2", null }, { "attr3", 2.0 } }))
                                .WithSelection(conf => conf.WithExpression(EQ("attr1_FK", 1)))
                                .Build();
        var serializer = new UpdateCommandSerializer();

        var serialization = serializer.Serialize(updateCommand);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(updateCommand, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of InsertCommand with JSON")]
    public void RoundTripInsertCommand()
    {
        var dataSource = GetTestDataSource();

        var dataToInsert = TabularDataBuilder.InitTabularData(new() { { "attr1_FK", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                .AddRow(conf => conf.WithRowData(new() { { "attr1_FK", 1 }, { "attr2", null }, { "attr3", 2.0 } }))
                                .Build();

        var insertCommand = InsertCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                .Build();

        var serializer = new InsertCommandSerializer();

        var serialization = serializer.Serialize(insertCommand);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(insertCommand, deserialization.Data);
    }
}
