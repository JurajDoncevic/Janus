using Janus.Communication.Messages;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;
using Janus.Communication.Tests.Mocks;
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

    [Fact(DisplayName = "Test HELLO between 2 nodes")]
    public void SendHello()
    {
        using var mediator1 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator1"]); 
        using var mediator2 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator2"]);

        HelloReqEventArgs helloRequestEventArgs = null;

        mediator2.HelloRequestReceived += (_, args) => helloRequestEventArgs = args; 

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);

        var helloResult = mediator1.SendHello(mediator2RemotePoint).Result;
        var resultRemotePoint = helloResult.Data;
        
        Assert.True(helloResult.IsSuccess);
        Assert.Equal(mediator2.Options.NodeId, resultRemotePoint.NodeId);
        Assert.Equal(mediator2.Options.ListenPort, resultRemotePoint.Port);
        Assert.Empty(mediator1.RemotePoints);
        Assert.Empty(mediator2.RemotePoints);
        Assert.NotNull(helloRequestEventArgs);
        Assert.Equal(mediator1.Options.NodeId, helloRequestEventArgs.ReceivedMessage.NodeId);

    }

    [Fact(DisplayName = "Test Register using HELLO between 2 nodes")]
    public void SendRegister()
    {
        using var mediator1 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator1"]);
        using var mediator2 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator2"]);

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var registerResult = mediator1.RegisterRemotePoint(mediator2RemotePoint).Result;
        var resultRemotePoint = registerResult.Data;

        Assert.True(registerResult.IsSuccess);
        Assert.Equal(mediator2.Options.NodeId, resultRemotePoint.NodeId);
        Assert.Equal(mediator2.Options.ListenPort, resultRemotePoint.Port);
        Assert.Contains(resultRemotePoint, mediator1.RemotePoints);
        Assert.Equal(mediator2.RemotePoints.First(), mediator1RemotePoint);

    }

    [Fact(DisplayName = "Test HELLO timeout")]
    public void SendHelloTimeout()
    {
        var mockNetworkAdapter = new AlwaysTimeoutTcpNetworkAdapter(_mediatorCommunicationNodeOptions["Mediator2"].ListenPort);
        using var mediator1 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator1"]);
        using var mediator2 = CommunicationNodes.CreateMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator2"], mockNetworkAdapter);

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var helloResult = mediator1.SendHello(mediator2RemotePoint).Result;
        

        Assert.True(helloResult.IsFailure);
        Assert.Contains("timeout", helloResult.ErrorMessage.ToLower());
    }

    [Fact(DisplayName = "Test Register timeout")]
    public void RegisterTimeout()
    {
        var mockNetworkAdapter = new AlwaysTimeoutTcpNetworkAdapter(_mediatorCommunicationNodeOptions["Mediator2"].ListenPort);
        using var mediator1 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator1"]);
        using var mediator2 = CommunicationNodes.CreateMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator2"], mockNetworkAdapter);

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var helloResult = mediator1.RegisterRemotePoint(mediator2RemotePoint).Result;


        Assert.True(helloResult.IsFailure);
        Assert.Contains("timeout", helloResult.ErrorMessage.ToLower());
    }

    [Fact(DisplayName = "Test BYE after a register was received")]
    public async void TestBye()
    {
        using var mediator1 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator1"]);
        using var mediator2 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator2"]);

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var registerResult = mediator1.RegisterRemotePoint(mediator2RemotePoint).Result;
        Result byeResult = default;
        if (registerResult.IsSuccess)
        {
            byeResult = await mediator2.SendBye(mediator1RemotePoint);
        }

        Assert.True(registerResult.IsSuccess);
        Assert.True(byeResult.IsSuccess);
        Assert.DoesNotContain(mediator2RemotePoint, mediator1.RemotePoints);
        Assert.DoesNotContain(mediator1RemotePoint, mediator2.RemotePoints);

    }

    [Fact(DisplayName = "Test BYE by sending register then sending a BYE")]
    public async void TestByeAfterRegister()
    {
        using var mediator1 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator1"]);
        using var mediator2 = CommunicationNodes.CreateTcpMediatorCommunicationNode(_mediatorCommunicationNodeOptions["Mediator2"]);

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var registerResult = mediator1.RegisterRemotePoint(mediator2RemotePoint).Result;
        Result byeResult = default;
        if (registerResult.IsSuccess)
        {
            var remotePoint = registerResult.Data;
            mediator2RemotePoint = (MediatorRemotePoint)remotePoint ?? mediator2RemotePoint;
            byeResult = await mediator1.SendBye(remotePoint);
        }

        Assert.True(registerResult.IsSuccess);
        Assert.True(byeResult.IsSuccess);
        Assert.DoesNotContain(mediator2RemotePoint, mediator1.RemotePoints);
        Assert.DoesNotContain(mediator1RemotePoint, mediator2.RemotePoints);

    }
}
