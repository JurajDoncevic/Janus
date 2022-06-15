using Janus.Commons.SchemaModels;
using Janus.Communication.Messages;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Communication.Tests.Mocks;
using Janus.Communication.Tests.TestFixtures;
using Xunit;

namespace Janus.Communication.Tests;

public class CommunicationNodeTests : IClassFixture<CommunicationNodeTestFixture>
{
    private readonly CommunicationNodeTestFixture _testFixture;

    private MediatorCommunicationNode GetUnresponsiveMediator()
        => CommunicationNodes.CreateMediatorCommunicationNode(
            _testFixture.MediatorCommunicationNodeOptions["MediatorUnresponsive"],
            new AlwaysTimeoutTcpNetworkAdapter(_testFixture.MediatorCommunicationNodeOptions["MediatorUnresponsive"].ListenPort)
            );

    private DataSource GetDataSource()
       => SchemaModelBuilder.InitDataSource("testDataSource")
                            .AddSchema("schema1", schemaBuilder =>
                                schemaBuilder
                                    .AddTableau("tableau1", tableauBuilder =>
                                        tableauBuilder
                                            .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT)))
                                    .AddTableau("tableau2", tableauBuilder =>
                                        tableauBuilder
                                            .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT)))
                                    .AddTableau("tableau3", tableauBuilder =>
                                        tableauBuilder
                                            .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))))
                            .AddSchema("schema2", schemaBuilder =>
                                schemaBuilder
                                    .AddTableau("tableau1", tableauBuilder =>
                                        tableauBuilder
                                            .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr3", attributeBuilder => attributeBuilder))
                                    .AddTableau("tableau2", tableauBuilder =>
                                        tableauBuilder
                                            .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT)))
                                    .AddTableau("tableau3", tableauBuilder =>
                                        tableauBuilder
                                            .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT)))
                                    .AddTableau("tableau4", tableauBuilder =>
                                        tableauBuilder
                                            .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.STRING))
                                            .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT)))
                                    .AddTableau("tableau5", tableauBuilder =>
                                        tableauBuilder
                                            .AddAttribute("attr1", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))
                                            .AddAttribute("attr2", attributeBuilder => attributeBuilder.WithIsNullable(true)
                                                                                                       .WithDataType(DataTypes.STRING))
                                            .AddAttribute("attr3", attributeBuilder => attributeBuilder.WithIsNullable(false)
                                                                                                       .WithDataType(DataTypes.INT))))
                            .Build();

    public CommunicationNodeTests(CommunicationNodeTestFixture testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact(DisplayName = "Exchange HELLO between 2 nodes")]
    public void SendHello()
    {
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

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

    [Fact(DisplayName = "Register nodes using HELLO")]
    public void SendRegister()
    {
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

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
        using var mediator1 = GetUnresponsiveMediator();
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator1");

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var helloResult = mediator1.SendHello(mediator2RemotePoint).Result;


        Assert.True(helloResult.IsFailure);
        Assert.Contains("timeout", helloResult.ErrorMessage.ToLower());
    }

    [Fact(DisplayName = "Test Register timeout")]
    public void RegisterTimeout()
    {
        using var mediator1 = GetUnresponsiveMediator();
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator1");

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var helloResult = mediator1.RegisterRemotePoint(mediator2RemotePoint).Result;


        Assert.True(helloResult.IsFailure);
        Assert.Contains("timeout", helloResult.ErrorMessage.ToLower());
    }

    [Fact(DisplayName = "Send a BYE after a Register")]
    public async void TestBye()
    {
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

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

    [Fact(DisplayName = "Send a COMMAND_REQ and get results")]
    public void SendCommandRequest()
    {
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");
    }

    [Fact(DisplayName = "Send a QUERY_REQ and get results")]
    public void SendQueryRequest()
    {
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");
    }

    [Fact(DisplayName = "Send a SCHEMA_REQ and get results")]
    public async void SendSchemaRequest()
    {
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);


        var registerResult = await mediator1.RegisterRemotePoint(mediator2RemotePoint);

        mediator2.SchemaRequestReceived += async (sender, args) 
            => await mediator2.SendSchemaResponse(args.ReceivedMessage.ExchangeId, GetDataSource(), args.FromRemotePoint);

        var schemaRequestResult = await mediator1.SendSchemaRequest(mediator2RemotePoint);

        Assert.True(registerResult);
        Assert.True(schemaRequestResult);
        Assert.Equal(GetDataSource(), schemaRequestResult.Data);
    }
}
