using Janus.Communication.Messages;
using Janus.Communication.Remotes;
using Janus.Communication.Tests.TestFixtures;
using Xunit;

namespace Janus.Communication.Tests;

public class CommunicationNodeTests : IClassFixture<CommunicationNodeTestFixture>
{
    private IReadOnlyDictionary<string, CommunicationNodeOptions> _maskCommunicationNodeOptions;
    private IReadOnlyDictionary<string, CommunicationNodeOptions> _mediatorCommunicationNodeOptions;
    private IReadOnlyDictionary<string, CommunicationNodeOptions> _wrapperCommunicationNodeOptions;
    private readonly CommunicationNodeTestFixture _testFixture;
    
    public CommunicationNodeTests(CommunicationNodeTestFixture testFixture)
    {
        _wrapperCommunicationNodeOptions = testFixture.WrapperCommunicationNodeOptions;
        _mediatorCommunicationNodeOptions = testFixture.MediatorCommunicationNodeOptions;
        _maskCommunicationNodeOptions = testFixture.MaskCommunicationNodeOptions;

        _testFixture = testFixture;
    }

    [Fact(DisplayName = "Test HELLO between 2 components")]
    public void HelloBetweenTwoComponents()
    {
        using var mediator1 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator1"]); 
        using var mediator2 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator2"]);

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);

        var helloResult = mediator1.SendHello(mediator2RemotePoint).Result;
        var resultRemotePoint = helloResult.Data;
        
        Assert.True(helloResult.IsSuccess);
        Assert.Equal(mediator2.Options.NodeId, resultRemotePoint.Id);
        Assert.Equal(mediator2.Options.ListenPort, resultRemotePoint.Port);
        Assert.Empty(mediator1.RemotePoints);
        Assert.Empty(mediator2.RemotePoints);

    }
}
