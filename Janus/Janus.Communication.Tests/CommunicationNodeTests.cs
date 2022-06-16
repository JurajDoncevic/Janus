﻿using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using static Janus.Commons.SelectionExpressions.Expressions;
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
       => SchemaModelBuilder.InitDataSource("dataSource")
                .AddSchema("schema1",
                    schemaConf => schemaConf.AddTableau("tableau1",
                        tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                             .WithIsNullable(true))
                                                  .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr4", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                             .WithIsNullable(false)
                                                                                             .WithIsPrimaryKey(true)))
                    .AddTableau("tableau2",
                        tableauConf => tableauConf.AddAttribute("attr1", attrConf => attrConf.WithDataType(DataTypes.INT)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr2", attrConf => attrConf.WithDataType(DataTypes.STRING)
                                                                                             .WithIsNullable(false))
                                                  .AddAttribute("attr3", attrConf => attrConf.WithDataType(DataTypes.DECIMAL)
                                                                                             .WithIsNullable(false))))
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
        var dataSource = GetDataSource();
        var tableauId = dataSource["schema1"]["tableau1"].Id;

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
    public async void SendCommandRequest()
    {
        var dataSource = GetDataSource();
        var tableauId = dataSource["schema1"]["tableau1"].Id;

        var dataToInsert =
            TabularDataBuilder.InitTabularData(new() { { "attr1", DataTypes.INT }, { "attr2", DataTypes.STRING }, { "attr3", DataTypes.DECIMAL }, { "attr4", DataTypes.INT } })
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 1 }, { "attr2", "TEST_STRING" }, { "attr3", 1.2 }, { "attr4", 0 } }))
                              .AddRow(conf => conf.WithRowData(new() { { "attr1", 2 }, { "attr2", null }, { "attr3", 2.3 }, { "attr4", 1 } }))
                              .Build();

        var insertCommand =
            InsertCommandBuilder.InitOnDataSource(tableauId, dataSource)
                                .WithInstantiation(conf => conf.WithValues(dataToInsert))
                                .Build();

        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        mediator2.CommandRequestReceived += async (sender, args) =>
        {
            if (args.ReceivedMessage.CommandReqType == CommandReqTypes.INSERT)
            {
                await mediator2.SendCommandResponse(args.ReceivedMessage.ExchangeId, true, args.FromRemotePoint, "Operation success");
            }
        };

        var registerResult = await mediator1.RegisterRemotePoint(mediator2RemotePoint);

        var commandResult = await mediator1.SendCommandRequest(insertCommand, mediator2RemotePoint);

        Assert.True(registerResult);
        Assert.True(commandResult);
        
    }

    [Fact(DisplayName = "Send a QUERY_REQ and get results")]
    public async void SendQueryRequest()
    {
        var dataSource = GetDataSource();
        var tableauId = dataSource["schema1"]["tableau1"].Id;

        var query = QueryModelBuilder.InitQueryOnDataSource(tableauId, dataSource)
            .WithJoining(conf => conf.AddJoin("dataSource.schema1.tableau1.attr1", "dataSource.schema1.tableau2.attr1"))
            .WithSelection(conf => conf.WithExpression(LT("dataSource.schema1.tableau1.attr3", 7.0)))
            .WithProjection(conf => conf.AddAttribute("dataSource.schema1.tableau1.attr1")
                                        .AddAttribute("dataSource.schema1.tableau1.attr2")
                                        .AddAttribute("dataSource.schema1.tableau1.attr3")
                                        .AddAttribute("dataSource.schema1.tableau2.attr1")
                                        .AddAttribute("dataSource.schema1.tableau2.attr3"))
            .Build();

        var queryResponseData = TabularDataBuilder
            .InitTabularData(new () 
            { 
                { "dataSource.schema1.tableau1.attr1", DataTypes.INT }, 
                { "dataSource.schema1.tableau1.attr2", DataTypes.STRING}, 
                { "dataSource.schema1.tableau1.attr3", DataTypes.DECIMAL}, 
                { "dataSource.schema1.tableau2.attr1", DataTypes.INT }, 
                { "dataSource.schema1.tableau2.attr3", DataTypes.DECIMAL } 
            })
            .AddRow(conf => conf.WithRowData(new() 
            {
                { "dataSource.schema1.tableau1.attr1", 1 },
                { "dataSource.schema1.tableau1.attr2", "HELLO1"},
                { "dataSource.schema1.tableau1.attr3", 2.0},
                { "dataSource.schema1.tableau2.attr1", 4 },
                { "dataSource.schema1.tableau2.attr3", 2.7 }
            }))
            .AddRow(conf => conf.WithRowData(new() 
            {
                { "dataSource.schema1.tableau1.attr1", 2 },
                { "dataSource.schema1.tableau1.attr2", "HELLO2"},
                { "dataSource.schema1.tableau1.attr3", 4.5},
                { "dataSource.schema1.tableau2.attr1", 5 },
                { "dataSource.schema1.tableau2.attr3", 2.3 }
            }))
            .AddRow(conf => conf.WithRowData(new() 
            {
                { "dataSource.schema1.tableau1.attr1", 3 },
                { "dataSource.schema1.tableau1.attr2", "HELL3"},
                { "dataSource.schema1.tableau1.attr3", 3.3},
                { "dataSource.schema1.tableau2.attr1", 5 },
                { "dataSource.schema1.tableau2.attr3", 1.0 }
            }))
            .Build();

        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        mediator2.QueryRequestReceived += async (sender, args) => 
            await mediator2.SendQueryResponse(args.ReceivedMessage.ExchangeId, queryResponseData, args.FromRemotePoint);


        var registerResult = await mediator1.RegisterRemotePoint(mediator2RemotePoint);

        var queryResult = await mediator1.SendQueryRequest(query, mediator2RemotePoint);

        Assert.True(registerResult);
        Assert.True(queryResult);
        Assert.Equal(queryResponseData, queryResult.Data);

    }

    [Fact(DisplayName = "Send a SCHEMA_REQ and get results")]
    public async void SendSchemaRequest()
    {
        var dataSource = GetDataSource();

        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);


        var registerResult = await mediator1.RegisterRemotePoint(mediator2RemotePoint);

        mediator2.SchemaRequestReceived += async (sender, args) 
            => await mediator2.SendSchemaResponse(args.ReceivedMessage.ExchangeId, dataSource, args.FromRemotePoint);

        var schemaRequestResult = await mediator1.SendSchemaRequest(mediator2RemotePoint);

        Assert.True(registerResult);
        Assert.True(schemaRequestResult);
        Assert.Equal(dataSource, schemaRequestResult.Data);
    }
}
