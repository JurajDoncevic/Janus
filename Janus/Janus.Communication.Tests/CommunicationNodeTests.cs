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
        var mediator1 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["mediator1"]); 
        var mediator2 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["mediator2"]);

        var mediator2RemotePoint = new MediatorRemotePoint("", "127.0.0.1", mediator2.Options.Port);

        var helloResult = mediator1.SendHello(mediator2RemotePoint);
        var resultRemotePoint = helloResult;
        
        Assert.True(helloResult.IsSuccess);
        Assert.Empty(mediator1.RemotePoints);
        Assert.Empty(mediator2.RemotePoints);

    }
}
