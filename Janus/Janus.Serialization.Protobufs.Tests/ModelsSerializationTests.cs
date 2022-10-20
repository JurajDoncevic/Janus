using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.Protobufs.Tests;
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
                                                                         .WithIsIdentity(true))
                                                     .AddAttribute("attr2", attributeBuilder =>
                                                         attributeBuilder.WithIsNullable(true)
                                                                         .WithDataType(DataTypes.STRING)
                                                                         .WithOrdinal(1)
                                                                         .WithIsIdentity(false))
                                                     .AddAttribute("attr3", attributeBuilder =>
                                                         attributeBuilder.WithIsNullable(true)
                                                                         .WithDataType(DataTypes.DECIMAL)
                                                                         .WithOrdinal(2)
                                                                         .WithIsIdentity(false))
                                                     .WithDefaultUpdateSet())
                                            .AddTableau("tableau2", tableauBuilder =>
                                                tableauBuilder.AddAttribute("attr1", attributeBuilder =>
                                                        attributeBuilder.WithDataType(DataTypes.INT)
                                                                        .WithIsNullable(false))
                                                              .AddAttribute("attr2", attributeBuilder =>
                                                        attributeBuilder.WithDataType(DataTypes.STRING)
                                                                        .WithIsNullable(false))
                                                              .AddUpdateSet(conf => conf.WithAttributesNamed("attr1", "attr2")))
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

    private readonly ISerializationProvider<byte[]> _serializationProvider = new ProtobufsSerializationProvider();

    [Fact(DisplayName = "Round-trip serialization of DataSource with Protobufs")]
    public void RoundTripDataSource()
    {
        var dataSource = GetTestDataSource();
        var serializer = _serializationProvider.DataSourceSerializer;

        var serialization = serializer.Serialize(dataSource);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(dataSource, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of Query with Protobufs")]
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
        var serializer = _serializationProvider.QuerySerializer;

        var serialization = serializer.Serialize(query);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(query, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of TabularData with Protobufs")]
    public void RoundTripTabularData()
    {
        var tabularData = GetTestTabularData();

        var serializer = _serializationProvider.TabularDataSerializer;

        var serialization = serializer.Serialize(tabularData);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(tabularData, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of DeleteCommand with Protobufs")]
    public void RoundTripDeleteCommand()
    {
        var dataSource = GetTestDataSource();


        var deleteCommand = DeleteCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithSelection(conf => conf.WithExpression(EQ("attr1_FK", 1)))
                                .Build();

        var serializer = _serializationProvider.DeleteCommandSerializer;

        var serialization = serializer.Serialize(deleteCommand);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(deleteCommand, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of UpdateCommand with Protobufs")]
    public void RoundTripUpdateCommand()
    {
        var dataSource = GetTestDataSource();

        var updateCommand = UpdateCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithMutation(conf => conf.WithValues(new() { { "attr2", null }, { "attr3", 2.0 } }))
                                .WithSelection(conf => conf.WithExpression(EQ("attr1_FK", 1)))
                                .Build();

        var serializer = _serializationProvider.UpdateCommandSerializer;

        var serialization = serializer.Serialize(updateCommand);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(updateCommand, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip serialization of InsertCommand with Protobufs")]
    public void RoundTripInsertCommand()
    {
        var dataSource = GetTestDataSource();

        var dataToInsert = TabularDataBuilder.InitTabularData(new() { { "attr1_FK", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                .AddRow(conf => conf.WithRowData(new() { { "attr1_FK", 1 }, { "attr2", null }, { "attr3", 2.0 } }))
                                .Build();

        var insertCommand = InsertCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                .Build();

        var serializer = _serializationProvider.InsertCommandSerializer;

        var serialization = serializer.Serialize(insertCommand);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(insertCommand, deserialization.Data);
    }
}
