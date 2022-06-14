using Janus.Communication.Nodes.Utils;
using Janus.Communication.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Communication.Tests;

public class MessageStoreTests
{
    private List<MockMessage> GetMockRequestMessages()
        => new List<MockMessage>
        {
            new MockMessage("test_node1", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node2", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node3", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node4", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node5", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node6", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node7", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node8", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node9", MockMesagePreambles.MOCK_REQ)
        };

    private List<MockMessage> GetMockResponseMessages()
        => new List<MockMessage>
        {
            new MockMessage("test_node1", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node2", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node3", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node4", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node5", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node6", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node7", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node8", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node9", MockMesagePreambles.MOCK_RES)
        };

    private List<MockMessage> GetMockResponseChain(string exchangeId)
        => new List<MockMessage>
        {
            new MockMessage(exchangeId, "test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage(exchangeId, "test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage(exchangeId, "test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage(exchangeId, "test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage(exchangeId, "test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage(exchangeId, "test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage(exchangeId, "test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage(exchangeId, "test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage(exchangeId, "test_node", MockMesagePreambles.MOCK_RES)
        };

    [Fact(DisplayName = "Enqueue request messages to the message store")]
    public void EnququeRequestsToMessageStore()
    {
        MessageStore messageStore = new MessageStore();

        var mockMessages = GetMockRequestMessages();

        var addingResult =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.EnqueueRequestInExchange(message.ExchangeId, message))
                .ToList();

        Assert.True(addingResult.All(r => r));
        Assert.Equal(mockMessages.Count, messageStore.CountRequestsEnqueued);
    }

    [Fact(DisplayName = "Enqueue response messages to the message store")]
    public void EnqueueResponsesToMessageStore()
    {
        MessageStore messageStore = new MessageStore();

        var mockMessages = GetMockResponseMessages();

        var registeringResult =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.RegisterExchange(message.ExchangeId))
                .ToList();

        var addingResult =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.EnqueueResponseInExchange(message.ExchangeId, message))
                .ToList();

        var countMessagesAfterAdding = messageStore.CountResponsesEnqueued;

        Assert.True(registeringResult.All(r => r));
        Assert.True(addingResult.All(r => r));
        Assert.Equal(mockMessages.Count, countMessagesAfterAdding);
    }

    [Fact(DisplayName = "Clear response messages on unregister exchanges")]
    public void EnqueueResponsesToMessageStoreAndUnregister()
    {
        MessageStore messageStore = new MessageStore();

        var mockMessages = GetMockResponseMessages();

        var registeringResult =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.RegisterExchange(message.ExchangeId))
                .ToList();

        var addingResult =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.EnqueueResponseInExchange(message.ExchangeId, message))
                .ToList();

        var countMessagesAfterAdding = messageStore.CountResponsesEnqueued;

        var unregisteringResult =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.UnregisterExchange(message.ExchangeId))
                .ToList();

        var addingResultAfter =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.EnqueueResponseInExchange(message.ExchangeId, message))
                .ToList();

        var countMessagesAfterUnregister = messageStore.CountResponsesEnqueued;

        Assert.True(registeringResult.All(r => r));
        Assert.True(unregisteringResult.All(r => r));
        Assert.True(addingResult.All(r => r));
        Assert.False(addingResultAfter.All(r => r));
        Assert.Equal(mockMessages.Count, countMessagesAfterAdding);
        Assert.Equal(0, countMessagesAfterUnregister);
    }

    [Fact(DisplayName = "Fail to add new responses without register")]
    public void EnqueueResponsesToMessageStoreWithoutRegister()
    {
        MessageStore messageStore = new MessageStore();
        
        const string exchangeId = "someExchangeId";

        var mockMessages = GetMockResponseMessages();
        var unregisteredMessageChain = GetMockResponseChain(exchangeId);


        

        mockMessages.ForEach(message => messageStore.RegisterExchange(message.ExchangeId));

        var addingMessagesResult =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.EnqueueResponseInExchange(message.ExchangeId, message))
                .ToList();

        var addingMessageChainResult =
            unregisteredMessageChain
                .AsParallel()
                .Select(message => messageStore.EnqueueResponseInExchange(message.ExchangeId, message))
                .ToList();

        var countMessagesAfterAdding = messageStore.CountResponsesEnqueued;

        Assert.True(addingMessagesResult.All(r => r));
        Assert.False(addingMessageChainResult.All(r => r));
        Assert.Equal(mockMessages.Count, countMessagesAfterAdding);
    }
}
