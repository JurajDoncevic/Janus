﻿using Janus.Commons.Messages;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;
using Janus.Communication.Tests.TestFixtures;
using Xunit;

namespace Janus.Communication.Tests;

public abstract class CommunicationNodeTests<TFixture> : IClassFixture<TFixture> where TFixture : CommunicationNodeTestFixture
{
    private readonly CommunicationNodeTestFixture _testFixture;

    protected CommunicationNodeTests(TFixture testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact(DisplayName = "BASE: Exchange HELLO between 2 nodes")]
    public void SendHello()
    {
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        HelloReqEventArgs? helloRequestEventArgs = null;

        mediator2.HelloRequestReceived += (_, args) => helloRequestEventArgs = args;

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);

        var helloResult = mediator1.SendHello(mediator2RemotePoint).Result;
        var resultRemotePoint = helloResult.Data;

        Assert.True(helloResult.IsSuccess);
        Assert.Equal(mediator2.Options.NodeId, resultRemotePoint?.NodeId);
        Assert.Equal(mediator2.Options.ListenPort, resultRemotePoint?.Port);
        Assert.Empty(mediator1.RemotePoints);
        Assert.Empty(mediator2.RemotePoints);
        Assert.NotNull(helloRequestEventArgs);
        Assert.Equal(mediator1.Options.NodeId, helloRequestEventArgs?.ReceivedMessage.NodeId);

    }

    [Fact(DisplayName = "BASE: Register nodes using HELLO")]
    public void SendRegister()
    {
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var registerResult = mediator1.RegisterRemotePoint(mediator2RemotePoint).Result;
        var resultRemotePoint = registerResult.Data;

        Assert.True(registerResult.IsSuccess);
        Assert.Equal(mediator2.Options.NodeId, resultRemotePoint?.NodeId);
        Assert.Equal(mediator2.Options.ListenPort, resultRemotePoint?.Port);
        Assert.Contains(resultRemotePoint, mediator1.RemotePoints);
        Assert.Equal(mediator2.RemotePoints.First(), mediator1RemotePoint);

    }


    [Fact(DisplayName = "BASE: Refuse mediator register on node with same ID")]
    public void RefuseMediatorRegisterOnSameId()
    {
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator1_Copy");

        var mediator2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var registerResult = mediator1.RegisterRemotePoint(mediator2RemotePoint).Result;
        var resultRemotePoint = registerResult.Data;

        Assert.False(registerResult);
        Assert.Contains("same id", registerResult.Message);
        Assert.Empty(mediator1.RemotePoints);
        Assert.Empty(mediator2.RemotePoints);

    }

    [Fact(DisplayName = "BASE: Refuse mask register on node with same ID")]
    public void RefuseMaskRegisterOnSameId()
    {
        using var mask = _testFixture.GetMaskCommunicationNode("MaskNodeX");
        using var mediator = _testFixture.GetMediatorCommunicationNode("MediatorNodeX");

        var mediatorRemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator.Options.ListenPort);
        var maskRemotePoint = new MaskRemotePoint(mask.Options.NodeId, "127.0.0.1", mask.Options.ListenPort);

        var registerResult = mask.RegisterRemotePoint(mediatorRemotePoint).Result;
        var resultRemotePoint = registerResult.Data;

        Assert.False(registerResult);
        Assert.Contains("same id", registerResult.Message);
        Assert.DoesNotContain(maskRemotePoint, mediator.RemotePoints);
        Assert.DoesNotContain(mediatorRemotePoint, mask.RemotePoints);

    }

    [Fact(DisplayName = "BASE: Refuse wrapper register on node with same ID")]
    public void RefuseWrapperRegisterOnSameId()
    {
        using var wrapper = _testFixture.GetWrapperCommunicationNode("WrapperNodeX");
        using var mediator = _testFixture.GetMediatorCommunicationNode("MediatorNodeX");

        var mediatorRemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator.Options.ListenPort);
        var wrapperRemotePoint = new WrapperRemotePoint(wrapper.Options.NodeId, "127.0.0.1", wrapper.Options.ListenPort);

        var registerResult = wrapper.RegisterRemotePoint(mediatorRemotePoint).Result;
        var resultRemotePoint = registerResult.Data;

        Assert.False(registerResult);
        Assert.Contains("same id", registerResult.Message);
        Assert.Empty(mediator.RemotePoints);
        Assert.Empty(wrapper.RemotePoints);

    }

    [Fact(DisplayName = "BASE: Refuse mask on mask register")]
    public void RefuseMaskOnMaskRegister()
    {
        using var mask1 = _testFixture.GetMaskCommunicationNode("Mask1");
        using var mask2 = _testFixture.GetMaskCommunicationNode("Mask2");

        var mask2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mask2.Options.ListenPort);
        var mask1RemotePoint = new MaskRemotePoint(mask1.Options.NodeId, "127.0.0.1", mask1.Options.ListenPort);

        var registerResult = mask1.RegisterRemotePoint(mask2RemotePoint).Result;
        var resultRemotePoint = registerResult.Data;

        Assert.False(registerResult);
        Assert.Empty(mask2.RemotePoints);
        Assert.Empty(mask1.RemotePoints);

    }

    [Fact(DisplayName = "BASE: Refuse wrapper on wrapper register")]
    public void RefuseWrapperOnWrapperRegister()
    {
        using var wrapper1 = _testFixture.GetWrapperCommunicationNode("Wrapper1");
        using var wrapper2 = _testFixture.GetWrapperCommunicationNode("Wrapper2");

        var wrapper2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", wrapper2.Options.ListenPort);
        var wrapper1RemotePoint = new WrapperRemotePoint(wrapper1.Options.NodeId, "127.0.0.1", wrapper1.Options.ListenPort);

        var registerResult = wrapper1.RegisterRemotePoint(wrapper2RemotePoint).Result;
        var resultRemotePoint = registerResult.Data;

        Assert.False(registerResult);
        Assert.Empty(wrapper2.RemotePoints);
        Assert.Empty(wrapper1.RemotePoints);

    }

    [Fact(DisplayName = "BASE: Refuse more than one registered node on mask")]
    public void RefuseMoreThanOneRegisteredNodeOnMask()
    {
        using var mask = _testFixture.GetMaskCommunicationNode("Mask1");
        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator1RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator1.Options.ListenPort);
        var mediator2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var maskRemotePoint = new MaskRemotePoint(mask.Options.NodeId, "127.0.0.1", mask.Options.ListenPort);
        var undeterminedMaskRemotePoint = new UndeterminedRemotePoint("127.0.0.1", mask.Options.ListenPort);

        var mediator1RegisterResult = mask.RegisterRemotePoint(mediator1RemotePoint).Result;
        var mediator2RegisterResult = mediator2.RegisterRemotePoint(undeterminedMaskRemotePoint).Result;
        var resultRemotePoint = mediator1RegisterResult.Data;

        Assert.True(mediator1RegisterResult);
        Assert.Contains(maskRemotePoint, mediator1.RemotePoints);
        Assert.Contains(new MediatorRemotePoint(mediator1.Options.NodeId, mediator1RemotePoint.Address, mediator1RemotePoint.Port), mask.RemotePoints);

        Assert.False(mediator2RegisterResult);
        Assert.DoesNotContain(maskRemotePoint, mediator2.RemotePoints);
        Assert.DoesNotContain(new MediatorRemotePoint(mediator2.Options.NodeId, mediator2RemotePoint.Address, mediator2RemotePoint.Port), mask.RemotePoints);

    }

    [Fact(DisplayName = "BASE: Test HELLO timeout")]
    public void SendHelloTimeout()
    {
        using var mediator1 = _testFixture.GetUnresponsiveMediator();
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator1");

        var mediator2RemotePoint = new MediatorRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var helloResult = mediator2.SendHello(mediator1RemotePoint).Result;


        Assert.False(helloResult);
        Assert.Contains("timeout", helloResult.Message.ToLower());
    }

    [Fact(DisplayName = "BASE: Test timeout on register operation")]
    public void RegisterTimeout()
    {
        using var mediator1 = _testFixture.GetUnresponsiveMediator();
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator1");

        var mediator2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator1.Options.ListenPort);

        var helloResult = mediator2.RegisterRemotePoint(mediator1RemotePoint).Result;


        Assert.False(helloResult);
        Assert.Contains("timeout", helloResult.Message.ToLower());
    }

    [Fact(DisplayName = "BASE: Send a BYE after a Register")]
    public async void TestBye()
    {
        var schema = _testFixture.GetSchema();
        var tableauId = schema["schema1"]["tableau1"].Id;

        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
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

    [Fact(DisplayName = "MEDIATOR: Send a COMMAND_REQ, respond and get results")]
    public async void MediatorSendCommandRequest()
    {
        var schema = _testFixture.GetSchema();
        var tableauId = schema["schema1"]["tableau1"].Id;

        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        mediator2.CommandRequestReceived += async (sender, args) =>
        {
            if (args.ReceivedMessage.CommandReqType == CommandReqTypes.INSERT)
            {
                await mediator2.SendCommandResponse(args.ReceivedMessage.ExchangeId, true, args.FromRemotePoint, "Operation success");
            }
        };

        var registerResult = await mediator1.RegisterRemotePoint(mediator2RemotePoint);

        var commandResult = await mediator1.SendCommandRequest(_testFixture.GetInsertCommand(), mediator2RemotePoint);

        Assert.True(registerResult);
        Assert.True(commandResult);

    }

    [Fact(DisplayName = "MEDIATOR: Send a QUERY_REQ, respond and get results")]
    public async void MediatorSendQueryRequest()
    {
        var schema = _testFixture.GetSchema();
        var tableauId = schema["schema1"]["tableau1"].Id;

        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);

        var query = _testFixture.GetQuery();
        var queryResultData = _testFixture.GetQueryResultData();

        mediator2.QueryRequestReceived += async (sender, args) =>
            await mediator2.SendQueryResponse(args.ReceivedMessage.ExchangeId, queryResultData, args.FromRemotePoint);


        var registerResult = await mediator1.RegisterRemotePoint(mediator2RemotePoint);

        var queryResult = await mediator1.SendQueryRequest(query, mediator2RemotePoint);

        Assert.True(registerResult);
        Assert.True(queryResult);
        Assert.Equal(queryResultData, queryResult.Data);

    }

    [Fact(DisplayName = "MEDIATOR: Send a SCHEMA_REQ, respond and get results")]
    public async void MediatorSendSchemaRequest()
    {
        var schema = _testFixture.GetSchema();

        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);


        var registerResult = await mediator1.RegisterRemotePoint(mediator2RemotePoint);

        mediator2.SchemaRequestReceived += async (sender, args)
            => await mediator2.SendSchemaResponse(args.ReceivedMessage.ExchangeId, schema, args.FromRemotePoint);

        var schemaRequestResult = await mediator1.SendSchemaRequest(mediator2RemotePoint);

        Assert.True(registerResult);
        Assert.True(schemaRequestResult);
        Assert.Equal(schema, schemaRequestResult.Data);
    }

    [Fact(DisplayName = "MEDIATOR: Send a SCHEMA_REQ, respond and get failing results")]
    public async void MediatorSendSchemaRequestFailing()
    {
        var schema = _testFixture.GetSchema();

        using var mediator1 = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var mediator2 = _testFixture.GetMediatorCommunicationNode("Mediator2");

        var mediator2RemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator2.Options.ListenPort);
        var mediator1RemotePoint = new MediatorRemotePoint(mediator1.Options.NodeId, "127.0.0.1", mediator1.Options.ListenPort);


        var registerResult = await mediator1.RegisterRemotePoint(mediator2RemotePoint);

        mediator2.SchemaRequestReceived += async (sender, args)
            => await mediator2.SendSchemaResponse(args.ReceivedMessage.ExchangeId, null, args.FromRemotePoint, "Some failure");

        var schemaRequestResult = await mediator1.SendSchemaRequest(mediator2RemotePoint);

        Assert.True(registerResult);
        Assert.False(schemaRequestResult);
        Assert.Contains("Some", schemaRequestResult.Message);
    }

    [Fact(DisplayName = "MASK: Send a SCHEMA_REQ and get results")]
    public async void MaskSendSchemaRequest()
    {
        var schema = _testFixture.GetSchema();

        using var mediator = _testFixture.GetMediatorCommunicationNode("Mediator1");
        var mediatorRemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator.Options.ListenPort);
        mediator.SchemaRequestReceived += async (sender, args)
            => await mediator.SendSchemaResponse(args.ReceivedMessage.ExchangeId, schema, args.FromRemotePoint);

        using var mask = _testFixture.GetMaskCommunicationNode("Mask1");

        var registerResult = await mask.RegisterRemotePoint(mediatorRemotePoint);

        var schemaResult = await mask.SendSchemaRequest(mediatorRemotePoint);

        Assert.True(registerResult);
        Assert.True(schemaResult);
        Assert.Equal(schema, schemaResult.Data);
    }

    [Fact(DisplayName = "MASK: Send a SCHEMA_REQ and get failing results")]
    public async void MaskSendSchemaRequestFailing()
    {
        var schema = _testFixture.GetSchema();

        using var mediator = _testFixture.GetMediatorCommunicationNode("Mediator1");
        var mediatorRemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator.Options.ListenPort);
        mediator.SchemaRequestReceived += async (sender, args)
            => await mediator.SendSchemaResponse(args.ReceivedMessage.ExchangeId, null, args.FromRemotePoint, "Some failure");

        using var mask = _testFixture.GetMaskCommunicationNode("Mask1");

        var registerResult = await mask.RegisterRemotePoint(mediatorRemotePoint);

        var schemaResult = await mask.SendSchemaRequest(mediatorRemotePoint);

        Assert.True(registerResult);
        Assert.False(schemaResult);
        Assert.Contains("Some", schemaResult.Message);
    }

    [Fact(DisplayName = "MASK: Send a QUERY_REQ and get results")]
    public async void MaskSendQueryRequest()
    {
        var query = _testFixture.GetQuery();
        var queryResultData = _testFixture.GetQueryResultData();

        using var mediator = _testFixture.GetMediatorCommunicationNode("Mediator1");
        var mediatorRemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator.Options.ListenPort);
        mediator.QueryRequestReceived += async (sender, args)
            => await mediator.SendQueryResponse(args.ReceivedMessage.ExchangeId, queryResultData, args.FromRemotePoint);

        using var mask = _testFixture.GetMaskCommunicationNode("Mask1");

        var registerResult = await mask.RegisterRemotePoint(mediatorRemotePoint);

        var queryDataResult = await mask.SendQueryRequest(query, mediatorRemotePoint);

        Assert.True(registerResult);
        Assert.True(queryDataResult);
        Assert.Equal(_testFixture.GetQueryResultData(), queryDataResult.Data);
    }

    [Fact(DisplayName = "MASK: Send a QUERY_REQ and get failing results")]
    public async void MaskSendQueryRequestFailing()
    {
        var query = _testFixture.GetQuery();
        var queryResultData = _testFixture.GetQueryResultData();

        using var mediator = _testFixture.GetMediatorCommunicationNode("Mediator1");
        var mediatorRemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator.Options.ListenPort);
        mediator.QueryRequestReceived += async (sender, args)
            => await mediator.SendQueryResponse(args.ReceivedMessage.ExchangeId, null, args.FromRemotePoint, "Some failure");

        using var mask = _testFixture.GetMaskCommunicationNode("Mask1");

        var registerResult = await mask.RegisterRemotePoint(mediatorRemotePoint);

        var queryDataResult = await mask.SendQueryRequest(query, mediatorRemotePoint);

        Assert.True(registerResult);
        Assert.False(queryDataResult);
        Assert.Contains("Some", queryDataResult.Message);
    }

    [Fact(DisplayName = "MASK: Send a COMMAND_REQ and get results")]
    public async void MaskSendCommandRequest()
    {
        var insertCommand = _testFixture.GetInsertCommand();

        using var mediator = _testFixture.GetMediatorCommunicationNode("Mediator1");
        var mediatorRemotePoint = new UndeterminedRemotePoint("127.0.0.1", mediator.Options.ListenPort);
        mediator.CommandRequestReceived += async (sender, args)
            => await mediator.SendCommandResponse(args.ReceivedMessage.ExchangeId, true, args.FromRemotePoint);

        using var mask = _testFixture.GetMaskCommunicationNode("Mask1");

        var registerResult = await mask.RegisterRemotePoint(mediatorRemotePoint);

        var queryDataResult = await mask.SendCommandRequest(insertCommand, mediatorRemotePoint);

        Assert.True(registerResult);
        Assert.True(queryDataResult);
    }

    [Fact(DisplayName = "WRAPPER: Receive a SCHEMA_REQ and respond")]
    public async void WrapperReceiveSchemaRequest()
    {
        var schema = _testFixture.GetSchema();

        using var mediator = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var wrapper = _testFixture.GetWrapperCommunicationNode("Wrapper1");
        var remotePoint = new UndeterminedRemotePoint("127.0.0.1", wrapper.Options.ListenPort);
        wrapper.SchemaRequestReceived +=
            async (sender, args) => await wrapper.SendSchemaResponse(args.ReceivedMessage.ExchangeId, schema, args.FromRemotePoint);

        var registerResult = await mediator.RegisterRemotePoint(remotePoint);
        var wrapperRemotePoint = registerResult.Data;

        var schemaResult = await mediator.SendSchemaRequest(wrapperRemotePoint);

        Assert.True(registerResult);
        Assert.True(schemaResult);
        Assert.Equal(schema, schemaResult.Data);
    }

    [Fact(DisplayName = "WRAPPER: Receive a QUERY_REQ and respond")]
    public async void WrapperReceiveQueryRequest()
    {
        var query = _testFixture.GetQuery();
        var queryResultData = _testFixture.GetQueryResultData();

        using var mediator = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var wrapper = _testFixture.GetWrapperCommunicationNode("Wrapper1");
        var remotePoint = new UndeterminedRemotePoint("127.0.0.1", wrapper.Options.ListenPort);
        wrapper.QueryRequestReceived +=
            async (sender, args) => await wrapper.SendQueryResponse(args.ReceivedMessage.ExchangeId, queryResultData, args.FromRemotePoint);

        var registerResult = await mediator.RegisterRemotePoint(remotePoint);
        var wrapperRemotePoint = registerResult.Data;

        var queryResult = await mediator.SendQueryRequest(query, wrapperRemotePoint);

        Assert.True(registerResult);
        Assert.True(queryResult);
        Assert.Equal(queryResultData, queryResult.Data);
    }

    [Fact(DisplayName = "WRAPPER: Receive a COMMAND_REQ and respond")]
    public async void WrapperReceiveCommandRequest()
    {
        var insertCommand = _testFixture.GetInsertCommand();

        using var mediator = _testFixture.GetMediatorCommunicationNode("Mediator1");
        using var wrapper = _testFixture.GetWrapperCommunicationNode("Wrapper1");
        var remotePoint = new UndeterminedRemotePoint("127.0.0.1", wrapper.Options.ListenPort);
        wrapper.CommandRequestReceived +=
            async (sender, args) => await wrapper.SendCommandResponse(args.ReceivedMessage.ExchangeId, true, args.FromRemotePoint);

        var registerResult = await mediator.RegisterRemotePoint(remotePoint);
        var wrapperRemotePoint = registerResult.Data;

        var commandResult = await mediator.SendCommandRequest(insertCommand, wrapperRemotePoint);

        Assert.True(registerResult);
        Assert.True(commandResult);
    }
}
