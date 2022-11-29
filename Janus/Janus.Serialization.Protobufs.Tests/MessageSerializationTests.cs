using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.Messages;
using Janus.Commons.Nodes;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.Protobufs.Tests;

public class MessageSerializationTests
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

    [Fact(DisplayName = "Round-trip HELLO_REQ serialization with Protobufs")]
    public void RoundTripHelloReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var listenPort = 20001;
        var nodeType = NodeTypes.MEDIATOR;
        var rememberMe = true;
        var serializer = _serializationProvider.HelloReqMessageSerializer;
        var helloReq = new HelloReqMessage(exchangeId, nodeId, listenPort, nodeType, rememberMe);

        var serialization = serializer.Serialize(helloReq);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(helloReq, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip HELLO_RES serialization with Protobufs")]
    public void RoundTripHelloRes()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var listenPort = 20001;
        var nodeType = NodeTypes.MEDIATOR;
        var rememberMe = true;
        var serializer = _serializationProvider.HelloResMessageSerializer;
        var helloReq = new HelloResMessage(exchangeId, nodeId, listenPort, nodeType, rememberMe);

        var serialization = serializer.Serialize(helloReq);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(helloReq, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip BYE_REQ serialization with Protobufs")]
    public void RoundTripByeReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var serializer = _serializationProvider.ByeReqMessageSerializer;
        var byeReq = new ByeReqMessage(exchangeId, nodeId);

        var serialization = serializer.Serialize(byeReq);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(byeReq, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip insert COMMAND_REQ serialization with Protobufs")]
    public void RoundTripCommandInsertReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var dataSource = GetTestDataSource();

        var dataToInsert = TabularDataBuilder.InitTabularData(new() { { "attr1_FK", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                .AddRow(conf => conf.WithRowData(new() { { "attr1_FK", 1 }, { "attr2", null }, { "attr3", 2.0 } }))
                                .Build();

        var insertCommand = InsertCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                .Build();


        var commandReqMessage = new CommandReqMessage(exchangeId, nodeId, insertCommand);

        var serializer = _serializationProvider.CommandReqMessageSerializer;

        var serialization = serializer.Serialize(commandReqMessage);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(commandReqMessage, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip delete COMMAND_REQ serialization with Protobufs")]
    public void RoundTripCommandDeleteReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var dataSource = GetTestDataSource();


        var deleteCommand = DeleteCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithSelection(conf => conf.WithExpression(EQ("datasource1.schema1.tableau1.attr1_FK", 1)))
                                .Build();


        var commandReqMessage = new CommandReqMessage(exchangeId, nodeId, deleteCommand);

        var serializer = _serializationProvider.CommandReqMessageSerializer;

        var serialization = serializer.Serialize(commandReqMessage);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(commandReqMessage, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip update COMMAND_REQ serialization with Protobufs")]
    public void RoundTripCommandUpdateReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var dataSource = GetTestDataSource();

        var updateCommand = UpdateCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithMutation(conf => conf.WithValues(new() { { "attr2", null }, { "attr3", 2.0 } }))
                                .WithSelection(conf => conf.WithExpression(EQ("datasource1.schema1.tableau1.attr1_FK", 1)))
                                .Build();


        var commandReqMessage = new CommandReqMessage(exchangeId, nodeId, updateCommand);

        var serializer = _serializationProvider.CommandReqMessageSerializer;

        var serialization = serializer.Serialize(commandReqMessage);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(commandReqMessage, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip COMMAND_RES serialization with Protobufs")]
    public void RoundTripCommandRes()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var isSuccess = true;
        var outcomeDescription = "some outcome description";
        var commandResMessage = new CommandResMessage(exchangeId, nodeId, isSuccess, outcomeDescription);

        var serializer = _serializationProvider.CommandResMessageSerializer;

        var serialization = serializer.Serialize(commandResMessage);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(commandResMessage, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip SCHEMA_REQ serialization with Protobufs")]
    public void RoundTripSchemaReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var schemaReqMessage = new SchemaReqMessage(exchangeId, nodeId);

        var serializer = _serializationProvider.SchemaReqMessageSerializer;

        var serialization = serializer.Serialize(schemaReqMessage);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(schemaReqMessage, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip SCHEMA_RES serialization with Protobufs")]
    public void RoundTripSchemaRes()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var dataSource = GetTestDataSource();
        var schemaResMessage = new SchemaResMessage(exchangeId, nodeId, dataSource);

        var serializer = _serializationProvider.SchemaResMessageSerializer;

        var serialization = serializer.Serialize(schemaResMessage);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(schemaResMessage, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip QUERY_REQ serialization with Protobufs")]
    public void RoundTripQueryReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";

        var dataSource = GetTestDataSource();

        var query =
            QueryModelBuilder.InitQueryOnDataSource("datasource1.schema1.tableau1", dataSource)
                .WithJoining(conf => conf.AddJoin("datasource1.schema1.tableau1.attr1_FK", "datasource1.schema1.tableau2.attr1"))
                .WithProjection(conf => conf.AddAttribute("datasource1.schema1.tableau1.attr1_FK")
                                            .AddAttribute("datasource1.schema1.tableau2.attr1"))
                .WithSelection(conf => conf.WithExpression(AND(GT("datasource1.schema1.tableau2.attr1", 1), TRUE())))
                .Build();

        var queryReqMessage = new QueryReqMessage(exchangeId, nodeId, query);

        var serializer = _serializationProvider.QueryReqMessageSerializer;

        var serialization = serializer.Serialize(queryReqMessage);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(queryReqMessage, deserialization.Data);
    }

    [Fact(DisplayName = "Round-trip QUERY_RES serialization with Protobufs")]
    public void RoundTripQueryRes()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";

        var tabularData = GetTestTabularData();

        var queryResMessage = new QueryResMessage(exchangeId, nodeId, tabularData);


        var serializer = _serializationProvider.QueryResMessageSerializer;

        var serialization = serializer.Serialize(queryResMessage);
        var deserialization = serialization.Bind(bytes => serializer.Deserialize(bytes));

        Assert.True(serialization);
        Assert.True(deserialization);
        Assert.Equal(queryResMessage, deserialization.Data);
    }

    [Fact(DisplayName = "Determine preamble of HELLO_REQ with Protobufs")]
    public void DeterminePreambleOnHelloReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var listenPort = 20001;
        var nodeType = NodeTypes.MEDIATOR;
        var rememberMe = true;
        var serializer = _serializationProvider.HelloReqMessageSerializer;
        var helloReq = new HelloReqMessage(exchangeId, nodeId, listenPort, nodeType, rememberMe);

        var serialization = serializer.Serialize(helloReq);

        var preamble = serialization.Bind(_serializationProvider.DetermineMessagePreamble).Data;

        Assert.Equal(helloReq.Preamble, preamble);
    }

    [Fact(DisplayName = "Determine preamble of HELLO_RES with Protobufs")]
    public void DeterminePreambleOnHelloRes()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var listenPort = 20001;
        var nodeType = NodeTypes.MEDIATOR;
        var rememberMe = true;
        var serializer = _serializationProvider.HelloResMessageSerializer;
        var helloRes = new HelloResMessage(exchangeId, nodeId, listenPort, nodeType, rememberMe);

        var serialization = serializer.Serialize(helloRes);

        var preamble = serialization.Bind(_serializationProvider.DetermineMessagePreamble).Data;

        Assert.Equal(helloRes.Preamble, preamble);
    }

    [Fact(DisplayName = "Determine preamble of BYE_REQ with Protobufs")]
    public void DeterminePreambleOnByeReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var listenPort = 20001;
        var nodeType = NodeTypes.MEDIATOR;
        var rememberMe = true;
        var serializer = _serializationProvider.ByeReqMessageSerializer;
        var byeReq = new ByeReqMessage(exchangeId, nodeId);

        var serialization = serializer.Serialize(byeReq);

        var preamble = serialization.Bind(_serializationProvider.DetermineMessagePreamble).Data;

        Assert.Equal(byeReq.Preamble, preamble);
    }

    [Fact(DisplayName = "Determine preamble of QUERY_REQ with Protobufs")]
    public void DeterminePreambleOnQueryReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";

        var dataSource = GetTestDataSource();

        var query =
            QueryModelBuilder.InitQueryOnDataSource("datasource1.schema1.tableau1", dataSource)
                .WithJoining(conf => conf.AddJoin("datasource1.schema1.tableau1.attr1_FK", "datasource1.schema1.tableau2.attr1"))
                .WithProjection(conf => conf.AddAttribute("datasource1.schema1.tableau1.attr1_FK")
                                            .AddAttribute("datasource1.schema1.tableau2.attr1"))
                .WithSelection(conf => conf.WithExpression(AND(GT("datasource1.schema1.tableau2.attr1", 1), TRUE())))
                .Build();

        var queryReqMessage = new QueryReqMessage(exchangeId, nodeId, query);

        var serializer = _serializationProvider.QueryReqMessageSerializer;

        var serialization = serializer.Serialize(queryReqMessage);

        var preamble = serialization.Bind(_serializationProvider.DetermineMessagePreamble).Data;

        Assert.Equal(queryReqMessage.Preamble, preamble);
    }

    [Fact(DisplayName = "Determine preamble of QUERY_RES with Protobufs")]
    public void DeterminePreambleOnQueryRes()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";

        var tabularData = GetTestTabularData();

        var queryResMessage = new QueryResMessage(exchangeId, nodeId, tabularData);


        var serializer = _serializationProvider.QueryResMessageSerializer;

        var serialization = serializer.Serialize(queryResMessage);

        var preamble = serialization.Bind(_serializationProvider.DetermineMessagePreamble).Data;

        Assert.Equal(queryResMessage.Preamble, preamble);
    }

    [Fact(DisplayName = "Determine preamble of COMMAND_REQ with Protobufs")]
    public void DeterminePreambleOnCommandReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var dataSource = GetTestDataSource();

        var updateCommand = UpdateCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithMutation(conf => conf.WithValues(new() { { "attr2", null }, { "attr3", 2.0 } }))
                                .WithSelection(conf => conf.WithExpression(EQ("datasource1.schema1.tableau1.attr1_FK", 1)))
                                .Build();


        var commandReqMessage = new CommandReqMessage(exchangeId, nodeId, updateCommand);

        var serializer = _serializationProvider.CommandReqMessageSerializer;

        var serialization = serializer.Serialize(commandReqMessage);

        var preamble = serialization.Bind(_serializationProvider.DetermineMessagePreamble).Data;

        Assert.Equal(commandReqMessage.Preamble, preamble);
    }

    [Fact(DisplayName = "Determine preamble of COMMAND_RES with Protobufs")]
    public void DeterminePreambleOnCommandRes()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var isSuccess = true;
        var outcomeDescription = "some outcome description";
        var commandResMessage = new CommandResMessage(exchangeId, nodeId, isSuccess, outcomeDescription);

        var serializer = _serializationProvider.CommandResMessageSerializer;

        var serialization = serializer.Serialize(commandResMessage);

        var preamble = serialization.Bind(_serializationProvider.DetermineMessagePreamble).Data;

        Assert.Equal(commandResMessage.Preamble, preamble);
    }

    [Fact(DisplayName = "Determine preamble of SCHEMA_REQ with Protobufs")]
    public void DeterminePreambleOnSchemaReq()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var schemaReqMessage = new SchemaReqMessage(exchangeId, nodeId);

        var serializer = _serializationProvider.SchemaReqMessageSerializer;

        var serialization = serializer.Serialize(schemaReqMessage);

        var preamble = serialization.Bind(_serializationProvider.DetermineMessagePreamble).Data;

        Assert.Equal(schemaReqMessage.Preamble, preamble);
    }

    [Fact(DisplayName = "Determine preamble of SCHEMA_RES with Protobufs")]
    public void DeterminePreambleOnSchemaRes()
    {
        var exchangeId = "test_exchange_id";
        var nodeId = "test_node_id";
        var dataSource = GetTestDataSource();
        var schemaResMessage = new SchemaResMessage(exchangeId, nodeId, dataSource);

        var serializer = _serializationProvider.SchemaResMessageSerializer;

        var serialization = serializer.Serialize(schemaResMessage);

        var preamble = serialization.Bind(_serializationProvider.DetermineMessagePreamble).Data;

        Assert.Equal(schemaResMessage.Preamble, preamble);
    }
}