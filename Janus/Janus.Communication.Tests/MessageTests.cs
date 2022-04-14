using static Janus.Communication.Messages.MessageExtensions;
using Janus.Communication.Messages;
using Xunit;
using FunctionalExtensions.Base.Results;
using Janus.Commons.SchemaModels;

namespace Janus.Communication.Tests;

public class MessageTests
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
                                            .AddTableau("tableau2", tableauBuilder => tableauBuilder)
                                            .AddTableau("tableau3", tableauBuilder => tableauBuilder))
                             .AddSchema("schema2", schemaBuilder => schemaBuilder)
                             .AddSchema("schema3", schemaBuilder => schemaBuilder)
                             .Build();
}
