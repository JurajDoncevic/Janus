using static Janus.Communication.Messages.MessageExtensions;
using Janus.Communication.Messages;
using Xunit;
using FunctionalExtensions.Base.Results;

namespace Janus.Communication.Tests;

public class MessageTests
{
    [Fact]
    public void HelloSerializationTest()
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
}
