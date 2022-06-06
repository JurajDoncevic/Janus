using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Remotes;
using System.Net.Sockets;

namespace Janus.Communication.Tests.Mocks
{
    public class AlwaysTimeoutTcpNetworkAdapter : NetworkAdapters.Tcp.NetworkAdapter, IMediatorNetworkAdapter
    {
        public AlwaysTimeoutTcpNetworkAdapter(int listenPort) : base(listenPort)
        {
            _tcpListener.Server.ReceiveTimeout = 1;
            _tcpListener.Server.SendTimeout = 1;
        }

        public event EventHandler<HelloReqReceivedEventArgs> HelloRequestMessageReceived;
        public event EventHandler<HelloResReceivedEventArgs> HelloResponseMessageReceived;
        public event EventHandler<QueryReqReceivedEventArgs> QueryRequestReceived;
        public event EventHandler<QueryResReceivedEventArgs> QueryResponseReceived;
        public event EventHandler<SchemaReqReceivedEventArgs> SchemaRequestReceived;
        public event EventHandler<SchemaResReceivedEventArgs> SchemaResponseReceived;
        public event EventHandler<CommandReqReceivedEventArgs> CommandRequestReceived;
        public event EventHandler<CommandResReceivedEventArgs> CommandResponseReceived;

        public override DataResult<BaseMessage> BuildSpecializedMessage(string preambule, byte[] messageBytes)
        {
            throw new NotImplementedException();
        }

        public override void RaiseSpecializedMessageReceivedEvent(BaseMessage message, string address)
        {
            throw new NotImplementedException();
        }

        public Result SendCommandRequest(CommandReqMessage message, RemotePoint remotePoint)
        {
            throw new NotImplementedException();
        }

        public Result SendCommandResponse(CommandResMessage message, RemotePoint remotePoint)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> SendHelloRequest(HelloReqMessage message, RemotePoint remotePoint)
        => await ResultExtensions.AsResult(
            () =>
                Using(() => new TcpClient(remotePoint.Address.ToString(), (int)remotePoint.Port),
                      async tcpClient =>
                      {
                          tcpClient.SendTimeout = 1;
                          var messageBytes = message.ToBson();
                          return tcpClient.Connected && await tcpClient.Client.SendAsync(message.ToBson(), SocketFlags.None) == messageBytes.Length;
                      }
                    )
            );

        public async Task<Result> SendHelloResponse(HelloResMessage message, RemotePoint remotePoint)
            => await ResultExtensions.AsResult(
                () =>
                    Using(() => new TcpClient(remotePoint.Address.ToString(), (int)remotePoint.Port),
                          async tcpClient =>
                          {
                              tcpClient.SendTimeout = 1;
                              var messageBytes = message.ToBson();
                              return tcpClient.Connected && await tcpClient.Client.SendAsync(message.ToBson(), SocketFlags.None) == messageBytes.Length;
                          }
                        )
                );

        public Result SendQueryRequest(QueryReqMessage message, RemotePoint remotePoint)
        {
            throw new NotImplementedException();
        }

        public Result SendQueryResponse(QueryResMessage message, RemotePoint remotePoint)
        {
            throw new NotImplementedException();
        }

        public Result SendSchemaRequest(SchemaReqMessage message, RemotePoint remotePoint)
        {
            throw new NotImplementedException();
        }

        public Result SendSchemaResponse(SchemaResMessage message, RemotePoint remotePoint)
        {
            throw new NotImplementedException();
        }
    }
}
