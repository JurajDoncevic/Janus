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
    [Fact(DisplayName = "Enqueue request messages to the message store")]
    public void EnququeRequestsToMessageStore()
    {
        List<MockMessage> mockMessages = new List<MockMessage>
        {
            new MockMessage("test_node", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node", MockMesagePreambles.MOCK_REQ),
            new MockMessage("test_node", MockMesagePreambles.MOCK_REQ)
        };
        MessageStore messageStore = new MessageStore();

        var addingResult =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.EnqueueRequestInExchange(message.ExchangeId, message))
                .ToList();

        Assert.True(addingResult.All(r => r));
        Assert.Equal(mockMessages.Count, messageStore.CountRequestsEnqueued);
    }

    [Fact(DisplayName = "Enqueue request messages to the message store")]
    public void EnqueueResponsesToMessageStore()
    {
        List<MockMessage> mockMessages = new List<MockMessage>
        {
            new MockMessage("test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node", MockMesagePreambles.MOCK_RES),
            new MockMessage("test_node", MockMesagePreambles.MOCK_RES)
        };
        MessageStore messageStore = new MessageStore();

        var addingResult =
            mockMessages
                .AsParallel()
                .Select(message => messageStore.EnqueueResponseInExchange(message.ExchangeId, message))
                .ToList();

        Assert.True(addingResult.All(r => r));
        Assert.Equal(mockMessages.Count, messageStore.CountResponsesEnqueued);
    }
}
