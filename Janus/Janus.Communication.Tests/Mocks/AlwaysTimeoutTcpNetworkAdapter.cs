﻿using Janus.Commons.Messages;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Remotes;
using Janus.Serialization;
using System.Net.Sockets;

namespace Janus.Communication.Tests.Mocks;

public class AlwaysTimeoutTcpNetworkAdapter : NetworkAdapters.Tcp.NetworkAdapter, IMediatorNetworkAdapter
{
    public AlwaysTimeoutTcpNetworkAdapter(int listenPort, IBytesSerializationProvider serializationProvider) : base(listenPort, serializationProvider)
    {
        _tcpListener.Server.ReceiveTimeout = 1;
        _tcpListener.Server.SendTimeout = 1;
    }

    public event EventHandler<QueryReqReceivedEventArgs>? QueryRequestReceived;
    public event EventHandler<QueryResReceivedEventArgs>? QueryResponseReceived;
    public event EventHandler<SchemaReqReceivedEventArgs>? SchemaRequestReceived;
    public event EventHandler<SchemaResReceivedEventArgs>? SchemaResponseReceived;
    public event EventHandler<CommandReqReceivedEventArgs>? CommandRequestReceived;
    public event EventHandler<CommandResReceivedEventArgs>? CommandResponseReceived;

    public override Result<BaseMessage> BuildSpecializedMessage(string preambule, byte[] messageBytes)
    {
        throw new NotImplementedException();
    }

    public override void RaiseSpecializedMessageReceivedEvent(BaseMessage message, string address)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendCommandRequest(CommandReqMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendCommandResponse(CommandResMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public new async Task<Result> SendHelloRequest(HelloReqMessage message, RemotePoint remotePoint)
        => await Results.AsResult(
            () =>
                Using(() => new TcpClient(remotePoint.Address.ToString(), (int)remotePoint.Port),
                      async tcpClient =>
                      {
                          System.Threading.Tasks.Task.Delay(1000).Wait();
                          tcpClient.SendTimeout = 1;
                          var messageBytes = _serializationProvider.HelloReqMessageSerializer.Serialize(message).Data!;
                          return tcpClient.Connected && await tcpClient.Client.SendAsync(messageBytes, SocketFlags.None) == messageBytes.Length;
                      }
                    )
            );

    public new async Task<Result> SendHelloResponse(HelloResMessage message, RemotePoint remotePoint)
        => await Results.AsResult(
            () =>
                Using(() => new TcpClient(remotePoint.Address.ToString(), (int)remotePoint.Port),
                      async tcpClient =>
                      {
                          System.Threading.Tasks.Task.Delay(1000).Wait();
                          tcpClient.SendTimeout = 1;
                          var messageBytes = _serializationProvider.HelloResMessageSerializer.Serialize(message).Data!;
                          return tcpClient.Connected && await tcpClient.Client.SendAsync(messageBytes, SocketFlags.None) == messageBytes.Length;
                      }
                    )
            );

    public Task<Result> SendQueryRequest(QueryReqMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendQueryResponse(QueryResMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendSchemaRequest(SchemaReqMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendSchemaResponse(SchemaResMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }
}
