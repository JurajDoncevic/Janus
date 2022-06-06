using static Janus.Communication.Messages.MessageExtensions;
using static Janus.Commons.SelectionExpressions.Expressions;
using Janus.Communication.Messages;
using Xunit;
using FunctionalExtensions.Base.Results;
using Janus.Commons.SchemaModels;
using Janus.Commons.QueryModels;
using Janus.Commons.DataModels;
using Janus.Commons.CommandModels;

namespace Janus.Communication.Tests;

public class MessageSerializationTests
{
    [Fact(DisplayName = "Test HELLO_REQ serialization and deserialization")]
    public void HelloReqSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";
        var port = 2000;
        var nodeType = NodeTypes.MEDIATOR_NODE;
        var rememberMe = false;

        var helloMessage = new HelloReqMessage(exchangeId, nodeId, port, nodeType, rememberMe);

        var messageBytes = helloMessage.ToBson();

        var result = messageBytes.ToHelloReqMessage().Map(_ => (BaseMessage)_);

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.HELLO_REQUEST, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
    }

    [Fact(DisplayName = "Test HELLO_RES serialization and deserialization")]
    public void HelloResSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";
        var port = 2000;
        var nodeType = NodeTypes.MEDIATOR_NODE;
        var rememberMe = false;

        var helloMessage = new HelloResMessage(exchangeId, nodeId, port, nodeType, rememberMe);

        var messageBytes = helloMessage.ToBson();

        var result = messageBytes.ToHelloResMessage().Map(_ => (BaseMessage)_);

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.HELLO_RESPONSE, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
    }

    [Fact(DisplayName = "Test BYE_REQ serialization and deserialization")]
    public void ByeReqSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";
        var port = 2000;
        var nodeType = NodeTypes.MEDIATOR_NODE;
        var rememberMe = false;

        var helloMessage = new ByeReqMessage(exchangeId, nodeId);

        var messageBytes = helloMessage.ToBson();

        var result = messageBytes.ToByeReqMessage().Map(_ => (BaseMessage)_);

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.BYE_REQUEST, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
    }

    [Fact(DisplayName = "Test SCHEMA_REQ serialization and deserialization")]
    public void SchemaReqSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";

        var schemaReqMessage = new SchemaReqMessage(exchangeId, nodeId);

        var messageBytes = schemaReqMessage.ToBson();

        var result = messageBytes.ToSchemaReqMessage().Map(_ => (BaseMessage)_);

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.SCHEMA_REQUEST, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
    }

    [Fact(DisplayName = "Test SCHEMA_RES serialization and deserialization")]
    public void SchemaResSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";

        var dataSource = GetTestDataSource();

        var schemaResMessage = new SchemaResMessage(exchangeId, nodeId, dataSource);

        var messageBytes = schemaResMessage.ToBson();

        var result = messageBytes.ToSchemaResMessage();

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.SCHEMA_RESPONSE, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
        Assert.Equal(GetTestDataSource(), message.DataSource);
    }

    [Fact(DisplayName = "Test QUERY_REQ serialization and deserialization")]
    public void QueryReqSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";

        var dataSource = GetTestDataSource();

        var query = 
            QueryModelBuilder.InitQueryOnDataSource("datasource1.schema1.tableau1", dataSource)
                .WithJoining(conf => conf.AddJoin("datasource1.schema1.tableau1.attr1_FK", "datasource1.schema1.tableau2.attr1"))
                .WithProjection(conf => conf.AddAttribute("datasource1.schema1.tableau1.attr1_FK")
                                            .AddAttribute("datasource1.schema1.tableau2.attr1"))
                .WithSelection(conf => conf.WithExpression(AND(GT("datasource1.schema1.tableau2.attr1", 1), TRUE())))
                .Build();

        var queryReqMessage = new QueryReqMessage(exchangeId, nodeId, query);

        var messageBytes = queryReqMessage.ToBson();
        var messageString = Encoding.UTF8.GetString(messageBytes);
        var result = messageBytes.ToQueryReqMessage();

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.QUERY_REQUEST, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
        Assert.Equal(query, message.Query);
    }

    [Fact(DisplayName = "Test QUERY_RES serialization and deserialization")]
    public void QueryResSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";
        var tabularData = GetTestTabularData();

        var queryResMessage = new QueryResMessage(exchangeId, nodeId, tabularData);

        var messageBytes = queryResMessage.ToBson();
        var messageString = Encoding.UTF8.GetString(messageBytes);
        var result = messageBytes.ToQueryResMessage();

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.QUERY_RESPONSE, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
        Assert.Equal(tabularData, message.TabularData);
    }

    [Fact(DisplayName = "Test COMMAND_REQ with insert serialization and deserialization")]
    public void CommandReqInsertSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";
        var dataSource = GetTestDataSource();

        var dataToInsert = TabularDataBuilder.InitTabularData(new() { { "attr1_FK", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL } })
                                .AddRow(conf => conf.WithRowData(new() { { "attr1_FK", 1 }, { "attr2", null }, { "attr3", 2.0 } }))
                                .Build();

        var insertCommand = InsertCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                .Build();


        var commandReqMessage = new CommandReqMessage(exchangeId, nodeId, insertCommand);

        var messageBytes = commandReqMessage.ToBson();
        var messageString = Encoding.UTF8.GetString(messageBytes);
        var result = messageBytes.ToCommandReqMessage();

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.COMMAND_REQUEST, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
        Assert.Equal(insertCommand, (InsertCommand)message.Command);
    }

    [Fact(DisplayName = "Test COMMAND_REQ with update serialization and deserialization")]
    public void CommandReqUpdateSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";
        var dataSource = GetTestDataSource();

        var updateCommand = UpdateCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithMutation(conf => conf.WithValues(new() { { "attr1_FK", 2 }, { "attr2", null }, { "attr3", 2.0 } }))
                                .WithSelection(conf => conf.WithExpression(EQ("attr1_FK", 1)))
                                .Build();


        var commandReqMessage = new CommandReqMessage(exchangeId, nodeId, updateCommand);

        var messageBytes = commandReqMessage.ToBson();
        var messageString = Encoding.UTF8.GetString(messageBytes);
        var result = messageBytes.ToCommandReqMessage();

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.COMMAND_REQUEST, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
        Assert.Equal(updateCommand, (UpdateCommand)message.Command);
    }

    [Fact(DisplayName = "Test COMMAND_REQ with delete serialization and deserialization")]
    public void CommandReqDeleteSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";
        var dataSource = GetTestDataSource();


        var deleteCommand = DeleteCommandBuilder.InitOnDataSource("datasource1.schema1.tableau1", dataSource)
                                .WithSelection(conf => conf.WithExpression(EQ("attr1_FK", 1)))
                                .Build();


        var commandReqMessage = new CommandReqMessage(exchangeId, nodeId, deleteCommand);

        var messageBytes = commandReqMessage.ToBson();
        var messageString = Encoding.UTF8.GetString(messageBytes);
        var result = messageBytes.ToCommandReqMessage();

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.COMMAND_REQUEST, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
        Assert.Equal(deleteCommand, (DeleteCommand)message.Command);
    }

    [Fact(DisplayName = "Test COMMAND_RES serialization and deserialization")]
    public void CommandResSerializationTest()
    {
        var exchangeId = "test_exchange";
        var nodeId = "test_node";
        var outcomeString = "TEST_STRING";

        var commandResMessage = new CommandResMessage(exchangeId, nodeId, true, outcomeString);

        var messageBytes = commandResMessage.ToBson();
        var messageString = Encoding.UTF8.GetString(messageBytes);
        var result = messageBytes.ToCommandResMessage();

        var message = result.Data;

        Assert.True(result.IsSuccess);
        Assert.Equal(Preambles.COMMAND_RESPONSE, message.Preamble);
        Assert.Equal(exchangeId, message.ExchangeId);
    }

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
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?> { {"attr1", 1 }, {"attr2", "TEST1" }, {"attr3", 1.0 } }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?> { {"attr1", 2 }, {"attr2", null }, {"attr3", 2.1 } }))
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?> { {"attr1", 3 }, {"attr2", "TEST3" }, {"attr3", 3.1 } }))
            .Build();
}
