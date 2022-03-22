using Janus.Communication.Messages;
using Janus.Communication.Remotes;
using System.Net;
using System.Net.Sockets;
using static FunctionalExtensions.Base.Disposing;

namespace Janus.Communication.NetworkAdapters.Tcp;

/// <summary>
/// Base class for a TCP network adapter
/// </summary>
public abstract class NetworkAdapter : INetworkAdapter
{
    protected readonly TcpListener _tcpListener;
    protected readonly int _listenPort;
    protected readonly CancellationTokenSource _cancellationTokenSource;
    protected readonly Task _listenerTask;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="listenPort"></param>
    protected NetworkAdapter(int listenPort)
    {
        _listenPort = listenPort;
        _tcpListener = new TcpListener(System.Net.IPAddress.Any, listenPort);
        _cancellationTokenSource = new CancellationTokenSource();
        _listenerTask = Task.Run(() => RunListener(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
    }

    /// <summary>
    /// Runs a TCP listener that waits for incoming messages. This method is only top be run in a Task, due to while(true)
    /// </summary>
    /// <param name="cancellationToken"></param>
    public void RunListener(CancellationToken cancellationToken)
    {
        _tcpListener.Start();
        while (true)
        {
            // maybe the task is cancelled?
            cancellationToken.ThrowIfCancellationRequested();
            // await a client
            var client = _tcpListener.AcceptTcpClient();  
            // see how many bytes the client is sending
            int countBytes = client.ReceiveBufferSize;
            // create a message bytes buffer
            byte[] messageBytes = new byte[countBytes];
            // receive the bytes
            client.GetStream().Read(messageBytes, 0, countBytes);

            var str = Encoding.UTF8.GetString(messageBytes, 0, countBytes);
            // determine message type and raise event
            messageBytes.DeterminePreambule()
                        .Bind<string, BaseMessage>(_ => BuildMessage(_, messageBytes))
                        .Map(_ => { RaiseMessageReceivedEvent(_, ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()); return _; });
        }
    }

    /// <summary>
    /// Invoked when a HELLO_REQ message is received
    /// </summary>
    public event EventHandler<HelloRequestReceivedEventArgs> HelloRequestMessageReceived;
    /// <summary>
    /// Invoked when a HELLO_RES message is received
    /// </summary>
    public event EventHandler<HelloResponseReceivedEventArgs> HelloResponseMessageReceived;

    /// <summary>
    /// Sends a HELLO_REQ message to the remote point
    /// </summary>
    /// <param name="message">HELLO_REQ message</param>
    /// <param name="remotePoint">Target remote point</param>
    /// <returns></returns>
    public async Task<Result> SendHelloRequest(HelloReqMessage message, RemotePoint remotePoint)
        => await ResultExtensions.AsResult(
            () =>
                Using(() => new TcpClient(remotePoint.Address.ToString(), (int)remotePoint.Port),
                      async tcpClient =>
                      {
                          var messageBytes = message.ToBson();
                          return tcpClient.Connected && await tcpClient.Client.SendAsync(message.ToBson(), SocketFlags.None) == messageBytes.Length;
                      }
                    )
            );

    /// <summary>
    /// Sends a HELLO_RES message to the remote point
    /// </summary>
    /// <param name="message">HELLO_RES message</param>
    /// <param name="remotePoint">Target remote point</param>
    /// <returns></returns>
    public async Task<Result> SendHelloResponse(HelloResMessage message, RemotePoint remotePoint)
        => await ResultExtensions.AsResult(
            () =>
                Using(() => new TcpClient(remotePoint.Address.ToString(), (int)remotePoint.Port),
                      async tcpClient =>
                      {
                          var messageBytes = message.ToBson();
                          return tcpClient.Connected && await tcpClient.Client.SendAsync(message.ToBson(), SocketFlags.None) == messageBytes.Length;
                      }
                    )
            );

    /// <summary>
    /// Base method to raise message events
    /// </summary>
    /// <param name="message">Message instance</param>
    /// <param name="address">Address from which the message was received</param>
    private void RaiseMessageReceivedEvent(BaseMessage message, string address)
    {
        switch (message.Preamble)
        {
            case Preambles.HELLO_REQUEST:   HelloRequestMessageReceived?.Invoke(this, new HelloRequestReceivedEventArgs((HelloReqMessage)message, address));
                                            break;
            case Preambles.HELLO_RESPONSE:  HelloResponseMessageReceived?.Invoke(this, new HelloResponseReceivedEventArgs((HelloResMessage)message, address));
                                            break;
            default:        RaiseSpecializedMessageReceivedEvent(message, address);
                            break;
        }
    }

    /// <summary>
    /// Specialization method to raise message events for subclass node types
    /// </summary>
    /// <param name="message">Message instance</param>
    /// <param name="address">Address from which the message was received</param>
    public abstract void RaiseSpecializedMessageReceivedEvent(BaseMessage message, string address);

    /// <summary>
    /// Creates base messages from bytes read over the network
    /// </summary>
    /// <param name="preambule">Message preambule</param>
    /// <param name="messageBytes">Message bytes</param>
    /// <returns>Created message boxed as BaseMessage</returns>
    private DataResult<BaseMessage> BuildMessage(string preambule, byte[] messageBytes)
        => preambule switch
        {
            Preambles.HELLO_REQUEST => MessageExtensions.ToHelloReqMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.HELLO_RESPONSE => MessageExtensions.ToHelloResMessage(messageBytes).Map(_ => (BaseMessage)_),
            _ => BuildSpecializedMessage(preambule, messageBytes)
        };

    /// <summary>
    /// Creates node specialized messages from bytes read over the network
    /// </summary>
    /// <param name="preambule">Message preambule</param>
    /// <param name="messageBytes">Message bytes</param>
    /// <returns>Created message boxed as BaseMessage</returns>
    public abstract DataResult<BaseMessage> BuildSpecializedMessage(string preambule, byte[] messageBytes);

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _tcpListener.Stop();
    }
}
