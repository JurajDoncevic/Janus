using Janus.Communication.Messages;
using Janus.Communication.Remotes;
using Janus.Communication.Tests.TestFixtures;
using Xunit;

namespace Janus.Communication.Tests;

public class CommunicationNodeTests : IClassFixture<CommunicationNodeTestFixture>
{
    private IReadOnlyDictionary<string, MaskCommunicationNode> _maskCommunicationNodes;
    private IReadOnlyDictionary<string, MediatorCommunicationNode> _mediatorCommunicationNodes;
    private IReadOnlyDictionary<string, WrapperCommunicationNode> _wrapperCommunicationNodes;
    private readonly CommunicationNodeTestFixture _testFixture;
    
    public CommunicationNodeTests(CommunicationNodeTestFixture testFixture)
    {
        _testFixture = testFixture;
        _maskCommunicationNodes = testFixture.MaskCommunicationNodes;
        _mediatorCommunicationNodes = testFixture.MediatorCommunicationNodes;
        _wrapperCommunicationNodes = testFixture.WrapperCommunicationNodes;
    }

    [Fact(DisplayName = "Test HELLO between 2 components")]
    public void HelloBetweenTwoComponents()
    {
        var mask2ReceivedHellos = new List<(HelloMessage message, RemotePoint remote)>();
        var mask1ReceivedHellos = new List<(HelloMessage message, RemotePoint remote)>();
        var maskNode1 = _maskCommunicationNodes["Mask1"]; 
        var maskNode2 = _maskCommunicationNodes["Mask2"];
        maskNode1.OnHelloReceived += (sender, args) => mask1ReceivedHellos.Add((args.Message, args.RemotePoint));
        maskNode2.OnHelloReceived += (sender, args) => mask2ReceivedHellos.Add((args.Message, args.RemotePoint));
        var remotePoint1 = new MaskRemotePoint(maskNode1.Options.Id, "127.0.0.1", maskNode1.Options.Port);
        var remotePoint2 = new MaskRemotePoint(maskNode2.Options.Id, "127.0.0.1", maskNode2.Options.Port);

        var result = maskNode1.SendHello(remotePoint2);

        System.Threading.Thread.Sleep(1000);

        Assert.True(result.IsSuccess);
        Assert.Contains(remotePoint1, maskNode2.RemotePoints);
        Assert.Contains(remotePoint2, maskNode1.RemotePoints);
        Assert.Single(mask1ReceivedHellos);
        Assert.Single(mask2ReceivedHellos);
    }
}
